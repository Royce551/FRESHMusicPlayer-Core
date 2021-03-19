using System;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CDLib;
using FRESHMusicPlayer.Backends;

namespace FmpCdLibBackend
{
    [Export(typeof(IAudioBackend))]
    public class FmpCdLibBackend : IAudioBackend
    {
        private IAudioCDPlayer player;
        public TimeSpan CurrentTime { get => player.CurrentPosition; set => player.Seek(value); }

        public TimeSpan TotalTime { get; private set; }

        public float Volume { get => (float)player.Volume; set => player.Volume = value; }

        public event EventHandler<EventArgs> OnPlaybackStopped;

        public FmpCdLibBackend()
        {
            player = AudioCDPlayer.GetPlayer();
            player.FinishedPlayingTrack += Player_FinishedPlayingTrack;
        }
        //static FmpCdLibBackend()
        //{
          //  Assembly.Load(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FRESHMusicPlayer", "Backends", "CDLib.winmd"));
         //   SetDllDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FRESHMusicPlayer", "Backends"));
        //}

        private void Player_FinishedPlayingTrack()
        {
            OnPlaybackStopped.Invoke(null, EventArgs.Empty);
        }

        public void Dispose()
        {
            player.Pause();
           //player.Dispose();
        }

        public void LoadSong(string file)
        {
            if (Path.GetExtension(file).ToUpper() != ".CDA") throw new Exception("Not a CD");
            // super hacky; assumes that the path is something like D:\Track01.cda, might be a better way to do this
            var driveLetter = char.Parse(file.Substring(0, 1));
            var trackNumber = int.Parse(file.Substring(8, 2));

            var drives = player.GetDrives();
            foreach (var drive in drives)
            {
                if (drive.DriveLetter == driveLetter)
                {
                    var trackToPlay = drive.InsertedMedia.Tracks[trackNumber - 1];
                    TotalTime = trackToPlay.Duration;
                    player.PlayTrack(trackToPlay);
                    return;
                }
            }
        }
        
        public void Pause()
        {
            player.Pause();
        }

        public void Play()
        {
            player.Resume();
        }
        [DllImport("kernel32")]
        private static extern void SetDllDirectory(string pathName);
    }
}
