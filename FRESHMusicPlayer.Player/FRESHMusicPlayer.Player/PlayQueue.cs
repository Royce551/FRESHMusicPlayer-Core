using System;
using System.Collections.Generic;
using System.Text;

namespace FRESHMusicPlayer
{
    /// <summary>
    /// Represents the player's queue.
    /// </summary>
    public class PlayQueue
    {
        /// <summary>
        /// Gets or sets the current queue. This is settable for situations where there's no method for what you want to do.
        /// Use the methods in this class for managing the queue so that events can fire and stuff doesn't break in the future.
        /// </summary>
        public List<string> Queue
        {
            get
            {
                if (Shuffle)
                    return shuffledQueue;
                else return queue;
            }
            set
            {
                if (Shuffle)
                {
                    shuffledQueue = value;
                    ShuffleQueue();
                }
                queue = value;
            }
        }

        private bool shuffle = false;
        /// <summary>
        /// Gets or sets whether the queue should be shuffled.
        /// </summary>
        public bool Shuffle 
        {
            get => shuffle;
            set
            {
                shuffle = value;
                if (shuffle)
                {
                    shuffledQueue = new List<string>(queue);
                    ShuffleQueue();
                }
                else shuffledQueue = null;
                QueueChanged?.Invoke(null, EventArgs.Empty);
            }
        }
        /// <summary>
        /// Gets or sets the current repeat mode.
        /// </summary>
        public RepeatMode RepeatMode { get; set; } = RepeatMode.None;
        /// <summary>
        /// Gets or sets the index in the queue of the track that the Player is going to play *next*.
        /// </summary>
        public int Position { get; set; }
        /// <summary>
        /// Fired when the queue changes.
        /// </summary>
        public event EventHandler QueueChanged;

        private List<string> queue = new List<string>();
        private List<string> shuffledQueue;

        private readonly Random rng = new Random();

        /// <summary>
        /// Adds a track to the queue.
        /// </summary>
        /// <param name="filePath">The track to add</param>
        public void Add(string filePath)
        {
            queue.Add(filePath);
            if (Shuffle)
            {
                shuffledQueue.Add(filePath);
                ShuffleQueue();
            }
            QueueChanged?.Invoke(null, EventArgs.Empty);
        }
        /// <summary>
        /// Adds multiple tracks to the queue.
        /// </summary>
        /// <param name="filePaths">The tracks to add.</param>
        public void Add(string[] filePaths)
        {
            queue.AddRange(filePaths);
            if (Shuffle)
            {
                shuffledQueue.AddRange(filePaths);
                ShuffleQueue();
            }
            QueueChanged?.Invoke(null, EventArgs.Empty);
        }
        public void PlayNext(string filePath)
        {
            queue.Insert(Position, filePath);
            if (Shuffle)
            {
                shuffledQueue.Insert(Position, filePath);
                // not shuffling since we want this track to play next
            }
            QueueChanged?.Invoke(null, EventArgs.Empty);
        }

        /// <summary>
        /// Clears the queue.
        /// </summary>
        public void Clear()
        {
            queue.Clear();
            if (Shuffle)
                shuffledQueue.Clear();
            Position = 0;
            QueueChanged?.Invoke(null, EventArgs.Empty);
        }
        /// <summary>
        /// Shuffles the queue. If <see cref="Shuffle"/> isn't true, this will not do anything.
        /// </summary>
        public void ManualShuffle()
        {
            if (Shuffle)
                ShuffleQueue();
            QueueChanged?.Invoke(null, EventArgs.Empty);
        }
        /// <summary>
        /// Removes a track from the queue.
        /// </summary>
        /// <param name="index">The index of the track you want to remove.</param>
        public void Remove(int index)
        {
            if (index <= (Position - 1)) Position--;
            if (Position < 0) Position = 1;
            queue.RemoveAt(index);
            if (Shuffle)
                shuffledQueue.RemoveAt(index);
            QueueChanged?.Invoke(null, EventArgs.Empty);
        }

        private void ShuffleQueue()
        {
            var listtosort = new List<string>();
            var listtoreinsert = new List<string>();
            var number = 0;
            foreach (var x in shuffledQueue)
            {
                if (Position < number) listtosort.Add(x);
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
            listtoreinsert.AddRange(listtosort);
            shuffledQueue = listtoreinsert;
        }
    }
    /// <summary>
    /// The way that the queue will be repeated
    /// </summary>
    public enum RepeatMode
    {
        /// <summary>
        /// Do not repeat tracks in the queue
        /// </summary>
        None,
        /// <summary>
        /// Repeat the currently playing track
        /// </summary>
        RepeatOne,
        /// <summary>
        /// Repeat the entire queue
        /// </summary>
        RepeatAll
    }
}
