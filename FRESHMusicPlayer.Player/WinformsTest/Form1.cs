using System;
using System.Collections.Generic;
using System.Text;
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
            player.Queue.QueueChanged += Queue_QueueChanged;
            library = DatabaseHandler.ReadSongs();
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
            var builder = new StringBuilder();
            foreach (var track in player.Queue.Queue)
            {
                builder.AppendLine(track);
            }
            MessageBox.Show(builder.ToString());
            player.Queue.ManualShuffle();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            player.Queue.Shuffle = !player.Queue.Shuffle;
            MessageBox.Show(player.Queue.Shuffle.ToString());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var list = new List<string>();
            for (var i = 1; i < 100; i++)
                list.Add(i.ToString());
            player.Queue.Add(list.ToArray());
            //var openFileDialog1 = new OpenFileDialog();
            //if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
            //player.Queue.Add(openFileDialog1.FileName);
            //player.PlayMusic();
            //player.CurrentVolume = 0.2f;
            //player.UpdateSettings();
        }
    }
}
