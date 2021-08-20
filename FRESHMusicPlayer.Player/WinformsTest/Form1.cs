using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using FRESHMusicPlayer;
using FRESHMusicPlayer.Handlers;
using System.Linq;

namespace WinformsTest
{
    public partial class FreshMusicPlayer : Form
    {
        private Player player = new Player();

        public FreshMusicPlayer()
        {
            InitializeComponent();
            player.SongChanged += Player_songChanged;
            player.SongStopped += Player_songStopped;
            player.SongException += Player_songException;
            player.Queue.QueueChanged += Queue_QueueChanged;
        }

        private void Queue_QueueChanged(object sender, EventArgs e)
        {
            var builder = new StringBuilder();
            foreach (var track in player.Queue.Queue)
            {
                builder.AppendLine(track);
            }
            MessageBox.Show(builder.ToString());
        }

        private void Player_songException(object sender, PlaybackExceptionEventArgs e)
        {
            MessageBox.Show(e.Details);
        }

        private void Player_songStopped(object sender, EventArgs e)
        {
  
        }

        private void Player_songChanged(object sender, EventArgs e)
        {
 
        }

        private void button1_Click(object sender, EventArgs e) // pause/resume
        {
            if (player.Paused) player.ResumeMusic();
            else player.PauseMusic();
        }

        private void button2_Click(object sender, EventArgs e) // stop
        {
            player.StopMusic();
        }

        private void button3_Click(object sender, EventArgs e) // play
        {
            var openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
            player.Queue.Add(openFileDialog1.FileName);
            player.PlayMusic();
            player.Volume = 0.7f;
        }

        private void FreshMusicPlayer_Load(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e) // next
        {
            player.NextSong();
        }

        private void button5_Click(object sender, EventArgs e) // previous
        {
            player.PreviousSong();
        }

        private void button6_Click(object sender, EventArgs e) // extra button 1
        {

        }

        private void button7_Click(object sender, EventArgs e) // extra button 2
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!player.FileLoaded) return;
            CurrentBackendLabel.Text = $"Current Backend: {player.CurrentBackend}";
            CurrentTimeLabel.Text = $"Current Time: {player.CurrentTime}";
            TotalTimeLabel.Text = $"Total Time: {player.TotalTime}";
            AvoidNextQueueLabel.Text = $"Avoid Next Queue: {player.AvoidNextQueue}";
            VolumeLabel.Text = $"Volume: {player.Volume}";
            FilePathLabel.Text = $"File Path: {player.FilePath}";
            FileLoadedLabel.Text = $"File Loaded: {player.FileLoaded}";
            PausedLabel.Text = $"Paused: {player.Paused}";
            QueueLabel.Text = $"Queue: put stuff here eventually";

            ShuffleLabel.Text = $"Shuffle: {player.Queue.Shuffle}";
            RepeatModeLabel.Text = $"Repeat: {player.Queue.RepeatMode}";
            PositionLabel.Text = $"Position: {player.Queue.Position}";
        }
    }
}
