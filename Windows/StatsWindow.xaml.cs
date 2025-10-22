using Media_Player.Objects;
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

namespace Media_Player.Windows
{
    /// <summary>
    /// Interaction logic for StatsWindow.xaml
    /// </summary>
    public partial class StatsWindow : Window
    {
        public StatsWindow()
        {
            InitializeComponent();
            total_listening_time_display.Text = $"{Math.Round(StatisticsObject.TimeListened / 60, 2)} min(s)";
            total_playlists_display.Text = $"{StatisticsObject.TotalPlaylists} playlists";
            total_tracks_played_display.Text = $"{StatisticsObject.TotalTracksInPlaylists} tracks";
            average_session_length_display.Text = $"{Math.Round(StatisticsObject.AverageSessionTime / 60, 2)} min(s)";
            current_session_time_display.Text = $"{Math.Round(StatisticsObject.CurrentSessionTime / 60, 2)} min(s)";
            highest_session_time_display.Text = $"{Math.Round(StatisticsObject.HighestSessionTime / 60, 2)} min(s)";
            most_listened_track_display.Text = StatisticsObject.MostListenedTrack;
            most_listened_track_plays_display.Text = StatisticsObject.MostListenedTrackPlays.ToString();
        }
    }
}
