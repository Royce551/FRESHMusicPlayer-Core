﻿using System;
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

        private bool hasPlaybackStarted = false;
        private IAudioCDTrack trackToPlay;

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

        public async Task<BackendLoadResult> LoadSongAsync(string file)
        {
            if (Path.GetExtension(file).ToUpper() != ".CDA") return BackendLoadResult.NotSupported;

            var result = BackendLoadResult.Invalid;

            // super hacky; assumes that the path is something like D:\Track01.cda, might be a better way to do this
            var driveLetter = char.Parse(file.Substring(0, 1));
            var trackNumber = int.Parse(file.Substring(8, 2));

            var drives = player.GetDrives();
            foreach (var drive in drives)
            {
                if (drive.DriveLetter == driveLetter)
                {
                    trackToPlay = drive.InsertedMedia.Tracks[trackNumber - 1];
                    TotalTime = trackToPlay.Duration;
                        
                    result = BackendLoadResult.OK;
                }
            }
            return result;
        }
        
        public async Task<IMetadataProvider> GetMetadataAsync(string file)
        {
            if (Path.GetExtension(file).ToUpper() != ".CDA") return null;

            IAudioCDTrack trackToPlay = null;
            // super hacky; assumes that the path is something like D:\Track01.cda, might be a better way to do this
            var driveLetter = char.Parse(file.Substring(0, 1));
            var trackNumber = int.Parse(file.Substring(8, 2));

            var drives = player.GetDrives();
            foreach (var drive in drives)
            {
                if (drive.DriveLetter == driveLetter) trackToPlay = drive.InsertedMedia.Tracks[trackNumber - 1];
            }
            return new CDLibMetadataProvider(trackToPlay);
        }

        public void Pause()
        {
            player.Pause();
        }

        public void Play()
        {
            if (!hasPlaybackStarted)
            {
                player.PlayTrack(trackToPlay);
                hasPlaybackStarted = true;
            }
            player.Resume();
        }
        [DllImport("kernel32")]
        private static extern void SetDllDirectory(string pathName);
    }
}
