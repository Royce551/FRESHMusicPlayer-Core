using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRESHMusicPlayer.Backends
{
    public interface IAudioBackend : IDisposable
    {
        Task<BackendLoadResult> LoadSongAsync(string file);
        void Play();
        void Pause();

        TimeSpan CurrentTime { get; set; }
        TimeSpan TotalTime { get; }

        IMetadataProvider Metadata { get; }

        float Volume { get; set; }

        event EventHandler<EventArgs> OnPlaybackStopped;
    }

    public interface IMetadataProvider
    {
        string Title { get; }
        string[] Artists { get; }
        string Album { get; }
        byte[] CoverArt { get; }
        string[] Genres { get; }
        int Year { get; }
        int TrackNumber { get; }
        int TrackTotal { get; }
        int DiscNumber { get; }
        int DiscTotal { get; }
    }

    public enum BackendLoadResult
    {
        OK,
        NotSupported,
        Invalid,
        Corrupt
    }
}
