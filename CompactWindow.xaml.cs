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
using System.Windows.Shapes;

namespace Media_Player
{
    /// <summary>
    /// Interaction logic for CompactWindow.xaml
    /// </summary>
    public partial class CompactWindow : Window
    {
        private MainWindow ?Master = null;
        public CompactWindow(MainWindow master_window)
        {
            InitializeComponent();
            Master = master_window;
            volume_bar.Value = Master.video_out_display.Volume * 100;
            volume_display.Text = $"Volume: {volume_bar.Value}";
        }

#pragma warning disable
        private void previous_song_btn_Click(object sender, RoutedEventArgs e)
        {
            Master.Previous();
        }

        private void rewind_current_song_btn_Click(object sender, RoutedEventArgs e)
        {
            Master.Rewind();
        }

        private void repeat_btn_Click(object sender, RoutedEventArgs e)
        {
            Master.ToggleRepeat();
        }

        private async void pause_btn_Click(object sender, RoutedEventArgs e) // compied logic from mainwindow pause event
        {
            if (MainWindow.current_state == PlayerState.Paused || MainWindow.current_state == PlayerState.None)
            {
                if (MainWindow.current_file_index < Master.playlist_contents.Items.Count)
                {
                    MainWindow.current_state = PlayerState.Playing;
                    pause_btn.Content = new Image
                    {
                        Source = new BitmapImage(new Uri($"/Sprites{MainWindow.sprite_path}pause.png", UriKind.RelativeOrAbsolute))
                    };
                    Master.pause_btn.Dispatcher.Invoke(new Action(() =>
                    {
                        Master.pause_btn.Content = new Image
                        {
                            Source = new BitmapImage(new Uri($"/Sprites{MainWindow.sprite_path}pause.png", UriKind.RelativeOrAbsolute))
                        };
                    }));
                    ListViewItem? media_item = Master.playlist_contents.Items[MainWindow.current_file_index] as ListViewItem;

                    if (media_item != null)
                    {
                        bool to_increment = (MainWindow.current_state == PlayerState.None) ? true : false;
                        await Master.PlayMedia(media_item.Tag.ToString(), increment: to_increment);
                    }
                    else
                    {
                        MessageBox.Show($"Something went wrong while attempting to fetch object at {MainWindow.current_file_index}", "Couldn't fetch object at index", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else if (MainWindow.current_state == PlayerState.Playing)
            {
                MainWindow.current_state = PlayerState.Paused;
                pause_btn.Content = new Image
                {
                    Source = new BitmapImage(new Uri($"/Sprites{MainWindow.sprite_path}play.png", UriKind.RelativeOrAbsolute))
                };
                Master.pause_btn.Dispatcher.Invoke(new Action(() =>
                {
                    Master.pause_btn.Content = new Image
                    {
                        Source = new BitmapImage(new Uri($"/Sprites{MainWindow.sprite_path}play.png", UriKind.RelativeOrAbsolute))
                    };
                }));
                Master.video_out_display.Pause();
            }
        }

        private void shuffle_btn_Click(object sender, RoutedEventArgs e)
        {
            Master.ShuffleMain();
            if (MainWindow.is_shuffled)
            {
                shuffle_btn.Content = new Image
                {
                    Source = new BitmapImage(new Uri($"/Sprites{MainWindow.sprite_path}shuffle_triggered.png", UriKind.RelativeOrAbsolute))
                };
            }
            else
            {
                shuffle_btn.Content = new Image
                {
                    Source = new BitmapImage(new Uri($"/Sprites{MainWindow.sprite_path}shuffle_untriggered.png", UriKind.RelativeOrAbsolute))
                };
            }
        }

        private void skip_song_btn_Click(object sender, RoutedEventArgs e)
        {
            Master.Skip();
        }

        public TimeSpan CalculateNewTimespan()
        {
            double slider_pos = Math.Clamp(media_pos_bar.Value, media_pos_bar.Minimum, media_pos_bar.Maximum);
            double media_duration = Master.video_out_display.NaturalDuration.TimeSpan.TotalSeconds;

            double target_seconds = (slider_pos / media_pos_bar.Maximum) * media_duration;

            return TimeSpan.FromSeconds(target_seconds);
        }
        private void volume_bar_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.current_state = PlayerState.Paused;
        }

        private void volume_bar_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Master.Seek();
        }

        private void volume_bar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int volume = (int)volume_bar.Value;
            volume_display.Text = $"Volume: {volume}";
            Master.video_out_display.Volume = volume_bar.Value / 100;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            MainWindow.compact_window = null;
            MainWindow.compact_mode_enabled = false;
            Master.volume_slider.Dispatcher.Invoke(new Action(() =>
            {
                Master.volume_slider.Value = volume_bar.Value;
            }));
            Master.UpdateVolume();
        }
    }
}