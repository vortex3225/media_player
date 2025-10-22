using Media_Player.Objects;
using Media_Player.Scripts;
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
    /// Interaction logic for SelectPlaylistWindow.xaml
    /// </summary>
    public partial class SelectPlaylistWindow : Window
    {

        public SelectPlaylistWindow(List<PlaylistObject> fetched_playlists)
        {
            InitializeComponent();
            
            foreach (PlaylistObject playlist in fetched_playlists)
            {
                ComboBoxItem new_item = new ComboBoxItem();
                new_item.Name = playlist.name;
                new_item.Content = playlist.name;
                playlist_selector_box.Items.Add(new_item);
            }
        }

        private void confirm_btn_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem ?selected_playlist_item = playlist_selector_box.SelectedItem as ComboBoxItem;
            if (selected_playlist_item != null)
            {
                PlaylistHandler.selected_playlist = PlaylistHandler.LoadPlaylist(selected_playlist_item.Name);
                DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Please select a playlist first!", "No playlist selected", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
    }
}
