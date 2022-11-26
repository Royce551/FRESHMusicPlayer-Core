using System;
using System.Collections.Generic;
using System.Text;

namespace FRESHMusicPlayer
{
    public class PlaybackStoppedEventArgs : EventArgs
    {
        public bool IsEndOfPlayback { get; }

        public PlaybackStoppedEventArgs(bool isEndOfPlayback)
        {
            IsEndOfPlayback = isEndOfPlayback;
        }
    }
}
