using Media_Player.Objects;
using Media_Player.Scripts;
using Microsoft.Win32;
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
using static Media_Player.Scripts.Externals;

namespace Media_Player.Windows
{
    /// <summary>
    /// Interaction logic for ExportWindow.xaml
    /// </summary>
    public partial class ExportWindow : Window
    {
        private static ExportParams exportParams = new ExportParams(string.Empty, false, true, new List<PlaylistObject>());
        private static List<PlaylistObject> all_playlists;

        public ExportWindow()
        {
            InitializeComponent();
            
            all_playlists = PlaylistHandler.GetPlaylists();


            foreach (PlaylistObject obj in all_playlists)
            {
                CheckBox check_box = new CheckBox();
                check_box.Tag = obj;
                check_box.Content = obj.name;
                check_box.Foreground = (Brush)Application.Current.Resources["ForegroundBrush"];
                check_box.IsChecked = false;
                check_box.Click += Check_box_Click;
                playlists_list.Items.Add(check_box);
            }
        }

        private void Update()
        {
            int counted = 0;
            foreach (PlaylistObject playlist in exportParams.export_list)
                counted += playlist.playlist_items.Count;
            total_playlist_items_display.Text = $"Total playlist items: {counted}";

            save_playlist_item_playcounts_check.IsEnabled = exportParams.export_list.Count == 1;
            export_as_single_file_check.IsEnabled = exportParams.export_list.Count > 1;
            export_btn.IsEnabled = exportParams.export_list.Count > 0 && !string.IsNullOrEmpty(exportParams.export_path);

            if (string.IsNullOrEmpty(exportParams.export_path)) export_path_display.Text = $"Export path: {exportParams.export_path}";
            else export_path_display.Text = $"Export path: none";
        }

        private void Check_box_Click(object sender, RoutedEventArgs e)
        {
            CheckBox c = sender as CheckBox;
            
            if (c.IsChecked == true && !exportParams.export_list.Contains(c.Tag as PlaylistObject)) exportParams.export_list.Add(c.Tag as PlaylistObject);
            else if (c.IsChecked == false && exportParams.export_list.Contains(c.Tag as PlaylistObject)) exportParams.export_list.Remove(c.Tag as PlaylistObject);

            Update();
        }

        private void select_all_btn_Click(object sender, RoutedEventArgs e)
        {
            exportParams.export_list = new List<PlaylistObject>(all_playlists);
            Update();
        }

        private void deselect_all_btn_Click(object sender, RoutedEventArgs e)
        {
            exportParams.export_list.Clear();
            Update();
        }

        private void select_export_path_btn_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog ofd = new OpenFolderDialog();
            ofd.Title = "Select an export location";
            ofd.Multiselect = false;
            ofd.InitialDirectory = AppContext.BaseDirectory;

            if (ofd.ShowDialog() == true)
            {
                exportParams.export_path = ofd.FolderName;
                Update();
            }

        }

        private void export_as_single_file_check_Click(object sender, RoutedEventArgs e)
        {

        }

        private void save_playlist_item_playcounts_check_Click(object sender, RoutedEventArgs e)
        {

        }

        private void export_btn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
