using DiscordRPC;
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

        private IAudioBackend currentBackend;
        public IAudioBackend CurrentBackend { get => currentBackend; }

        public bool AvoidNextQueue { get; set; }
        public DiscordRpcClient Client { get; set; }

        public float CurrentVolume { get; set; } = 1;
        public string FilePath { get; set; } = "";
        public bool Playing { get; set; }
        public bool Paused { get; set; }

        public bool RepeatOnce { get; set; } = false;

        public bool Shuffle { get; set; } = false;

        // TODO:  ^^  for the two properties above, check if their setters are ever used 
        public List<string> Queue { get; private set; } = new List<string>();
        public int QueuePosition { get; set; }

        public DateTime LastUpdateCheck { get; set; }


        /// <summary>
        /// Raised whenever a new track is being played.
        /// </summary>
        public event EventHandler SongChanged;
        public event EventHandler SongStopped;
        public event EventHandler<PlaybackExceptionEventArgs> SongException;

        #region CoreFMP

        // Queue System
        /// <summary>
        /// Adds a track to the <see cref="Queue"/>.
        /// </summary>
        /// <param name="filePath">The file path to the track to add.</param>
        public void AddQueue(string filePath)
        {
            Queue.Add(filePath);
        }

        public void AddQueue(string[] filePaths)
        {
            Queue.AddRange(filePaths);
        }

        public void ClearQueue() => Queue.Clear();

        /// <summary>
        /// Skips to the previous track in the Queue. If there are no tracks for the player to go back to, nothing will happen.
        /// </summary>
        public void PreviousSong()
        {
            if (QueuePosition <= 1) return;
            if (Shuffle) Queue = this.ShuffleQueue(Queue);
            QueuePosition -= 2;
            PlayMusic();
        }

        /// <summary>
        /// Skips to the next track in the Queue. If there are no more tracks, the player will stop.
        /// </summary>
        /// <param name="avoidNext">Intended to be used only by the player</param>
        public void NextSong(bool avoidNext = false)
        {
            AvoidNextQueue = avoidNext;
            if (RepeatOnce) QueuePosition--; // Don't advance Queue, play the same thing again
            if (Shuffle) Queue = this.ShuffleQueue(Queue);

            if (QueuePosition >= Queue.Count)
            {
                Queue.Clear();
                QueuePosition = 0;
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
            currentBackend.CurrentTime = TimeSpan.FromSeconds(seconds);
        }

        /// <summary>
        /// Starts playing the Queue. In order to play a track, you must first add it to the Queue using <see cref="AddQueue(string)"/>.
        /// </summary>
        /// <param name="repeat">If true, avoids dequeuing the next track. Not to be used for anything other than the player.</param>
        public void PlayMusic(bool repeat = false)
        {
            if (!repeat && Queue.Count != 0)
                FilePath = Queue[QueuePosition];
            QueuePosition++;

            void PMusic()
            {
                currentBackend = AudioBackendFactory.CreateBackend(FilePath);

                currentBackend.Play();
                currentBackend.Volume = CurrentVolume;
                currentBackend.OnPlaybackStopped += OnPlaybackStopped;

                Playing = true;
            }

            try
            {
                if (Playing != true)
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
            catch (System.IO.FileNotFoundException)
            {
                var args = new PlaybackExceptionEventArgs {Details = "That's not a valid file path!"};
                SongException?.Invoke(null, args);
            }
            catch (ArgumentException)
            {
                var args = new PlaybackExceptionEventArgs {Details = "That's not a valid file path!"};
                SongException?.Invoke(null, args);
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                var args = new PlaybackExceptionEventArgs {Details = "This isn't a valid audio file!"};
                SongException?.Invoke(null, args);
            }
            catch (FormatException)
            {
                var args = new PlaybackExceptionEventArgs {Details = "This audio file might be corrupt!"};
                SongException?.Invoke(null, args);
            }
            catch (InvalidOperationException)
            {
                var args = new PlaybackExceptionEventArgs {Details = "This audio file uses VBR \nor might be corrupt!"};
                SongException?.Invoke(null, args);
            }
            catch (Exception e)
            {
                var args = new PlaybackExceptionEventArgs {Details = $"{e.Message}\n{e.StackTrace}"};
                SongException?.Invoke(null, args);
            }
        }

        /// <summary>
        /// Completely stops and disposes the player and resets all playback related variables to their defaults.
        /// </summary>
        public void StopMusic()
        {
            if (!Playing) return;
                //outputDevice.Dispose();
                //outputDevice = null;
                //AudioFile?.Dispose();
                //AudioFile = null;

                currentBackend.Dispose();
                currentBackend = null;

                Playing = false;
                Paused = false;
                SongStopped?.Invoke(null, EventArgs.Empty);
                //position = 0;
        }

        /// <summary>
        /// Pauses playback without disposing. Can later be resumed with <see cref="ResumeMusic()"/>.
        /// </summary>
        public void PauseMusic()
        {
            if (!Paused) currentBackend?.Pause();
            Paused = true;
        } // Pauses the music without completely disposing it

        /// <summary>
        /// Resumes playback.
        /// </summary>
        public void ResumeMusic()
        {
            if (Paused) currentBackend?.Play();
            //playing = true;
            Paused = false;
        } // Resumes music that has been paused

        /// <summary>
        /// Updates the volume of the player during playback to the value of <see cref="CurrentVolume"/>.
        /// Even if you don't call this, the volume of the player will update whenever the next track plays.
        /// </summary>
        public void UpdateSettings()
        {
            currentBackend.Volume = CurrentVolume;
        }

        // Other Logic Stuff
        /// <summary>
        /// Returns a formatted string of the current playback position.
        /// </summary>
        /// <returns></returns>
        public string SongPositionString() => $"{currentBackend.CurrentTime:c} / {currentBackend.TotalTime:c}";


        #endregion

        // Integration

        #region DiscordRPC
        /// <summary>
        /// Initializes the Discord RPC client. Once it has been initialized, you can set the presence by using <see cref="UpdateRPC(string, string, string)"/>
        /// </summary>
        /// <param name="applicationID">The application ID of your app</param>
        public void InitDiscordRPC(string applicationID)
        { // FMP application ID - 656678380283887626
            Client = new DiscordRpcClient(applicationID);

            Client.OnReady += (sender, e) => { Console.WriteLine("Received Ready from user {0}", e.User.Username); };
            Client.OnPresenceUpdate += (sender, e) => { Console.WriteLine("Received Update! {0}", e.Presence); };
            Client.Initialize();
        }

        public void UpdateRPC(string Activity, string Artist = null, string Title = null)
        {
            Client?.SetPresence(new RichPresence()
                {
                    Details = Title,
                    State = $"by {Artist}",
                    Assets = new Assets()
                    {
                        LargeImageKey = "icon",
                        SmallImageKey = Activity
                    },
                    Timestamps = Timestamps.Now
                }
            );
        }

        public void DisposeRPC() => Client?.Dispose();

        #endregion
    }
}