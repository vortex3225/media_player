using Dapper;
using Media_Player.Objects;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
namespace Media_Player.Scripts
{
    public static class SettingsHandler
    {
        public static string LoadConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
        }

        public static List<string> GetPreviouslySavedFiles()
        {
            List<string> fetched = new List<string>();
            
            try
            {
                using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    cnn.Open();
                    string sql = "SELECT saved_files FROM Settings";
                    using var cmd = new SQLiteCommand(sql, cnn);
                    using var reader = cmd.ExecuteReader();

                    string saved_string = string.Empty;
                    while (reader.Read())
                    {
                        saved_string = reader.GetString(0);
                    }

                    fetched = saved_string.Split(UtilityHandler.PLAYLIST_ITEM_SEPARATOR).ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR LOADING SETTINS #1: {ex.Message}");
            }
            return fetched;
        }

        public static void SaveLoadedFiles(List<string> openedList)
        {
            string joined = string.Empty;
            for (int i = 0; i < openedList.Count; i++)
            {
                if (i <  openedList.Count - 1)
                {
                    joined += openedList[i] + UtilityHandler.PLAYLIST_ITEM_SEPARATOR;
                }
                else
                {
                    joined += openedList[i];
                }
            }

            try
            {
                using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    cnn.Execute("UPDATE Settings SET saved_files = @N", new { N = joined });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void SaveMediaData(string media_source_path, double media_position)
        {
            try
            {
                using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    cnn.Execute("UPDATE Settings SET current_media_source = @N, current_media_position = @P", new { N = media_source_path, P = media_position });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR LOADING SETTINS #2: {ex.Message}");
            }
        }

        public static (string, double) GetMediaData()
        {
            try
            {
                using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    cnn.Open();
                    string sqlQuery = "SELECT current_media_source, current_media_position FROM Settings";
                    using var cmd = new SQLiteCommand(sqlQuery, cnn);
                    using var reader = cmd.ExecuteReader();

                    string fetched_path = string.Empty;
                    double fetched_position = 0;

                    while (reader.Read())
                    {
                        fetched_path = reader.GetString(0);
                        fetched_position = reader.GetDouble(1);
                    }

                    return (fetched_path, fetched_position);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR LOADING SETTINS #3: {ex.Message}");
                return (string.Empty, 0);
            }
        }

        public static void Clear()
        {
            try
            {
                using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    cnn.Execute("UPDATE Settings SET saved_files=@N, current_media_source=@P, current_media_position=@C", new { N = string.Empty, P = string.Empty, C = 0 });
                }
                AppHandler.SaveFile(new AppSettings());
                MessageBox.Show("Cleared saved options.", "Successfully cleared saved options", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR CLEARING SETTINGS: {ex.Message}");
            }
        }
    }
}