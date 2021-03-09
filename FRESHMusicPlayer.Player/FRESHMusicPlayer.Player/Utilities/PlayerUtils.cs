using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FRESHMusicPlayer;
namespace FRESHMusicPlayer.Utilities
{
    static class PlayerUtils
    {
        private static readonly Random rng = new Random();

        public static string TruncateBytes(string str, int bytes)
        {
            if (Encoding.UTF8.GetByteCount(str) <= bytes) return str;
            int i = 0;
            while (true)
            {
                if (Encoding.UTF8.GetByteCount(str.Substring(0, i)) > bytes) return str.Substring(0, i);
                i++;
            }
        }
    }
}
