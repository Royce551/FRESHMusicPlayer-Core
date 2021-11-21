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
    class NAudioBackend : IAudioBackend
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
                    OutputDevice.Init(AudioFile);
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
            OutputDevice.Dispose();
            AudioFile.Dispose();
        }

        public void Pause()
        {
            OutputDevice.Pause();
        }
    }
}
