using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer
{
    public class PlayerContext
    {
        private Player[] players;
        public Player[] Players
        {
            get
            {
                if (players == null)
                {
                    players = new Player[Properties.Settings.Default.Players];
                    for (var i = 0; i < players.Length; i++)
                        players[i] = new Player();
                }
                return players;
            }
        }
        private int playerIndex;
        public Player CurrentPlayer { get { return players[playerIndex]; } }

        private List<AvailableSong> availableSongs;
        public List<AvailableSong> AvailableSongs 
        {
            get 
            {
                if (availableSongs == null)
                {
                    availableSongs = new List<AvailableSong>();
                    foreach (string filePath in Directory.GetFiles(Properties.Settings.Default.MusicFolder.Replace("%User%", Environment.UserName), Properties.Settings.Default.MusicFileType, SearchOption.AllDirectories))
                        availableSongs.Add(new AvailableSong { FilePath = filePath });
                }
                return availableSongs;
            }
        }

        public void Play(AvailableSong song)
        {
            players[playerIndex++].Stop();
            if (playerIndex >= players.Length)
                playerIndex = 0;
            players[playerIndex].SourcePath = song.FilePath;
            players[playerIndex].Play();
        }
    }

    public class Player : INotifyPropertyChanged
    {
        private string sourcePath;
        public string SourcePath { get { return sourcePath; } set { sourcePath = value; OnPropertyChanged("SourcePath"); OnPropertyChanged("Source"); } }
        public Uri Source { get { return SourcePath != null ? new Uri(SourcePath, UriKind.Absolute) : null; } }

        public event EventHandler Playing;
        public void Play()
        {
            if (Playing != null)
                Playing(this, EventArgs.Empty);
        }

        public event EventHandler Stopping;
        public void Stop()
        {
            if (Stopping != null)
                Stopping(this, EventArgs.Empty);
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
