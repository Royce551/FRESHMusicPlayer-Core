using System;
using System.Collections.Generic;
using System.Windows.Forms;
using FRESHMusicPlayer;
using FRESHMusicPlayer.Handlers;

namespace WinformsTest
{
    public partial class FreshMusicPlayer : Form
    {
        private Player player = new Player();
        private List<string> library = new List<string>();

        public FreshMusicPlayer()
        {
            InitializeComponent();
            player.SongChanged += Player_songChanged;
            player.SongStopped += Player_songStopped;
            player.SongException += Player_songException;
            library = DatabaseHandler.ReadSongs();
        }

        private void Player_songException(object sender, PlaybackExceptionEventArgs e)
        {
            MessageBox.Show("something did a fucky wucky");
        }

        private void Player_songStopped(object sender, EventArgs e)
        {
            label1.Text = "stopped";
        }

        private void Player_songChanged(object sender, EventArgs e)
        {
            label1.Text = "playing something!";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (player.Paused) player.ResumeMusic();
            else player.PauseMusic();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            player.StopMusic();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
            player.AddQueue(openFileDialog1.FileName);
            player.PlayMusic();
            player.CurrentVolume = 0.2f;
            player.UpdateSettings();
        }
    }
}
