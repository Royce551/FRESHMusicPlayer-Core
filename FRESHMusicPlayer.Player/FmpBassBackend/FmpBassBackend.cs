using FRESHMusicPlayer.Backends;
using ManagedBass;
using System;
using System.Composition;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace FmpBassBackend
{
    [Export(typeof(IAudioBackend))]
    public class FmpBassBackend : IAudioBackend
    {
        public TimeSpan CurrentTime { get => player.Position; set => player.Position = value; }

        public TimeSpan TotalTime { get => player.Duration; }

        public float Volume { get => (float)player.Volume; set => player.Volume = value; }

        public event EventHandler<EventArgs> OnPlaybackStopped;

        public IMetadataProvider Metadata { get; private set; }

        private readonly MediaPlayer player = new MediaPlayer();

        public FmpBassBackend()
        {
            player.MediaEnded += Player_MediaEnded;
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) // on windows media foundation already provides flac support,
            {                                                         // don't bother
                var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                Bass.PluginLoad(Path.Combine(currentDirectory, GetExtensionForCurrentPlatform("bassflac")));
            }
        }

        private void Player_MediaEnded(object sender, EventArgs e) => OnPlaybackStopped?.Invoke(null, EventArgs.Empty);

        public void Dispose() => player.Dispose();

        public async Task<BackendLoadResult> LoadSongAsync(string file)
        {
            var wasSuccessful = await player.LoadAsync(file);

            Metadata = new FileMetadataProvider(file);

            if (!wasSuccessful) return BackendLoadResult.Invalid;
            else return BackendLoadResult.OK;
        }

        public void Pause() => player.Pause();

        public void Play() => player.Play();

        private string GetExtensionForCurrentPlatform(string name)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return $"lib{name}.so";
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return $"lib{name}.dylib";
            else return $"{name}.dll";
        }
    }
}
