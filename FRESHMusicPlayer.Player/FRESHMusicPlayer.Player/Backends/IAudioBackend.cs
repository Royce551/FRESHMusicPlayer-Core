using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRESHMusicPlayer.Backends
{
    interface IAudioBackend : IDisposable
    {
        void Play();
        void Pause();

        TimeSpan CurrentTime { get; set; }
        TimeSpan TotalTime { get; }

        float Volume { get; set; }

        event EventHandler<EventArgs> OnPlaybackStopped;
    }
}
