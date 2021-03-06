﻿using ATL;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FRESHMusicPlayer.Handlers
{
    public class Library
    {
        public LiteDatabase Database { get; private set; }

        public Library(LiteDatabase library)
        {
            Database = library;
        }
        public List<DatabaseTrack> Read(string filter = "Title") => Database.GetCollection<DatabaseTrack>("tracks").Query().OrderBy(filter).ToList();
        public List<DatabaseTrack> ReadTracksForArtist(string artist) => Database.GetCollection<DatabaseTrack>("tracks").Query().Where(x => x.Artist == artist).OrderBy("Title").ToList();
        public List<DatabaseTrack> ReadTracksForAlbum(string album) => Database.GetCollection<DatabaseTrack>("tracks").Query().Where(x => x.Album == album).OrderBy("TrackNumber").ToList();
        public List<DatabaseTrack> ReadTracksForPlaylist(string playlist)
        {
            var x = Database.GetCollection<DatabasePlaylist>("playlists").FindOne(y => y.Name == playlist);
            var z = new List<DatabaseTrack>();
            foreach (string path in x.Tracks) z.Add(GetFallbackTrack(path));
            return z;
        }
        public void AddTrackToPlaylist(string playlist, string path)
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
        public void RemoveTrackFromPlaylist(string playlist, string path)
        {
            var x = Database.GetCollection<DatabasePlaylist>("playlists").FindOne(y => y.Name == playlist);
            x.Tracks.Remove(path);
            Database.GetCollection<DatabasePlaylist>("playlists").Update(x);
        }
        public DatabasePlaylist CreatePlaylist(string playlist, string path = null)
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
        public void DeletePlaylist(string playlist) => Database.GetCollection<DatabasePlaylist>("playlists").DeleteMany(x => x.Name == playlist);
        public void Import(string[] tracks)
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
        public void Import(List<string> tracks)
        {
            var stufftoinsert = new List<DatabaseTrack>();
            foreach (string y in tracks)
            {
                var track = new Track(y);
                stufftoinsert.Add(new DatabaseTrack { Title = track.Title, Artist = track.Artist, Album = track.Album, Path = track.Path, TrackNumber = track.TrackNumber, Length = track.Duration });
            }
            Database.GetCollection<DatabaseTrack>("tracks").InsertBulk(stufftoinsert);
        }
        public void Import(string path)
        {
            var track = new Track(path);
            Database.GetCollection<DatabaseTrack>("tracks")
                                .Insert(new DatabaseTrack { Title = track.Title, Artist = track.Artist, Album = track.Album, Path = track.Path, TrackNumber = track.TrackNumber, Length = track.Duration });
        }
        public void Remove(string path)
        {
            Database.GetCollection<DatabaseTrack>("tracks").DeleteMany(x => x.Path == path);
        }
        public virtual void Nuke(bool nukePlaylists = true)
        {
            Database.GetCollection<DatabaseTrack>("tracks").DeleteAll();
            if (nukePlaylists) Database.GetCollection<DatabasePlaylist>("playlists").DeleteAll();
        }
        public DatabaseTrack GetFallbackTrack(string path)
        {
            var dbTrack = Database.GetCollection<DatabaseTrack>("tracks").FindOne(x => path == x.Path);
            if (dbTrack != null) return dbTrack;
            else
            {
                var track = new Track(path);
                return new DatabaseTrack { Artist = track.Artist, Title = track.Title, Album = track.Album, Length = track.Duration, Path = path, TrackNumber = track.TrackNumber };
            }
        }
    }
    public class DatabaseTrack
    {
        public string Path { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public int TrackNumber { get; set; }
        public int Length { get; set; }
    }
    public class DatabasePlaylist
    {
        public int DatabasePlaylistID { get; set; }
        public string Name { get; set; }
        public List<string> Tracks { get; set; }
    }
}
