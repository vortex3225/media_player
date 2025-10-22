using Media_Player.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Windows.Media.Media3D;

namespace Media_Player.Objects
{
    public static class StatisticsObject
    {
        public static DateTime InstallationDate { get; set; } = DateTime.Now;
        public static double TimeListened { get; set; } = 0;
        public static double CurrentSessionTime { get; set; } = 0;
        public static double HighestSessionTime { get; set; } = 0;
        public static double AverageSessionTime { get; set; } = 0;
        public static int Sessions {  get; set; } = 0;
        public static int TracksPlayed { get; set; } = 0;
        public static string MostListenedTrack { get; set; } = string.Empty;
        public static int MostListenedTrackPlays { get; set; } = 0;
        public static int TotalPlaylists { get; set; } = 0;
        public static int TotalTracksInPlaylists { get; set; } = 0;

        public static bool IsLoaded = false;

        private static string LCS()
        {
            return System.Configuration.ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
        }

        public static void Clear()
        {
            InstallationDate = DateTime.Now;
            TimeListened = 0;
            CurrentSessionTime = 0;
            HighestSessionTime = 0;
            AverageSessionTime = 0;
            Sessions = 0;
            TracksPlayed = 0;
            MostListenedTrack = string.Empty;
            TotalPlaylists = 0;
            TotalTracksInPlaylists = 0;
            MostListenedTrackPlays = 0;
            Sessions = 0;
        }


        public static void Load()
        {
            try
            {
                using (SQLiteConnection cnn = new SQLiteConnection(LCS()))
                {
                    cnn.Open();

                    string sql = "SELECT * FROM Stats";
                    using var cmd = new SQLiteCommand(sql, cnn);
                    using var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        InstallationDate = (reader.GetInt64(0) == 0 ? DateTime.Now : DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64(0)).ToLocalTime().Date);
                        TimeListened = reader.GetDouble(1);
                        HighestSessionTime = reader.GetDouble(2);
                        AverageSessionTime = reader.GetDouble(3);
                        TracksPlayed = reader.GetInt32(4);
                        MostListenedTrack = reader.GetString(5);
                        TotalPlaylists = reader.GetInt32(6);
                        TotalTracksInPlaylists = reader.GetInt32(7);
                        Sessions = reader.GetInt32(8);
                        MostListenedTrackPlays = reader.GetInt32(9);
                    }

                    cnn.Close();
                    IsLoaded = true;
                }

                if (InstallationDate == DateTime.MinValue)
                    InstallationDate = DateTime.Now;

                if (string.IsNullOrEmpty(MostListenedTrack))
                {
                    (string name, int plays) fetched = UtilityHandler.GetMostListenedSong();
                    MostListenedTrack = fetched.name;
                    MostListenedTrackPlays = fetched.plays;
                }

                if (TotalTracksInPlaylists == 0)
                    TotalTracksInPlaylists = UtilityHandler.GetTotalTracksInPlaylists();

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Failed to load statistics: {ex.Message} {ex.StackTrace}");
            }
        }
        public static void Save()
        {
            try
            {
                long unix_secs = ((DateTimeOffset)InstallationDate).ToUnixTimeSeconds();
                TotalPlaylists = UtilityHandler.GetPlaylistCount();

                using (SQLiteConnection cnn = new SQLiteConnection(LCS()))
                {
                    cnn.Open();

                    string sql = "UPDATE Stats SET installation_date=@I, time_listened=@TL, highest_session_time=@HST, average_session_length=@ASL, tracks_played=@TP," +
                        "most_listened_track=@MLT, total_playlists=@TLS, total_tracks_in_playlists=@TTILS, sessions=@S, most_listened_track_ammount=@MLTA";
                    using var cmd = new SQLiteCommand(sql, cnn);
                    cmd.Parameters.AddWithValue("@I", unix_secs);
                    cmd.Parameters.AddWithValue("@TL", TimeListened);
                    cmd.Parameters.AddWithValue("@HST", HighestSessionTime);
                    cmd.Parameters.AddWithValue("@ASL", AverageSessionTime);
                    cmd.Parameters.AddWithValue("@TP", TracksPlayed);
                    cmd.Parameters.AddWithValue("@MLT", MostListenedTrack);
                    cmd.Parameters.AddWithValue("@TLS", TotalPlaylists);
                    cmd.Parameters.AddWithValue("@TTILS", TotalTracksInPlaylists);
                    cmd.Parameters.AddWithValue("@S", Sessions);
                    cmd.Parameters.AddWithValue("@MLTA", MostListenedTrackPlays);

                    cmd.ExecuteNonQuery();
                    cnn.Close();
                };
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Failed to save statistics: {ex.Message}");
            }
        }
    }
}