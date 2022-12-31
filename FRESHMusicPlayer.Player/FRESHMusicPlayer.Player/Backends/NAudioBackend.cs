using NAudio.Dsp;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRESHMusicPlayer.Backends
{
    [Export(typeof(IAudioBackend))]
    class NAudioBackend : IAudioBackend, ISupportEqualization
    {
        public WaveOutEvent OutputDevice;

        public event EventHandler<EventArgs> OnPlaybackStopped;

        public AudioFileReader AudioFile { get; set; }

        public float Volume 
        { 
            get => AudioFile.Volume; 
            set => AudioFile.Volume = value; 
        }
        public TimeSpan CurrentTime 
        {
            get => AudioFile.CurrentTime;
            set => AudioFile.CurrentTime = value;
        }
        public TimeSpan TotalTime => AudioFile.TotalTime;

        public List<EqualizerBand> EqualizerBands
        {
            get => equalizer.Bands;
            set => equalizer.Bands = value;
        }

        public void UpdateEqualizer() => equalizer?.Update();

        private Equalizer equalizer;

        public NAudioBackend()
        {
            if (OutputDevice is null)
            {
                OutputDevice = new WaveOutEvent();
                OutputDevice.PlaybackStopped += (object o, StoppedEventArgs e) =>
                {
                    OnPlaybackStopped.Invoke(null, EventArgs.Empty);
                };
            }
        }

        public async Task<BackendLoadResult> LoadSongAsync(string file)
        {
            if (AudioFile != null) AudioFile.Dispose();
            try
            {
                await Task.Run(() =>
                {
                    AudioFile = new AudioFileReader(file);
                    equalizer = new Equalizer(AudioFile, new List<EqualizerBand>());
                    OutputDevice.Init(equalizer);
                });
            }
            catch (ArgumentException)
            {
                return BackendLoadResult.Invalid;
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                return BackendLoadResult.Invalid;
            }
            catch (FormatException)
            {
                return BackendLoadResult.Corrupt;
            }
            catch (InvalidOperationException)
            {
                return BackendLoadResult.Invalid;
            }
            return BackendLoadResult.OK;
        }

        public async Task<IMetadataProvider> GetMetadataAsync(string file) => await Task.Run(() => new FileMetadataProvider(file));

        public void Play()
        {
            OutputDevice.Play();
        }

        public void Dispose()
        {
            OutputDevice?.Dispose();
            AudioFile?.Dispose();
        }

        public void Pause()
        {
            OutputDevice.Pause();
        }
    }

    public class Equalizer : ISampleProvider
    {
        private readonly ISampleProvider sourceProvider;
        public List<EqualizerBand> Bands;
        private BiQuadFilter[,] filters;
        private readonly int channels;
        private int bandCount;
        private bool updated;

        public Equalizer(ISampleProvider sourceProvider, List<EqualizerBand> bands)
        {
            this.sourceProvider = sourceProvider;
            this.Bands = bands;
            channels = sourceProvider.WaveFormat.Channels;
            bandCount = bands.Count;
            filters = new BiQuadFilter[channels, bands.Count];
            CreateFilters();
        }

        private void CreateFilters()
        {
            for (int bandIndex = 0; bandIndex < bandCount; bandIndex++)
            {
                var band = Bands[bandIndex];
                for (int n = 0; n < channels; n++)
                {
                    if (filters[n, bandIndex] == null)
                        filters[n, bandIndex] = BiQuadFilter.PeakingEQ(sourceProvider.WaveFormat.SampleRate, band.Frequency, band.Bandwidth, band.Gain);
                    else
                        filters[n, bandIndex].SetPeakingEq(sourceProvider.WaveFormat.SampleRate, band.Frequency, band.Bandwidth, band.Gain);
                }
            }
        }

        public void Update()
        {
            updated = true;
            bandCount = Bands.Count;
            filters = new BiQuadFilter[channels, Bands.Count];
            CreateFilters();
        }

        public WaveFormat WaveFormat => sourceProvider.WaveFormat;

        public int Read(float[] buffer, int offset, int count)
        {
            int samplesRead = sourceProvider.Read(buffer, offset, count);

            if (updated)
            {
                CreateFilters();
                updated = false;
            }

            for (int n = 0; n < samplesRead; n++)
            {
                int ch = n % channels;

                for (int band = 0; band < bandCount; band++)
                {
                    buffer[offset + n] = filters[ch, band].Transform(buffer[offset + n]);
                }
            }
            return samplesRead;
        }
    }
}
