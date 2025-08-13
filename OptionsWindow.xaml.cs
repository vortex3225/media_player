using Media_Player.Objects;
using Media_Player.Scripts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
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
    /// Interaction logic for OptionsWindow.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        private AppSettings settings = MainWindow.fetched_settings;
        private MainWindow actual = null;
        public OptionsWindow(MainWindow window)
        {
            InitializeComponent();

            if (MainWindow.compact_mode_enabled)
            {
                compact_mode_checkbox.IsChecked = true;
                compact_aot_panel.Visibility = Visibility.Visible;
            }
            else
            {
                compact_aot_panel.Visibility = Visibility.Collapsed;
            }

            resume_checkbox.IsChecked = settings.resume_on_enter;
            save_opened_files_checkbox.IsChecked = settings.save_files;

            this.Closing += OptionsWindow_Closing;
            actual = window;
        }

        private void OptionsWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            AppHandler.SaveFile(settings);
            MainWindow.createdOptions = false;
        }

        private void compact_mode_checkbox_Click(object sender, RoutedEventArgs e)
        {
            if (compact_mode_checkbox.IsChecked == true)
            {
                MainWindow.compact_window = new CompactWindow(actual);
                MainWindow.compact_window.Show();
                MainWindow.compact_window.song_title.Text = actual.media_title_display.Text;
                MainWindow.compact_window.media_pos_bar.Value = actual.media_position_slider.Value;
                MainWindow.compact_window.current_display.Text = actual.current_pos_display.Text;
                MainWindow.compact_window.total_display.Text = actual.total_pos_display.Text;
                MainWindow.compact_window.volume_bar.Value = actual.volume_slider.Value;
                MainWindow.compact_window.volume_display.Text = actual.volume_display.Text;

                int current_index = MainWindow.current_file_index;
                if (current_index + 1 < actual.playlist_contents.Items.Count)
                {
                    ListViewItem ?item = actual.playlist_contents.Items[current_index + 1] as ListViewItem;
                    if (item != null)
                    {
                        MainWindow.compact_window.next_song_display.Text = $"Next song: {System.IO.Path.GetFileNameWithoutExtension(item.Tag.ToString())}";
                    }
                }
                else
                {
                    ListViewItem ?item = actual.playlist_contents.Items[0] as ListViewItem;
                    if (item != null)
                    {
                        MainWindow.compact_window.next_song_display.Text = $"Next song: {System.IO.Path.GetFileNameWithoutExtension(item.Tag.ToString())}";
                    }
                }

                MainWindow.compact_mode_enabled = true;
                this.Close();
            }
            else
            {
                if (MainWindow.compact_window != null)
                {
                    MainWindow.compact_window.Close();
                }
                MainWindow.compact_mode_enabled = false;
                compact_aot_panel.Visibility = Visibility.Collapsed;
            }
        }

        private void resume_checkbox_Click(object sender, RoutedEventArgs e)
        {
            settings.resume_on_enter = (bool)resume_checkbox.IsChecked;
        }

        private void dark_mode_checkbox_Click(object sender, RoutedEventArgs e)
        {

        }

        private void save_opened_files_checkbox_Click(object sender, RoutedEventArgs e)
        {
            settings.save_files = (bool)save_opened_files_checkbox.IsChecked;
        }

        private void compact_always_on_top_check_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.compact_window != null)
            {
                MainWindow.compact_window.Topmost = (bool)compact_always_on_top_check.IsChecked;
            }
        }
    }
}