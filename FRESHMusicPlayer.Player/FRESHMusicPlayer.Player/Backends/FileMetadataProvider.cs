using ATL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FRESHMusicPlayer.Backends
{
    /// <summary>
    /// A metadata provider that uses ATL to get metadata for most audio formats
    /// </summary>
    public class FileMetadataProvider : IMetadataProvider
    {
        /// <inheritdoc/>
        public string Title => ATLTrack.Title;
                                                               // for mp3s, ATL still uses /
        /// <inheritdoc/>
        public string[] Artists => ATLTrack.Artist.Split(Settings.DisplayValueSeparator, '/');

        /// <inheritdoc/>
        public string Album => ATLTrack.Album;

        /// <inheritdoc/>
        public byte[] CoverArt
        { 
            get
            {
                if (ATLTrack.EmbeddedPictures.Count != 0) return ATLTrack.EmbeddedPictures[0].PictureData;

                if (!File.Exists(path)) return null;
                else
                {
                    foreach (var file in Directory.EnumerateFiles(Path.GetDirectoryName(path)))
                    {
                        if (Path.GetFileNameWithoutExtension(file).ToUpper() == "COVER" ||
                            Path.GetFileNameWithoutExtension(file).ToUpper() == "ARTWORK" ||
                            Path.GetFileNameWithoutExtension(file).ToUpper() == "FRONT" ||
                            Path.GetFileNameWithoutExtension(file).ToUpper() == "BACK" ||
                            Path.GetFileNameWithoutExtension(file).ToUpper() == "JACKET" ||
                            Path.GetFileNameWithoutExtension(file).ToUpper() == path)
                        {
                            if (Path.GetExtension(file) == ".png" || Path.GetExtension(file) == ".jpg" || Path.GetExtension(file) == ".jpeg")
                                return File.ReadAllBytes(file);
                        }
                    }
                    return null;
                }
            }
        }

        /// <inheritdoc/>
        public string[] Genres => ATLTrack.Genre.Split(Settings.DisplayValueSeparator, '/');

        /// <inheritdoc/>
        public int Year => ATLTrack.Year;

        /// <inheritdoc/>
        public int TrackNumber => ATLTrack.TrackNumber;

        /// <inheritdoc/>
        public int TrackTotal => ATLTrack.TrackTotal;

        /// <inheritdoc/>
        public int DiscNumber => ATLTrack.DiscNumber;

        /// <inheritdoc/>
        public int DiscTotal => ATLTrack.DiscTotal;

        /// <inheritdoc/>
        public int Length => ATLTrack.Duration;

        /// <inheritdoc/>
        public Track ATLTrack { get; set; }

        private string path;

        /// <summary>
        /// Gets metadata for the supplied file path
        /// </summary>
        /// <param name="path">The file path to query metadata for</param>
        public FileMetadataProvider(string path)
        {
            this.path = path;
            ATLTrack = new Track(path);
        }
    }
}
