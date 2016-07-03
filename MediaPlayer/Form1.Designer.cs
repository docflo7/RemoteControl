using System;
using System.Collections.Generic;
using Constellation.Package;
using AxWMPLib;

namespace MediaPlayer
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">True if managed resources should be disposed; otherwise, false.</param>
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.player = new AxWMPLib.AxWindowsMediaPlayer();
            ((System.ComponentModel.ISupportInitialize)(this.player)).BeginInit();
            this.SuspendLayout();
            // 
            // player
            // 
            this.player.Enabled = true;
            this.player.Location = new System.Drawing.Point(0, 0);
            this.player.Name = "player";
            this.player.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("player.OcxState")));
            this.player.Size = new System.Drawing.Size(330, 300);
            this.player.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(330, 300);
            this.Controls.Add(this.player);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.player)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AxWMPLib.AxWindowsMediaPlayer player;
        System.Timers.Timer timer = new System.Timers.Timer();



        [StateObjectLink(Package = "MediaPlayer", Name = "CurrentSong")]
        private StateObjectNotifier MPlayer { get; set; }
        bool loadVid = true;

        #region Control Function

        [MessageCallback]
        private bool Shuffle(string mode)
        {
            bool shuffle = player.settings.getMode("shuffle");
            if (mode == "set")
            {
                player.settings.setMode("shuffle", !shuffle);
            }
            return shuffle;
        }

        [MessageCallback]
        private bool FullScreen(string mode)
        {
            bool fullscreen = player.fullScreen;
            if (mode == "set")
            {
                player.fullScreen = !fullscreen;
            }
            return fullscreen;
        }


        [MessageCallback]
        private void Play()
        {
            player.Ctlcontrols.play();
        }

        [MessageCallback]
        private void Pause()
        {
            player.Ctlcontrols.pause();
        }

        [MessageCallback]
        private void Stop()
        {
            player.Ctlcontrols.stop();
        }

        [MessageCallback]
        private void Previous()
        {
            player.Ctlcontrols.previous();
        }

        [MessageCallback]
        private void Next()
        {
            player.Ctlcontrols.next();
        }

        #endregion


        #region Music Control


        /// <summary>
        /// Sets the time.
        /// </summary>
        /// <param name="time">The time.</param>
        [MessageCallback]
        private void SetTime(int time)
        {
            player.Ctlcontrols.currentPosition = time;
        }


        /// <summary>
        /// Loads the artist.
        /// </summary>
        /// <param name="artist">The artist.</param>
        [MessageCallback]
        private void LoadArtist(string artist)
        {
            player.currentPlaylist = player.mediaCollection.getByAuthor(artist);
        }

        /*[MessageCallback]
        private void loadVideo()
        {
            player.currentPlaylist = player.mediaCollection.getAttributeStringCollection("Title", "Video");
        }*/

        /// <summary>
        /// Loads the album.
        /// </summary>
        /// <param name="album">The album.</param>
        [MessageCallback]
        private void LoadAlbum(string album)
        {
            player.currentPlaylist = player.mediaCollection.getByAlbum(album);
        }

        /// <summary>
        /// Loads the title from playlist.
        /// </summary>
        /// <param name="title">The title.</param>
        [MessageCallback]
        private void LoadTitleFromPlaylist(string title)
        {
            int i = 0;
            while (i < player.currentPlaylist.count)
            {
                if (player.currentPlaylist.Item[i].getItemInfo("Title") == title)
                {
                    player.Ctlcontrols.playItem(player.currentPlaylist.Item[i]);
                    return;
                }
                i++;
            }
        }


        /// <summary>
        /// Gets the collection.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        [MessageCallback]
        private TupleList<string, string, string> GetCollection(string type, string value)
        {

            var playlist = player.mediaCollection.getByAttribute(type, value);

            int count = playlist.count;
            var collection = new TupleList<string, string, string>
            {
                {"Artist", "Album", "Title"  }
            };


            for (int i = 0; i < count; i++)
            {
                collection.Add(playlist.Item[i].getItemInfo("Author"), playlist.Item[i].getItemInfo("Album"), playlist.Item[i].getItemInfo("Title"));
            }
            return collection;

        }
        /// <summary>
        /// Gets the playlist.
        /// </summary>
        /// <returns></returns>
        [MessageCallback]
        private TupleList<string, string, string> GetPlaylist()
        {
            int count = player.currentPlaylist.count;
            var collection = new TupleList<string, string, string>
            {
                {"Artist", "Album", "Title"  }
            };


            for (int i = 0; i < count; i++)
            {
                collection.Add(player.currentPlaylist.Item[i].getItemInfo("Author"), player.currentPlaylist.Item[i].getItemInfo("Album"), player.currentPlaylist.Item[i].getItemInfo("Title"));
            }
            return collection;
        }

        #endregion

        /// <summary>
        /// Gets the videos.
        /// </summary>
        [MessageCallback]
        private void GetVideos(string search)
        {
            loadVid = false;
            player.currentPlaylist = player.mediaCollection.getByAttribute("MediaType", "video");
            player.Ctlcontrols.pause();

            if (search != null)
            {
                int count = player.currentPlaylist.count;
                int i = 0;
                while (i < count)
                {
                    if (!player.currentPlaylist.Item[i].getItemInfo("Title").ToLower().Contains(search.ToLower()))
                    {
                        player.currentPlaylist.removeItem(player.currentPlaylist.Item[i]);
                        i--;
                        count = player.currentPlaylist.count;
                    }
                    i++;
                                
                }
                loadVid = true;
                player.Ctlcontrols.playItem(player.currentPlaylist.Item[0]);

            }
            else
            {
                player.Ctlcontrols.pause();
                loadVid = true;
            }
            player.Ctlcontrols.pause();
            loadVid = true;


        }

        /// <summary>
        /// Player media change event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event.</param>
        private void player_MediaChange(object sender, _WMPOCXEvents_MediaChangeEvent e)
        {

            if (DateTime.Now > this.MPlayer.Value.LastUpdate.AddSeconds(2))
            {
                if (loadVid)
                {
                    PackageHost.PushStateObject("CurrentPlaylist", GetPlaylist());
                    PackageHost.PushStateObject("CurrentSong", new TupleList<string, string, string> { { player.currentMedia.getItemInfo("Author"), player.currentMedia.getItemInfo("Album"), player.currentMedia.getItemInfo("Title") } });

                }

            }
        }



        /// <summary>
        /// Handles the Elapsed event of the Timer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Timer_Elapsed(object sender, System.EventArgs e)
        {
            string total = convertInHHMMSS(player.currentMedia.duration);
            string current = convertInHHMMSS(player.Ctlcontrols.currentPosition);

            PackageHost.PushStateObject("TimeData", new { total, current, player.currentMedia.duration, player.Ctlcontrols.currentPosition });
        }


        /// <summary>
        /// Converts seconds in HHMMSS.
        /// </summary>
        /// <param name="time">The time in seconds.</param>
        /// <returns></returns>
        private string convertInHHMMSS(double time)
        {
            int seconds = (int)time;
            int minutes = seconds / 60;
            int hours = minutes / 60;
            seconds = seconds % 60;
            minutes = minutes % 60;
            return string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
        }

    }

    /// <summary>
    /// Creates a new list of tuple
    /// </summary>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    /// <typeparam name="T2">The type of the 2.</typeparam>
    /// <typeparam name="T3">The type of the 3.</typeparam> />
    public class TupleList<T1, T2, T3> : List<Tuple<T1, T2, T3>>
    {
        /// <summary>
        /// Adds the specified datas in the list.
        /// </summary>
        /// <param name="artist">The artist.</param>
        /// <param name="album">The album.</param>
        /// <param name="title">The title.</param>
        public void Add(T1 artist, T2 album, T3 title)
        {
            Add(new Tuple<T1, T2, T3>(artist, album, title));
        }
    }
}
