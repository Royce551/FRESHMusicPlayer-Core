using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRESHMusicPlayer.Backends
{
    /// <summary>
    /// A wrapper for some type of audio library for playing audio
    /// </summary>
    public interface IAudioBackend : IDisposable
    {
        /// <summary>
        /// Loads the track and gets the backend ready for playback
        /// </summary>
        /// <param name="file">File path of the track to be played</param>
        /// <returns>The result of the load attempt</returns>
        Task<BackendLoadResult> LoadSongAsync(string file);
        /// <summary>
        /// Gets metadata for the given file path
        /// </summary>
        /// <param name="file">File path of the track to query metadata for</param>
        /// <returns>The metadata</returns>
        Task<IMetadataProvider> GetMetadataAsync(string file);
        /// <summary>
        /// Begins playback. If paused, begins playback again
        /// </summary>
        void Play();
        /// <summary>
        /// Pauses playback but keeps the backend ready to continue
        /// </summary>
        void Pause();

        /// <summary>
        /// The current playhead position in the track
        /// </summary>
        TimeSpan CurrentTime { get; set; }
        /// <summary>
        /// The total length of the track
        /// </summary>
        TimeSpan TotalTime { get; }

        /// <summary>
        /// The current playback volume, from 0 to 1
        /// </summary>
        float Volume { get; set; }

        /// <summary>
        /// Raised when playback stops
        /// </summary>
        event EventHandler<EventArgs> OnPlaybackStopped;
    }

    /// <summary>
    /// Should be populated with info about the track the backend is playing
    /// </summary>
    public interface IMetadataProvider
    {
        /// <summary>
        /// Title of the track
        /// </summary>
        string Title { get; }
        /// <summary>
        /// Artists of the track
        /// </summary>
        string[] Artists { get; }
        /// <summary>
        /// The album the track comes from
        /// </summary>
        string Album { get; }
        /// <summary>
        /// Binary image data for the, ideally, front cover of the album
        /// </summary>
        byte[] CoverArt { get; }
        /// <summary>
        /// Genres of the track
        /// </summary>
        string[] Genres { get; }
        /// <summary>
        /// Year the track was made. If not available, 0
        /// </summary>
        int Year { get; }
        /// <summary>
        /// Track's track number. If not available, 0
        /// </summary>
        int TrackNumber { get; }
        /// <summary>
        /// Total number of tracks in the album the track is in. If not available, 0
        /// </summary>
        int TrackTotal { get; }
        /// <summary>
        /// Disc number of the album the track is in. If not available, 0
        /// </summary>
        int DiscNumber { get; }
        /// <summary>
        /// Total number of discs in the album the track is in. If not available, 0
        /// </summary>
        int DiscTotal { get; }
        /// <summary>
        /// The length of the track in seconds
        /// </summary>
        int Length { get; }
    }

    /// <summary>
    /// The result of a backend load attempt
    /// </summary>
    public enum BackendLoadResult
    {
        /// <summary>
        /// The backend loaded successfully and is really to start playing!
        /// </summary>
        OK,
        /// <summary>
        /// The backend does not support the kind of file given
        /// </summary>
        NotSupported,
        /// <summary>
        /// An invalid file was given
        /// </summary>
        Invalid,
        /// <summary>
        /// There is something wrong with the file given
        /// </summary>
        Corrupt,
        /// <summary>
        /// Something else went wrong
        /// </summary>
        UnknownError
    }
}
