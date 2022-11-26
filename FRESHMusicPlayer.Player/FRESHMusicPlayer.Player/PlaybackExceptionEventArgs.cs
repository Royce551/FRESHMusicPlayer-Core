using FRESHMusicPlayer.Backends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRESHMusicPlayer
{
    /// <summary>
    /// Playback exception args
    /// </summary>
    public class PlaybackExceptionEventArgs : EventArgs
    {
        /// <summary>
        /// The actual exception
        /// </summary>
        public Dictionary<string, Exception> Exceptions { get; }
        
        public Dictionary<string, BackendLoadResult> Problems { get; }

        /// <summary>
        /// Constructs new playback exception args
        /// </summary>
        /// <param name="exception">The actual exception</param>
        /// <param name="details">A nicely formatted version of the exception for display purposes</param>
        public PlaybackExceptionEventArgs(Dictionary<string, Exception> exceptions, Dictionary<string, BackendLoadResult> problems)
        {
            Exceptions = exceptions;
            Problems = problems;
        }
    }
}
