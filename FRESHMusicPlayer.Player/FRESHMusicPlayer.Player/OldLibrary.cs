using ATL;
using FRESHMusicPlayer.Backends;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FRESHMusicPlayer
{
    /// <summary>
    /// Representation of a track in the database
    /// </summary>
    public class OldDatabaseTrack
    {
        /// <summary>
        /// The file path
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// Title of the track
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Semicolon separated list of artists
        /// </summary>
        public string Artist { get; set; }
        /// <summary>
        /// Album of the track
        /// </summary>
        public string Album { get; set; }
        /// <summary>
        /// The track's track number. If not available, set to 0
        /// </summary>
        public int TrackNumber { get; set; }
        /// <summary>
        /// The length of the track in seconds
        /// </summary>
        public int Length { get; set; }
    }
    /// <summary>
    /// Representation of a playlist in the database
    /// </summary>
    public class OldDatabasePlaylist
    {
        /// <summary>
        /// The ID of the playlist
        /// </summary>
        public int DatabasePlaylistID { get; set; }
        /// <summary>
        /// The playlist's name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// All tracks in the playlist
        /// </summary>
        public List<string> Tracks { get; set; }
    }
}
