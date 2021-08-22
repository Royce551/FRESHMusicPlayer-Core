using ATL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FRESHMusicPlayer.Backends
{
    public class FileMetadataProvider : IMetadataProvider
    {
        public string Title => ATLTrack.Title;
                                                                                                    // for mp3s, ATL still uses /
        public string[] Artists => ATLTrack.Artist.Split(new char[] { Settings.DisplayValueSeparator, '/' });

        public string Album => ATLTrack.Album;

        public byte[] CoverArt => ATLTrack.EmbeddedPictures[0].PictureData;

        public string[] Genres => ATLTrack.Genre.Split(new char[] { Settings.DisplayValueSeparator, '/' });

        public int Year => ATLTrack.Year;

        public int TrackNumber => ATLTrack.TrackNumber;

        public int TrackTotal => ATLTrack.TrackTotal;

        public int DiscNumber => ATLTrack.DiscNumber;

        public int DiscTotal => ATLTrack.DiscTotal;

        public Track ATLTrack { get; set; }

        private string path;
        public FileMetadataProvider(string path) => this.path = path;

        public async Task LoadAsync() => await Task.Run(() => ATLTrack = new Track(path));
    }
}
