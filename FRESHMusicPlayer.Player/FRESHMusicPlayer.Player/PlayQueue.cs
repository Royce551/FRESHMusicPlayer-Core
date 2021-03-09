using System;
using System.Collections.Generic;
using System.Text;

namespace FRESHMusicPlayer
{
    public class PlayQueue
    {
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
        public bool Shuffle 
        {
            get => shuffle;
            set
            {
                shuffle = value;
                if (shuffle) shuffledQueue = new List<string>(queue);
                else shuffledQueue = null;
            }
        }
        public RepeatMode RepeatMode { get; set; } = RepeatMode.None;
        public int Position { get; internal set; }

        public event EventHandler QueueChanged;

        private List<string> queue = new List<string>();
        private List<string> shuffledQueue;

        private readonly Random rng = new Random();

        public void Add(string filePath)
        {
            Queue.Add(filePath);
            if (Shuffle)
                ShuffleQueue();
            QueueChanged?.Invoke(null, EventArgs.Empty);
        }
        public void Add(string[] filePaths)
        {
            Queue.AddRange(filePaths);
            if (Shuffle)
                ShuffleQueue();
            QueueChanged?.Invoke(null, EventArgs.Empty);
        }
        public void Clear()
        {
            Queue.Clear();
            Position = 0;
            QueueChanged?.Invoke(null, EventArgs.Empty);
        }
        public void ManualShuffle()
        {
            if (Shuffle)
                ShuffleQueue();
            QueueChanged?.Invoke(null, EventArgs.Empty);
        }
        public void Remove(int index)
        {
            if (index <= (Position - 1)) Position--;
            if (Position < 0) Position = 1;
            Queue.RemoveAt(index);
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
    public enum RepeatMode
    {
        None,
        RepeatOne,
        RepeatAll
    }
}
