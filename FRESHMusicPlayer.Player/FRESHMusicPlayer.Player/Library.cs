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
    /// Wrapper over LiteDB for interacting with the FMP library
    /// </summary>
    public class Library
    {
        /// <summary>
        /// The actual LiteDB connection, for things that can't be done with the methods here
        /// </summary>
        public LiteDatabase Database { get; private set; }

        /// <summary>
        /// Constructs a new library
        /// </summary>
        /// <param name="library">The actual LiteDB connection</param>
        public Library(LiteDatabase library)
        {
            Database = library;
        }
        /// <summary>
        /// Gets all tracks in the library
        /// </summary>
        /// <param name="filter">Property of DatabaseTrack to order by</param>
        /// <returns>All the tracks in the library</returns>
        public List<DatabaseTrack> Read(string filter = "Title") => Database.GetCollection<DatabaseTrack>("tracks").Query().OrderBy(filter).ToList();
        /// <summary>
        /// Gets all tracks for the given artist
        /// </summary>
        /// <param name="artist">The artist to get tracks for</param>
        /// <returns>All the tracks for the artist</returns>
        public List<DatabaseTrack> ReadTracksForArtist(string artist) => Database.GetCollection<DatabaseTrack>("tracks").Query().Where(x => x.Artist == artist).OrderBy("Title").ToList();
        /// <summary>
        /// Gets all tracks for the given album
        /// </summary>
        /// <param name="album">The artist to get tracks for</param>
        /// <returns>All the tracks for the album</returns>
        public List<DatabaseTrack> ReadTracksForAlbum(string album) => Database.GetCollection<DatabaseTrack>("tracks").Query().Where(x => x.Album == album).OrderBy("TrackNumber").ToList();
        /// <summary>
        /// Gets all tracks for the given playlist. This must be Task.Run()'d due to a quirk that will be fixed next major release
        /// </summary>
        /// <param name="playlist">The artist to get tracks for</param>
        /// <returns>All the tracks for the playlist</returns>
        public List<DatabaseTrack> ReadTracksForPlaylist(string playlist)
        {
            var x = Database.GetCollection<DatabasePlaylist>("playlists").FindOne(y => y.Name == playlist);
            var z = new List<DatabaseTrack>();
            foreach (string path in x.Tracks) z.Add(GetFallbackTrack(path));
            return z;
        }
        /// <summary>
        /// Adds a track to a playlist
        /// </summary>
        /// <param name="playlist">The playlist</param>
        /// <param name="path">The file path to add</param>
        public virtual void AddTrackToPlaylist(string playlist, string path)
        {
            var x = Database.GetCollection<DatabasePlaylist>("playlists").FindOne(y => y.Name == playlist);
            if (Database.GetCollection<DatabasePlaylist>("playlists").FindOne(y => y.Name == playlist) is null)
            {
                x = CreatePlaylist(playlist, path);
                x.Tracks.Add(path);
            }
            else
            {
                x.Tracks.Add(path);
                Database.GetCollection<DatabasePlaylist>("playlists").Update(x);
            }
        }
        /// <summary>
        /// Removes a track from a playlist
        /// </summary>
        /// <param name="playlist">The playlist</param>
        /// <param name="path">The file path to remove</param>
        public virtual void RemoveTrackFromPlaylist(string playlist, string path)
        {
            var x = Database.GetCollection<DatabasePlaylist>("playlists").FindOne(y => y.Name == playlist);
            x.Tracks.Remove(path);
            Database.GetCollection<DatabasePlaylist>("playlists").Update(x);
        }
        /// <summary>
        /// Creates a new playlist
        /// </summary>
        /// <param name="playlist">The name of the playlist</param>
        /// <param name="path">An optional track to start the playlist off with</param>
        /// <returns>The created playlist</returns>
        public virtual DatabasePlaylist CreatePlaylist(string playlist, string path = null)
        {
            var newplaylist = new DatabasePlaylist
            {
                Name = playlist,
                Tracks = new List<string>()
            };
            if (Database.GetCollection<DatabasePlaylist>("playlists").Count() == 0) newplaylist.DatabasePlaylistID = 0;
            else newplaylist.DatabasePlaylistID = Database.GetCollection<DatabasePlaylist>("playlists").Query().ToList().Last().DatabasePlaylistID + 1;
            if (path != null) newplaylist.Tracks.Add(path);
            Database.GetCollection<DatabasePlaylist>("playlists").Insert(newplaylist);
            return newplaylist;
        }
        /// <summary>
        /// Deletes a playlist
        /// </summary>
        /// <param name="playlist">The name of the playlist to delete</param>
        public virtual void DeletePlaylist(string playlist) => Database.GetCollection<DatabasePlaylist>("playlists").DeleteMany(x => x.Name == playlist);
        /// <summary>
        /// Imports some tracks to the library
        /// </summary>
        /// <param name="tracks">The file paths to import</param>
        public virtual void Import(string[] tracks)
        {
            var stufftoinsert = new List<DatabaseTrack>();
            int count = 0;
            foreach (string y in tracks)
            {
                var track = new Track(y);
                stufftoinsert.Add(new DatabaseTrack { Title = track.Title, Artist = track.Artist, Album = track.Album, Path = track.Path, TrackNumber = track.TrackNumber, Length = track.Duration });
                count++;
            }
            Database.GetCollection<DatabaseTrack>("tracks").InsertBulk(stufftoinsert);
        }
        /// <summary>
        /// Imports some tracks to the library
        /// </summary>
        /// <param name="tracks">The file paths to import</param>
        public virtual void Import(List<string> tracks)
        {
            var stufftoinsert = new List<DatabaseTrack>();
            foreach (string y in tracks)
            {
                var track = new Track(y);
                stufftoinsert.Add(new DatabaseTrack { Title = track.Title, Artist = track.Artist, Album = track.Album, Path = track.Path, TrackNumber = track.TrackNumber, Length = track.Duration });
            }
            Database.GetCollection<DatabaseTrack>("tracks").InsertBulk(stufftoinsert);
        }
        /// <summary>
        /// Imports a track to the library
        /// </summary>
        /// <param name="path">The file path to import</param>
        public virtual void Import(string path)
        {
            var track = new Track(path);
            Database.GetCollection<DatabaseTrack>("tracks")
                                .Insert(new DatabaseTrack { Title = track.Title, Artist = track.Artist, Album = track.Album, Path = track.Path, TrackNumber = track.TrackNumber, Length = track.Duration });
        }
        /// <summary>
        /// Removes a track from the library
        /// </summary>
        /// <param name="path">The file path to remove</param>
        public virtual void Remove(string path)
        {
            Database.GetCollection<DatabaseTrack>("tracks").DeleteMany(x => x.Path == path);
        }
        /// <summary>
        /// Clears the entire library
        /// </summary>
        /// <param name="nukePlaylists">Whether to also clear playlists</param>
        public virtual void Nuke(bool nukePlaylists = true)
        {
            Database.GetCollection<DatabaseTrack>("tracks").DeleteAll();
            if (nukePlaylists) Database.GetCollection<DatabasePlaylist>("playlists").DeleteAll();
        }
        /// <summary>
        /// Gets a DatabaseTrack for the given file path. Will try getting from the library system first (fast), before
        /// falling back to the audio backend system, then finally the default ATL handling
        /// </summary>
        /// <param name="path">The file path</param>
        /// <returns>The track</returns>
        public DatabaseTrack GetFallbackTrack(string path)
        {
            var dbTrack = Database.GetCollection<DatabaseTrack>("tracks").FindOne(x => path == x.Path);
            if (dbTrack != null) return dbTrack;
            else
            {
                try
                {
                    var backend = AudioBackendFactory.CreateAndLoadBackendAsync(path).Result; // doing async over sync here isn't that great
                    var track = backend.GetMetadataAsync(path).Result;                 // but it's likely that the frontend will task.run this anyway
                    if (track != null) return new DatabaseTrack { Artist = string.Join(", ", track.Artists), Title = track.Title, Album = track.Album, Length = track.Length, Path = path, TrackNumber = track.TrackNumber };
                }
                catch
                {
                    // ignored (for now)
                }
                var atlTrack = new Track(path);
                return new DatabaseTrack { Artist = atlTrack.Artist, Title = atlTrack.Title, Album = atlTrack.Album, TrackNumber = atlTrack.TrackNumber, Length = atlTrack.Duration };
            }
        }
    }
    /// <summary>
    /// Representation of a track in the database
    /// </summary>
    public class DatabaseTrack
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
    public class DatabasePlaylist
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
