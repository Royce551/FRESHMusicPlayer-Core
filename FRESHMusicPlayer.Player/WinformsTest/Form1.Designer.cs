using FRESHMusicPlayer;

namespace WinformsTest
{
    partial class FreshMusicPlayer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.CurrentBackendLabel = new System.Windows.Forms.Label();
            this.CurrentTimeLabel = new System.Windows.Forms.Label();
            this.TotalTimeLabel = new System.Windows.Forms.Label();
            this.AvoidNextQueueLabel = new System.Windows.Forms.Label();
            this.VolumeLabel = new System.Windows.Forms.Label();
            this.FileLoadedLabel = new System.Windows.Forms.Label();
            this.PausedLabel = new System.Windows.Forms.Label();
            this.QueueLabel = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.PositionLabel = new System.Windows.Forms.Label();
            this.RepeatModeLabel = new System.Windows.Forms.Label();
            this.ShuffleLabel = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.FilePathLabel = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(542, 14);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(167, 47);
            this.button1.TabIndex = 1;
            this.button1.Text = "Pause/Resume";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(542, 68);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(167, 47);
            this.button2.TabIndex = 2;
            this.button2.Text = "Stop";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(542, 122);
            this.button3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(167, 47);
            this.button3.TabIndex = 3;
            this.button3.Text = "Play Song!";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(632, 175);
            this.button4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(77, 47);
            this.button4.TabIndex = 5;
            this.button4.Text = "Next";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(542, 175);
            this.button5.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(82, 47);
            this.button5.TabIndex = 6;
            this.button5.Text = "Previous";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(542, 250);
            this.button6.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(167, 47);
            this.button6.TabIndex = 7;
            this.button6.Text = "Extra Button 1";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(542, 303);
            this.button7.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(167, 47);
            this.button7.TabIndex = 8;
            this.button7.Text = "Extra Button 2";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.FilePathLabel);
            this.groupBox1.Controls.Add(this.QueueLabel);
            this.groupBox1.Controls.Add(this.PausedLabel);
            this.groupBox1.Controls.Add(this.FileLoadedLabel);
            this.groupBox1.Controls.Add(this.VolumeLabel);
            this.groupBox1.Controls.Add(this.AvoidNextQueueLabel);
            this.groupBox1.Controls.Add(this.TotalTimeLabel);
            this.groupBox1.Controls.Add(this.CurrentTimeLabel);
            this.groupBox1.Controls.Add(this.CurrentBackendLabel);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(505, 178);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Player";
            // 
            // CurrentBackendLabel
            // 
            this.CurrentBackendLabel.AutoSize = true;
            this.CurrentBackendLabel.Location = new System.Drawing.Point(7, 23);
            this.CurrentBackendLabel.Name = "CurrentBackendLabel";
            this.CurrentBackendLabel.Size = new System.Drawing.Size(38, 15);
            this.CurrentBackendLabel.TabIndex = 0;
            this.CurrentBackendLabel.Text = "label1";
            // 
            // CurrentTimeLabel
            // 
            this.CurrentTimeLabel.AutoSize = true;
            this.CurrentTimeLabel.Location = new System.Drawing.Point(7, 38);
            this.CurrentTimeLabel.Name = "CurrentTimeLabel";
            this.CurrentTimeLabel.Size = new System.Drawing.Size(38, 15);
            this.CurrentTimeLabel.TabIndex = 1;
            this.CurrentTimeLabel.Text = "label1";
            // 
            // TotalTimeLabel
            // 
            this.TotalTimeLabel.AutoSize = true;
            this.TotalTimeLabel.Location = new System.Drawing.Point(7, 53);
            this.TotalTimeLabel.Name = "TotalTimeLabel";
            this.TotalTimeLabel.Size = new System.Drawing.Size(38, 15);
            this.TotalTimeLabel.TabIndex = 2;
            this.TotalTimeLabel.Text = "label1";
            // 
            // AvoidNextQueueLabel
            // 
            this.AvoidNextQueueLabel.AutoSize = true;
            this.AvoidNextQueueLabel.Location = new System.Drawing.Point(7, 68);
            this.AvoidNextQueueLabel.Name = "AvoidNextQueueLabel";
            this.AvoidNextQueueLabel.Size = new System.Drawing.Size(38, 15);
            this.AvoidNextQueueLabel.TabIndex = 3;
            this.AvoidNextQueueLabel.Text = "label1";
            // 
            // VolumeLabel
            // 
            this.VolumeLabel.AutoSize = true;
            this.VolumeLabel.Location = new System.Drawing.Point(7, 88);
            this.VolumeLabel.Name = "VolumeLabel";
            this.VolumeLabel.Size = new System.Drawing.Size(38, 15);
            this.VolumeLabel.TabIndex = 4;
            this.VolumeLabel.Text = "label1";
            // 
            // FileLoadedLabel
            // 
            this.FileLoadedLabel.AutoSize = true;
            this.FileLoadedLabel.Location = new System.Drawing.Point(7, 103);
            this.FileLoadedLabel.Name = "FileLoadedLabel";
            this.FileLoadedLabel.Size = new System.Drawing.Size(38, 15);
            this.FileLoadedLabel.TabIndex = 5;
            this.FileLoadedLabel.Text = "label1";
            // 
            // PausedLabel
            // 
            this.PausedLabel.AutoSize = true;
            this.PausedLabel.Location = new System.Drawing.Point(7, 118);
            this.PausedLabel.Name = "PausedLabel";
            this.PausedLabel.Size = new System.Drawing.Size(38, 15);
            this.PausedLabel.TabIndex = 6;
            this.PausedLabel.Text = "label1";
            // 
            // QueueLabel
            // 
            this.QueueLabel.AutoSize = true;
            this.QueueLabel.Location = new System.Drawing.Point(7, 133);
            this.QueueLabel.Name = "QueueLabel";
            this.QueueLabel.Size = new System.Drawing.Size(38, 15);
            this.QueueLabel.TabIndex = 7;
            this.QueueLabel.Text = "label1";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.PositionLabel);
            this.groupBox2.Controls.Add(this.RepeatModeLabel);
            this.groupBox2.Controls.Add(this.ShuffleLabel);
            this.groupBox2.Location = new System.Drawing.Point(12, 196);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(505, 77);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Queue";
            // 
            // PositionLabel
            // 
            this.PositionLabel.AutoSize = true;
            this.PositionLabel.Location = new System.Drawing.Point(7, 53);
            this.PositionLabel.Name = "PositionLabel";
            this.PositionLabel.Size = new System.Drawing.Size(38, 15);
            this.PositionLabel.TabIndex = 2;
            this.PositionLabel.Text = "label1";
            // 
            // RepeatModeLabel
            // 
            this.RepeatModeLabel.AutoSize = true;
            this.RepeatModeLabel.Location = new System.Drawing.Point(7, 38);
            this.RepeatModeLabel.Name = "RepeatModeLabel";
            this.RepeatModeLabel.Size = new System.Drawing.Size(38, 15);
            this.RepeatModeLabel.TabIndex = 1;
            this.RepeatModeLabel.Text = "label1";
            // 
            // ShuffleLabel
            // 
            this.ShuffleLabel.AutoSize = true;
            this.ShuffleLabel.Location = new System.Drawing.Point(7, 23);
            this.ShuffleLabel.Name = "ShuffleLabel";
            this.ShuffleLabel.Size = new System.Drawing.Size(38, 15);
            this.ShuffleLabel.TabIndex = 0;
            this.ShuffleLabel.Text = "label1";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // FilePathLabel
            // 
            this.FilePathLabel.AutoSize = true;
            this.FilePathLabel.Location = new System.Drawing.Point(7, 148);
            this.FilePathLabel.Name = "FilePathLabel";
            this.FilePathLabel.Size = new System.Drawing.Size(38, 15);
            this.FilePathLabel.TabIndex = 8;
            this.FilePathLabel.Text = "label1";
            // 
            // FreshMusicPlayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(722, 358);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "FreshMusicPlayer";
            this.Text = "FRESHMusicPlayer Core Tester Thingamajiga";
            this.Load += new System.EventHandler(this.FreshMusicPlayer_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label AvoidNextQueueLabel;
        private System.Windows.Forms.Label TotalTimeLabel;
        private System.Windows.Forms.Label CurrentTimeLabel;
        private System.Windows.Forms.Label CurrentBackendLabel;
        private System.Windows.Forms.Label QueueLabel;
        private System.Windows.Forms.Label PausedLabel;
        private System.Windows.Forms.Label FileLoadedLabel;
        private System.Windows.Forms.Label VolumeLabel;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label PositionLabel;
        private System.Windows.Forms.Label RepeatModeLabel;
        private System.Windows.Forms.Label ShuffleLabel;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label FilePathLabel;
    }
}

