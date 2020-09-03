using System.Collections.Generic;

namespace FRESHMusicPlayer.Utilities
{
    public class DatabaseFormat
    {
        public int Version { get; set; }
        public List<string> Songs { get; set; }
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
}