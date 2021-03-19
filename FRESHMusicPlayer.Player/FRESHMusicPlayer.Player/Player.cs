using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FRESHMusicPlayer.Handlers;
using FRESHMusicPlayer.Utilities;
using FRESHMusicPlayer.Backends;

namespace FRESHMusicPlayer
{
    public class Player
    {
        /// <summary>
        /// The current backend the Player is using for audio playback
        /// </summary>
        public IAudioBackend CurrentBackend { get; private set; }
        /// <summary>
        /// The current playback position./>
        /// </summary>
        public TimeSpan CurrentTime { get => CurrentBackend.CurrentTime; set => CurrentBackend.CurrentTime = value; }
        /// <summary>
        /// The total length of the current track.
        /// </summary>
        public TimeSpan TotalTime { get => CurrentBackend.TotalTime; }
        /// <summary>
        /// If true, suppresses an internal event handler. I honestly don't understand how this thing works; just make sure to keep
        /// setting this to false, or things will explode.
        /// </summary>
        public bool AvoidNextQueue { get; set; }

        private float volume = 1f;
        /// <summary>
        /// The current volume, from 0 to 1.
        /// </summary>
        public float Volume
        {
            get => volume;
            set
            {
                volume = value;
                if (FileLoaded)
                    CurrentBackend.Volume = volume;
            }
        }
        /// <summary>
        /// The current path the Player is playing. Keep in mind that this may not necessarily be a file. For example, it could be the
        /// URL to a network stream.
        /// </summary>
        public string FilePath { get; private set; } = string.Empty;
        /// <summary>
        /// Whether the audio backend and file has been loaded and things are ready to go. If you interact with the Player everything
        /// will explode.
        /// </summary>
        public bool FileLoaded { get; set; }
        /// <summary>
        /// Whether the Player is paused.
        /// </summary>
        public bool Paused { get; set; }

        public PlayQueue Queue { get; set; } = new PlayQueue();

        /// <summary>
        /// Raised whenever a new track is being played.
        /// </summary>
        public event EventHandler SongChanged;
        /// <summary>
        /// Raised whenever the player is stopping.
        /// </summary>
        public event EventHandler SongStopped;
        /// <summary>
        /// Raised whenever an exception is thrown while the Player is loading a file.
        /// </summary>
        public event EventHandler<PlaybackExceptionEventArgs> SongException;

        #region CoreFMP

        /// <summary>
        /// Skips to the previous track in the Queue. If there are no tracks for the player to go back to, nothing will happen.
        /// </summary>
        public void PreviousSong()
        {
            if (Queue.Position <= 1) return;
            Queue.Position -= 2;
            PlayMusic();
        }

        /// <summary>
        /// Skips to the next track in the Queue. If there are no more tracks, the player will stop.
        /// </summary>
        /// <param name="avoidNext">Intended to be used only by the player</param>
        public void NextSong(bool avoidNext = false)
        {
            AvoidNextQueue = avoidNext;
            if (Queue.RepeatMode == RepeatMode.RepeatOne) Queue.Position--; // Don't advance Queue, play the same thing again

            if (Queue.Position >= Queue.Queue.Count)
            {
                if (Queue.RepeatMode == RepeatMode.RepeatAll) // Go back to the first track and play it again
                {
                    Queue.Position = 0;
                    PlayMusic();
                    return;
                }
                Queue.Clear();
                StopMusic();
                return;
            }
            PlayMusic();
        }

        // Music Playing Controls
        private void OnPlaybackStopped(object sender, EventArgs args)
        {
            if (!AvoidNextQueue) 
                NextSong();
            else
                AvoidNextQueue = false;
        }

        /// <summary>
        /// Repositions the playback position of the player.
        /// </summary>
        /// <param name="seconds">The position in to the track to skip in, in seconds.</param>
        public void RepositionMusic(int seconds)
        {
            CurrentBackend.CurrentTime = TimeSpan.FromSeconds(seconds);
        }

        /// <summary>
        /// Plays a track. This is equivalent to calling Queue.Add() and then PlayMusic()./>
        /// </summary>
        /// <param name="path">The track to play</param>
        public void PlayMusic(string path)
        {
            Queue.Add(path);
            PlayMusic();
        }
        /// <summary>
        /// Starts playing the Queue. In order to play a track, you must first add it to the Queue using <see cref="AddQueue(string)"/>.
        /// </summary>
        /// <param name="repeat">If true, avoids dequeuing the next track. Not to be used for anything other than the player.</param>
        public void PlayMusic(bool repeat = false)
        {
            if (!repeat && Queue.Queue.Count != 0)
                FilePath = Queue.Queue[Queue.Position];
            Queue.Position++;
            void PMusic()
            {
                CurrentBackend = AudioBackendFactory.CreateBackend(FilePath);

                CurrentBackend.Play();
                CurrentBackend.Volume = Volume;
                CurrentBackend.OnPlaybackStopped += OnPlaybackStopped;

                FileLoaded = true;
            }

            try
            {
                if (FileLoaded != true)
                {
                    PMusic();
                }
                else
                {
                    AvoidNextQueue = true;
                    StopMusic();
                    PMusic();
                }

                SongChanged?.Invoke(null,
                    EventArgs.Empty); // Now that playback has started without any issues, fire the song changed event.
            }
            //catch (FileNotFoundException) // TODO: move these to NAudioBackend
            //{
            //    var args = new PlaybackExceptionEventArgs {Details = "That's not a valid file path!"};
            //    SongException?.Invoke(null, args);
            //}
            //catch (ArgumentException)
            //{
            //    var args = new PlaybackExceptionEventArgs {Details = "That's not a valid file path!"};
            //    SongException?.Invoke(null, args);
            //}
            //catch (System.Runtime.InteropServices.COMException)
            //{
            //    var args = new PlaybackExceptionEventArgs {Details = "This isn't a valid audio file!"};
            //    SongException?.Invoke(null, args);
            //}
            //catch (FormatException)
            //{
            //    var args = new PlaybackExceptionEventArgs {Details = "This audio file might be corrupt!"};
            //    SongException?.Invoke(null, args);
            //}
            //catch (InvalidOperationException)
            //{
            //    var args = new PlaybackExceptionEventArgs {Details = "This audio file uses VBR \nor might be corrupt!"};
            //    SongException?.Invoke(null, args);
            //}
            catch (Exception e)
            {
                var args = new PlaybackExceptionEventArgs(e, $"{e.Message}\n{e.StackTrace}");
                SongException?.Invoke(null, args);
            }
        }

        /// <summary>
        /// Completely stops and disposes the player and resets all playback related variables to their defaults.
        /// </summary>
        public void StopMusic()
        {
            if (!FileLoaded) return;

            CurrentBackend.Dispose();
            CurrentBackend = null;

            FileLoaded = false;
            Paused = false;
            SongStopped?.Invoke(null, EventArgs.Empty);
        }

        /// <summary>
        /// Pauses playback without disposing. Can later be resumed with <see cref="ResumeMusic()"/>.
        /// </summary>
        public void PauseMusic()
        {
            if (!Paused) 
                CurrentBackend?.Pause();
            Paused = true;
        }

        /// <summary>
        /// Resumes playback.
        /// </summary>
        public void ResumeMusic()
        {
            if (Paused) 
                CurrentBackend?.Play();
            Paused = false;
        }

        #endregion

        // Integration

        //#region DiscordRPC // TODO: move this to the frontend
        ///// <summary>
        ///// Initializes the Discord RPC client. Once it has been initialized, you can set the presence by using <see cref="UpdateRPC(string, string, string)"/>
        ///// </summary>
        ///// <param name="applicationID">The application ID of your app</param>
        //public void InitDiscordRPC(string applicationID)
        //{ // FMP application ID - 656678380283887626
        //    Client = new DiscordRpcClient(applicationID);

        //    Client.OnReady += (sender, e) => { Console.WriteLine("Received Ready from user {0}", e.User.Username); };
        //    Client.OnPresenceUpdate += (sender, e) => { Console.WriteLine("Received Update! {0}", e.Presence); };
        //    Client.Initialize();
        //}

        //public void UpdateRPC(string Activity, string Artist = null, string Title = null)
        //{
        //    Client?.SetPresence(new RichPresence()
        //    {
        //        Details = PlayerUtils.TruncateBytes(Title, 120),
        //        State = PlayerUtils.TruncateBytes(Artist, 120),
        //        Assets = new Assets()
        //        {
        //            LargeImageKey = "icon",
        //            SmallImageKey = Activity
        //        },
        //        Timestamps = Timestamps.Now
        //    }
        //    );
        //}

        //public void DisposeRPC() => Client?.Dispose();

        //#endregion
    }
}