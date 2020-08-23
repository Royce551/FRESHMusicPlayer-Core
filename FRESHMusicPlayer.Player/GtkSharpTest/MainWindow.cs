using Gtk;
using System;
using System.Collections.Generic;
using FRESHMusicPlayer;
using FRESHMusicPlayer.Handlers;

namespace GtkSharpTest
{
    class MainWindow : Window
    {
        Player player = new Player();
        List<string> lib = new List<string>();

        Label statusLabel = new Label("Status status status status");
        Button pauseResumeButton = new Button("Pause or Resume");
        Button stopButton = new Button("STOP!");
        Button playButton = new Button("Play Song!");

        public MainWindow() : base("gtk sharp test") 
        {
            var vertical = new Box(Orientation.Vertical, 3);
            vertical.Add(pauseResumeButton);
            vertical.Add(stopButton);
            vertical.Add(playButton);

            var horizontal = new Box(Orientation.Horizontal, 3);
            horizontal.Add(statusLabel);
            horizontal.Add(vertical);

            Add(horizontal);

            ShowAll();

            pauseResumeButton.Clicked += (o, e) =>         
            {
                if (player.Paused) player.ResumeMusic();
                else player.PauseMusic();
            };

            stopButton.Clicked += (o, e) => player.StopMusic();

            playButton.Clicked += PlaySong_Clicked;
            player.SongChanged += Player_Changed;
            player.SongStopped += Player_Stopped;
            player.SongException += Player_Exception;
            lib = DatabaseHandler.ReadSongs();    
        }

        void Player_Exception(object s, PlaybackExceptionEventArgs e)
        {
            using var md = new MessageDialog(this, 
                                   DialogFlags.Modal, 
                                   MessageType.Error, 
                                   ButtonsType.Ok,
                                   null);
            md.Text = e.Details;
            md.Run();
        }

        void Player_Changed(object s, EventArgs e) 
        {
            statusLabel.Text = "Playing";
        }

        void Player_Stopped(object s, EventArgs e)
        {
            statusLabel.Text = "Stopped";
        }

        void PlaySong_Clicked(object sender, EventArgs e)
        {
            using var openFileDialog1 = new FileChooserDialog("Choose file",
                                                              this,
                                                              FileChooserAction.Open,
                                                              "Cancel", ResponseType.Cancel,
                                                              "OK", ResponseType.Accept);
            var result = (ResponseType) openFileDialog1.Run();
            if (result != ResponseType.Accept) return;
            player.AddQueue(openFileDialog1.Filename);
            player.PlayMusic();
            player.CurrentVolume = 0.2f;
            player.UpdateSettings();
        }
    }
}
