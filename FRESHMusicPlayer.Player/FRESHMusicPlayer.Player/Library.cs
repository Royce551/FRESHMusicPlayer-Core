using ATL;
using FRESHMusicPlayer.Backends;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member temp

namespace FRESHMusicPlayer
{
    public class Library
    {
        public LiteDatabase Database { get; private set; }

        public const string TracksCollectionName = "Tracks";
        public const string PlaylistsCollectionName = "Playlists";

        public Library(LiteDatabase database)
        {
            Database = database;
        }

        public virtual List<DatabaseTrack> GetAllTracks(string filter = "Title") => Database.GetCollection<DatabaseTrack>(TracksCollectionName).Query().OrderBy(filter).ToList();
        public virtual List<DatabaseTrack> GetTracksForArtist(string artist) => Database.GetCollection<DatabaseTrack>(TracksCollectionName).Query().Where(x => x.Artists.Contains(artist)).OrderBy("Title").ToList();
        public virtual List<DatabaseTrack> GetTracksForAlbum(string album) => Database.GetCollection<DatabaseTrack>(TracksCollectionName).Query().Where(x => x.Album == album).OrderBy("TrackNumber").ToList();

        public virtual List<DatabaseTrack> GetTracksForPlaylist(string playlist)
        {
            var dbPlaylist = Database.GetCollection<DatabasePlaylist>(PlaylistsCollectionName).FindOne(x => x.Name == playlist);
            return Database.GetCollection<DatabaseTrack>(TracksCollectionName).Query().Where(x => dbPlaylist.Tracks.Contains(x.Id)).ToList();
        }

        public virtual async Task AddTrackToPlaylistAsync(string playlist, string path, bool isSystemPlaylist = false)
        {
            var dbPlaylist = Database.GetCollection<DatabasePlaylist>(PlaylistsCollectionName).FindOne(x => x.Name == playlist);
            var track = Database.GetCollection<DatabaseTrack>(TracksCollectionName).FindOne(x => x.Path == path);
            if (track is null)
            {
                await ImportAsync(path);
                track = Database.GetCollection<DatabaseTrack>(TracksCollectionName).FindOne(x => x.Path == path);
            }

            if (dbPlaylist is null)
                dbPlaylist = await CreatePlaylistAsync(playlist, isSystemPlaylist, path);

            dbPlaylist.Tracks.Add(track.Id);
            Database.GetCollection<DatabasePlaylist>(PlaylistsCollectionName).Update(dbPlaylist);
        }

        public virtual void RemoveTrackFromPlaylist(string playlist, string path)
        {
            var dbPlaylist = Database.GetCollection<DatabasePlaylist>(PlaylistsCollectionName).FindOne(x => x.Name == playlist);
            var track = Database.GetCollection<DatabaseTrack>(TracksCollectionName).FindOne(x => x.Path == path);
            dbPlaylist.Tracks.Remove(track.Id);
            Database.GetCollection<DatabasePlaylist>(PlaylistsCollectionName).Update(dbPlaylist);
        }

        public virtual async Task<DatabasePlaylist> CreatePlaylistAsync(string playlist, bool isSystemPlaylist, string path = null)
        {
            var newPlaylist = new DatabasePlaylist
            {
                Name = playlist,
                Tracks = new List<int>(),
                IsSystemPlaylist = isSystemPlaylist
            };

            Database.GetCollection<DatabasePlaylist>(PlaylistsCollectionName).Insert(newPlaylist);
            if (path != null) await AddTrackToPlaylistAsync(playlist, path);
            return newPlaylist;
        }

        public virtual void DeletePlaylist(string playlist) => Database.GetCollection<DatabasePlaylist>(PlaylistsCollectionName).DeleteMany(x => x.Name == playlist);

        public virtual async Task ImportAsync(string[] tracks)
        {
            var tracksToImport = new List<DatabaseTrack>();
            
            foreach (var track in tracks)
            {
                tracksToImport.Add(new DatabaseTrack { Path = track, HasBeenProcessed = false, Title = track });
            }
            if (tracksToImport is null)
            {
                Console.WriteLine("???");
            }
            Database.GetCollection<DatabaseTrack>(TracksCollectionName).InsertBulk(tracksToImport);
            _ = ProcessDatabaseMetadataAsync();
        }

        public virtual async Task ImportAsync(List<string> tracks) => await ImportAsync(tracks.ToArray());
        public virtual async Task ImportAsync(string track) => await ImportAsync(new string[] { track });

        public virtual async Task<List<DatabaseTrack>> ProcessDatabaseMetadataAsync(Action<int> progress = null)
        {
            var tracksToProcess = Database.GetCollection<DatabaseTrack>(TracksCollectionName).Query().Where(x => !x.HasBeenProcessed).ToList();
            var remainingTracksToProcess = tracksToProcess.Count;
            foreach (var track in tracksToProcess)
            {
                try
                {
                    var backend = await AudioBackendFactory.CreateAndLoadBackendAsync(track.Path);
                    var track2 = await backend.backend?.GetMetadataAsync(track.Path);

                    IMetadataProvider metadata;

                    if (backend.backend != null)
                    {
                        metadata = await backend.backend.GetMetadataAsync(track.Path);
                    }
                    else
                    {
                        metadata = new FileMetadataProvider(track.Path);
                    }
                    track.UpdateFieldsFrom(metadata);
                    track.HasBeenProcessed = true;
                    if (!Database.GetCollection<DatabaseTrack>(TracksCollectionName).Update(track)) throw new Exception("Fueh?!?!?!");

                    backend.backend?.Dispose();  
                }
                catch
                {
                    // ignored for now
                }

                remainingTracksToProcess--;
                progress?.Invoke(remainingTracksToProcess);
            }
            return tracksToProcess;
        }

        public virtual void Update(string track, IMetadataProvider newMetadata)
        {
            var dbTrack = Database.GetCollection<DatabaseTrack>(TracksCollectionName).FindOne(x => x.Path == track);
            dbTrack.UpdateFieldsFrom(newMetadata);

            if (!Database.GetCollection<DatabaseTrack>(TracksCollectionName).Update(dbTrack)) throw new Exception("Fueh?!?!?!");
        }

        public virtual void Remove(string path) => Database.GetCollection<DatabaseTrack>(TracksCollectionName).DeleteMany(x => x.Path == path);

        public virtual void Nuke(bool alsoNukePlaylists = true)
        {
            Database.GetCollection<DatabaseTrack>(TracksCollectionName).DeleteAll();
            if (alsoNukePlaylists) Database.GetCollection<DatabasePlaylist>(PlaylistsCollectionName).DeleteAll();
        }

        public async Task<DatabaseTrack> GetFallbackTrackAsync(string path)
        {
            var dbTrack = Database.GetCollection<DatabaseTrack>(TracksCollectionName).FindOne(x => path == x.Path);
            if (dbTrack != null) return dbTrack;
            else
            {
                try
                {
                    var backend = await AudioBackendFactory.CreateAndLoadBackendAsync(path);
                    var track = await backend.backend?.GetMetadataAsync(path);

                    backend.backend?.Dispose();

                    if (track != null) return new DatabaseTrack(path, track, true);
                }
                catch
                {
                    // ignored (for now)
                }
                return new DatabaseTrack(path, new FileMetadataProvider(path), true);
            }
        } 
    }

    public class DatabaseTrack
    {
        public int Id { get; set; }

        public string Path { get; set; }

        public bool HasBeenProcessed { get; set; }

        public string Title { get; set; } = string.Empty;
        public string[] Artists { get; set; } = new string[0];
        public string Album { get; set; } = string.Empty;
        public string[] Genres { get; set; } = new string[0];
        public int Year { get; set; }
        public int TrackNumber { get; set; }
        public int TrackTotal { get; set; }
        public int DiscNumber { get; set; }
        public int DiscTotal { get; set; }
        public int Length { get; set; }

        public DatabaseTrack()
        {

        }

        public DatabaseTrack(string path, IMetadataProvider metadata, bool hasBeenProcessed)
        {
            Path = path;
            HasBeenProcessed = hasBeenProcessed;
            UpdateFieldsFrom(metadata);
        }
        public DatabaseTrack(string path, DatabaseTrack track, bool hasBeenProcessed)
        {
            Path = path;
            HasBeenProcessed = hasBeenProcessed;
            UpdateFieldsFrom(track);
        }

        public void UpdateFieldsFrom(IMetadataProvider metadata)
        {
            Title = metadata.Title;
            Artists = metadata.Artists;
            Album = metadata.Album;
            Genres = metadata.Genres;
            Year = metadata.Year;
            TrackNumber = metadata.TrackNumber;
            TrackTotal = metadata.TrackTotal;
            DiscNumber = metadata.DiscNumber;
            DiscTotal = metadata.DiscTotal;
            Length = metadata.Length;
        }

        public void UpdateFieldsFrom(DatabaseTrack track)
        {
            Title = track.Title;
            Artists = track.Artists;
            Album = track.Album;
            Genres = track.Genres;
            Year = track.Year;
            TrackNumber = track.TrackNumber;
            TrackTotal = track.TrackTotal;
            DiscNumber = track.DiscNumber;
            DiscTotal = track.DiscTotal;
            Length = track.Length;
        }
    }

    public class DatabasePlaylist
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsSystemPlaylist { get; set; }

        public List<int> Tracks { get; set; }
    }
}
#pragma warning restore CS1591
