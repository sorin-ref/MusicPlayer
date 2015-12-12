using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MusicPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = Context;

            DispatcherTimer timer = new DispatcherTimer { Interval = Properties.Settings.Default.TickInterval };
            timer.Tick += (ts, te) =>
            {
                var element = PlayersItemsControl.ItemContainerGenerator.ContainerFromItem(Context.CurrentPlayer) as FrameworkElement;
                var mediaPlayer = VisualTreeHelper.GetChild(element, 0) as MediaElement;
                if (mediaPlayer.Source == null || isNextTriggered || mediaPlayer.NaturalDuration.HasTimeSpan && mediaPlayer.Position >= mediaPlayer.NaturalDuration.TimeSpan - Properties.Settings.Default.CrossFadingInterval)
                {
                    isNextTriggered = false;
                    var playableSongs = Context.AvailableSongs.Where(s => s.IsPlayable).ToArray();
                    if (playableSongs.Length > 0)
                    {
                        var random = new Random();
                        var song = playableSongs[random.Next(playableSongs.Length)];
                        Context.Play(song);
                        AvailableSongsListBox.ScrollIntoView(song);
                        AvailableSongsListBox.SelectedItem = song;
                    }
                }
            };
            timer.Start();

            SearchTextBox.Focus();
        }

        private PlayerContext context;
        public PlayerContext Context
        {
            get 
            {
                if (context == null)
                    context = new PlayerContext();
                foreach (Player player in context.Players)
                {
                    player.Playing += Player_Playing;
                    player.Stopping += Player_Stopping;
                }
                return context;
            }
        }

        private void AvailableSongsListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                PlaySelectedFile();
        }

        private void AvailableSongsListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                PlaySelectedFile();
        }

        private void PlaySelectedFile()
        {
            if (AvailableSongsListBox.SelectedItem == null)
                return;
            var song = (AvailableSong)AvailableSongsListBox.SelectedItem;
            if (song.IsPlayable)
                Context.Play(song);
        }

        private void Player_Playing(object sender, EventArgs e)
        {
            var element = PlayersItemsControl.ItemContainerGenerator.ContainerFromItem(sender) as FrameworkElement;
            var mediaPlayer = VisualTreeHelper.GetChild(element, 0) as MediaElement;
            DispatcherTimer timer = new DispatcherTimer { Interval = Properties.Settings.Default.TickInterval };
            timer.Tick += (ts, te) =>
            {
                if (!mediaPlayer.IsLoaded)
                    return;
                timer.Stop();
                StartPlaying(mediaPlayer);
            };
            timer.Start();
        }

        private void StartPlaying(MediaElement mediaPlayer)
        {
            mediaPlayer.Volume = 0;
            int count = (int)(Properties.Settings.Default.CrossFadingInterval.TotalMilliseconds / Properties.Settings.Default.TickInterval.TotalMilliseconds);
            int i = 0;
            DispatcherTimer timer = new DispatcherTimer { Interval = Properties.Settings.Default.TickInterval };
            timer.Tick += (ts, te) =>
            {
                i++;
                mediaPlayer.Volume = i < count ? (double)i / count : 1;
                if (i >= count)
                    timer.Stop();
            };
            mediaPlayer.Play();
            timer.Start();
        }

        private void Player_Stopping(object sender, EventArgs e)
        {
            DispatcherTimer timer = new DispatcherTimer { Interval = Properties.Settings.Default.CrossFadingInterval };
            timer.Tick += (ts, te) => 
            {
                timer.Stop();
                var element = PlayersItemsControl.ItemContainerGenerator.ContainerFromItem(sender) as FrameworkElement;
                var mediaPlayer = VisualTreeHelper.GetChild(element, 0) as MediaElement;
                StartFadingOut(mediaPlayer);
            };
            timer.Start();
        }

        private void StartFadingOut(MediaElement mediaPlayer)
        {
            mediaPlayer.Volume = 1;
            int count = (int)(Properties.Settings.Default.CrossFadingInterval.TotalMilliseconds / Properties.Settings.Default.TickInterval.TotalMilliseconds);
            int i = count;
            DispatcherTimer timer = new DispatcherTimer { Interval = Properties.Settings.Default.TickInterval };
            timer.Tick += (ts, te) =>
            {
                i--;
                mediaPlayer.Volume = i > 0 ? (double)i / count : 0;
                if (i <= 0)
                {
                    mediaPlayer.Stop();
                    timer.Stop();
                }
            };
            timer.Start();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            AvailableSongsListBox.SelectedItem = Context.AvailableSongs.FirstOrDefault(s => s.ToString().ToLower().Contains(SearchTextBox.Text.ToLower()));
            if (AvailableSongsListBox.SelectedItem != null)
                AvailableSongsListBox.ScrollIntoView(AvailableSongsListBox.SelectedItem);
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            isNextTriggered = true;
        }
        private bool isNextTriggered;
    }
}
