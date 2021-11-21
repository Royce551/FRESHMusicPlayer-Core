using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FRESHMusicPlayer.Backends;
using System.Threading.Tasks;

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

        public bool IsLoading { get; private set; } = false;
        /// <summary>
        /// The current path the Player is playing. Keep in mind that this may not necessarily be a file. For example, it could be the
        /// URL to a network stream.
        /// </summary>
        public string FilePath { get; private set; } = string.Empty;
        /// <summary>
        /// Whether the audio backend and file has been loaded and things are ready to go. If you interact with the Player when this is false everything
        /// will explode.
        /// </summary>
        public bool FileLoaded { get; set; }
        /// <summary>
        /// Whether the Player is paused.
        /// </summary>
        public bool Paused { get; set; }

        public PlayQueue Queue { get; set; } = new PlayQueue();

        public event EventHandler SongLoading;
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
        public async Task PreviousAsync()
        {
            if (Queue.Position <= 1) return;
            Queue.Position -= 2;
            await PlayAsync();
        }

        /// <summary>
        /// Skips to the next track in the Queue. If there are no more tracks, the player will stop.
        /// </summary>
        /// <param name="avoidNext">Intended to be used only by the player</param>
        public async Task NextAsync(bool avoidNext = false)
        {
            AvoidNextQueue = avoidNext;
            if (Queue.RepeatMode == RepeatMode.RepeatOne) Queue.Position--; // Don't advance Queue, play the same thing again

            if (Queue.Position >= Queue.Queue.Count)
            {
                if (Queue.RepeatMode == RepeatMode.RepeatAll) // Go back to the first track and play it again
                {
                    Queue.Position = 0;
                    await PlayAsync();
                    return;
                }
                Stop();
                return;
            }
            await PlayAsync();
        }

        // Music Playing Controls
        private async void OnPlaybackStopped(object sender, EventArgs args)
        {
            if (!AvoidNextQueue) 
                await NextAsync();
            else
                AvoidNextQueue = false;
        }

        /// <summary>
        /// Plays a track. This is equivalent to calling Queue.Add() and then PlayMusic()./>
        /// </summary>
        /// <param name="path">The track to play</param>
        public async Task PlayAsync(string path)
        {
            Queue.Add(path);
            await PlayAsync();
        }
        /// <summary>
        /// Starts playing the Queue. In order to play a track, you must first add it to the Queue using <see cref="AddQueue(string)"/>.
        /// </summary>
        /// <param name="repeat">If true, avoids dequeuing the next track. Not to be used for anything other than the player.</param>
        public async Task PlayAsync(bool repeat = false)
        {
            if (IsLoading) return;
            IsLoading = true;
            SongLoading?.Invoke(null, EventArgs.Empty);

            if (!repeat && Queue.Queue.Count != 0)
                FilePath = Queue.Queue[Queue.Position];
            Queue.Position++;
            async Task PMusic()
            {
                CurrentBackend = await AudioBackendFactory.CreateBackendAsync(FilePath);

                CurrentBackend.Play();
                CurrentBackend.Volume = Volume;
                CurrentBackend.OnPlaybackStopped += OnPlaybackStopped;

                FileLoaded = true;
            }

            try
            {
                if (FileLoaded != true)
                {
                    await PMusic();
                }
                else
                {
                    AvoidNextQueue = true;
                    Stop();
                    await PMusic();
                }

                SongChanged?.Invoke(null,
                    EventArgs.Empty); // Now that playback has started without any issues, fire the song changed event.
            }
            catch (Exception e)
            {
                var args = new PlaybackExceptionEventArgs(e, $"{e.Message}\n{e.StackTrace}");
                SongException?.Invoke(null, args);
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Completely stops and disposes the player and resets all playback related variables to their defaults.
        /// </summary>
        public void Stop()
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
        public void Pause()
        {
            if (!Paused) 
                CurrentBackend?.Pause();
            Paused = true;
        }

        /// <summary>
        /// Resumes playback.
        /// </summary>
        public void Resume()
        {
            if (Paused) 
                CurrentBackend?.Play();
            Paused = false;
        }

        #endregion
    }
}