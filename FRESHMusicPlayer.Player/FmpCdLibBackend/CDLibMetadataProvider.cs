using CDLib;
using FRESHMusicPlayer.Backends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FmpCdLibBackend
{
    public class CDLibMetadataProvider : IMetadataProvider
    {
        public string Title => track.Title;

        public string[] Artists => new string[] { track.Artist };

        public string Album => track.AlbumTitle;

        public byte[] CoverArt => null;

        public string[] Genres => Array.Empty<string>();

        public int Year => 0;

        public int TrackNumber => (int)track.TrackNumber;

        public int TrackTotal => 0;

        public int DiscNumber => 0;

        public int DiscTotal => 0;

        private readonly IAudioCDTrack track;
        public CDLibMetadataProvider(IAudioCDTrack track) => this.track = track;
    }
}
