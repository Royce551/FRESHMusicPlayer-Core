using System;
using System.Collections.Generic;
using System.Text;

namespace FRESHMusicPlayer.Backends
{
    public interface ISupportEqualization
    {
        List<EqualizerBand> EqualizerBands { get; set; }

        void UpdateEqualizer();
    }

    public class EqualizerBand
    {
        public float Frequency { get; set; }

        public float Gain { get; set; }

        public float Bandwidth { get; set; }
    }
}
