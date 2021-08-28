using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRESHMusicPlayer
{
    public class PlaybackExceptionEventArgs : EventArgs
    {
        public Exception Exception { get; }
        public string Details { get; }

        public PlaybackExceptionEventArgs(Exception exception, string details)
        {
            Exception = exception;
            Details = details;
        }
    }
}
