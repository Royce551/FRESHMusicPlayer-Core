using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRESHMusicPlayer.Backends
{
    [Export(typeof(IAudioBackend))]
    class NAudioBackend : IAudioBackend
    {
        private readonly WaveOutEvent outputDevice;

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
            if (outputDevice is null)
            {
                outputDevice = new WaveOutEvent();
                outputDevice.PlaybackStopped += (object o, StoppedEventArgs e) =>
                {
                    OnPlaybackStopped(this, new EventArgs());
                };
            }
        }

        public void LoadSong(string file)
        {
            if (AudioFile != null)
            {
                AudioFile.Dispose();
            }
            AudioFile = new AudioFileReader(file);
            outputDevice.Init(AudioFile);
        }

        public void Play()
        {
            outputDevice.Play();
        }

        public void Dispose()
        {
            outputDevice.Dispose();
            AudioFile.Dispose();
        }

        public void Pause()
        {
            outputDevice.Pause();
        }
    }
}
