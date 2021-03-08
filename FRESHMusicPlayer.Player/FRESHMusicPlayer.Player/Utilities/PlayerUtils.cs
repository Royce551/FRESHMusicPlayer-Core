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

        public static List<string> ShuffleQueue(this Player player, List<string> list)
        {
            var listtosort = new List<string>();
            var listtoreinsert = new List<string>();
            var number = 0;  
            foreach (var x in list)
            {
                if (player.QueuePosition < number) listtosort.Add(x);
                else listtoreinsert.Add(x);
                number++;
            }
            
            var n = listtosort.Count;
            while (n > 1)
            {
                n--;
                var k = rng.Next(n + 1);
                var value = listtosort[k];
                listtosort[k] = listtosort[n];
                listtosort[n] = value;
            }
            foreach (var x in listtosort) listtoreinsert.Add(x);
            return listtoreinsert;
        }
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
