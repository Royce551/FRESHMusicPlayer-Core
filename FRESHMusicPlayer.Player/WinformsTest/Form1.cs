using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FRESHMusicPlayer;
namespace WinformsTest
{
    public partial class FreshMusicPlayer : Form
    {
        public FreshMusicPlayer()
        {
            InitializeComponent();
            Player.songChanged += Player_songChanged;
            Player.songStopped += Player_songStopped;
            Player.songException += Player_songException;
        }

        private void Player_songException(object sender, FRESHMusicPlayer.Handlers.PlaybackExceptionEventArgs e)
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
            if (Player.paused) Player.ResumeMusic();
            else Player.PauseMusic();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Player.StopMusic();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Player.AddQueue(openFileDialog1.FileName);
                Player.PlayMusic();
                Player.currentvolume = 0.2f;
                Player.UpdateSettings();
            }
        }
    }
}
