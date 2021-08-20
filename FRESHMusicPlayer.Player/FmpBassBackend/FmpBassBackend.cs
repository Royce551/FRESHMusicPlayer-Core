using FRESHMusicPlayer.Backends;
using ManagedBass;
using System;
using System.Composition;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace FmpBassBackend
{
    [Export(typeof(IAudioBackend))]
    public class FmpBassBackend : IAudioBackend
    {
        public TimeSpan CurrentTime { get => player.Position; set => player.Position = value; }

        public TimeSpan TotalTime { get => player.Duration; }

        public float Volume { get => (float)player.Volume; set => player.Volume = value; }

        public event EventHandler<EventArgs> OnPlaybackStopped;

        private readonly FMPMediaPlayer player = new FMPMediaPlayer();

        public FmpBassBackend()
        {
            player.MediaEnded += Player_MediaEnded;
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) // on windows media foundation already provides flac support,
            {                                                         // don't bother
                var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                Bass.PluginLoad(Path.Combine(currentDirectory, GetExtensionForCurrentPlatform("libbassflac")));
            }
        }

        private void Player_MediaEnded(object sender, EventArgs e) => OnPlaybackStopped?.Invoke(null, EventArgs.Empty);

        public void Dispose() => player.Dispose();

        public void LoadSong(string file)
        {
            if (!player.Load(file)) throw new Exception("loading didn't work :("); // not awaited because fmpcore currently does not support await like this
        }

        public void Pause() => player.Pause();

        public void Play() => player.Play();

        private string GetExtensionForCurrentPlatform(string name)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return $"{name}.so";
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return $"{name}.dylib";
            else return $"{name}.dll";
        }
    }
}
