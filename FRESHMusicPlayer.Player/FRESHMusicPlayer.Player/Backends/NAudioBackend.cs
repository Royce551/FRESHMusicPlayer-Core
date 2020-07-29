using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRESHMusicPlayer.Backends
{
    class NAudioBackend : IAudioBackend
    {
        private WaveOutEvent outputDevice;

        public event EventHandler<EventArgs> OnPlaybackStopped;

        public AudioFileReader AudioFile { get; set; }

        public float Volume { get => AudioFile.Volume; set => AudioFile.Volume = value; }
        public TimeSpan CurrentTime { get => AudioFile.CurrentTime; set => AudioFile.CurrentTime = value; }
        public TimeSpan TotalTime { get => AudioFile.TotalTime; }

        public NAudioBackend(string file)
        {
            if (outputDevice == null)
            {
                outputDevice = new WaveOutEvent();
                outputDevice.PlaybackStopped += (Object o, StoppedEventArgs e) =>
                {
                    OnPlaybackStopped(this, new EventArgs());
                };
            }

            if (AudioFile == null)
            {
                AudioFile = new AudioFileReader(file);
                outputDevice.Init(AudioFile);
            }
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
