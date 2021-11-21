using ATL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FRESHMusicPlayer.Backends
{
    public class FileMetadataProvider : IMetadataProvider
    {
        public string Title => ATLTrack.Title;
        // for mp3s, ATL still uses /
        public string[] Artists => ATLTrack.Artist.Split(Settings.DisplayValueSeparator, '/');

        public string Album => ATLTrack.Album;

        public byte[] CoverArt
        {
            get
            {
                if (ATLTrack.EmbeddedPictures.Count != 0) return ATLTrack.EmbeddedPictures[0].PictureData;
                else // no embedded cover art? fall back to getting it from the directory the audio file is in
                {
                    if (File.Exists(path))
                    {
                        var currentDirectory = Path.GetDirectoryName(path);
                        foreach (var file in Directory.EnumerateFiles(currentDirectory))
                        {
                            if (Path.GetFileNameWithoutExtension(file).ToUpper() == "COVER" ||
                                Path.GetFileNameWithoutExtension(file).ToUpper() == "ARTWORK" ||
                                Path.GetFileNameWithoutExtension(file).ToUpper() == "FRONT" ||
                                Path.GetFileNameWithoutExtension(file).ToUpper() == "BACK" ||
                                Path.GetFileNameWithoutExtension(file).ToUpper() == path)
                            {
                                if (Path.GetExtension(file) == ".png" || Path.GetExtension(file) == ".jpg" || Path.GetExtension(file) == ".jpeg")
                                {
                                    return File.ReadAllBytes(file);
                                }
                            }
                        }
                    }
                    return null;
                }
            }

        }

        public string[] Genres => ATLTrack.Genre.Split(Settings.DisplayValueSeparator, '/');

        public int Year => ATLTrack.Year;

        public int TrackNumber => ATLTrack.TrackNumber;

        public int TrackTotal => ATLTrack.TrackTotal;

        public int DiscNumber => ATLTrack.DiscNumber;

        public int DiscTotal => ATLTrack.DiscTotal;

        public Track ATLTrack { get; set; }

        private string path;
        public FileMetadataProvider(string path)
        {
            this.path = path;
            ATLTrack = new Track(path);
        }
    }
}
