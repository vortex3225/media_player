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
    public static class PlaylistHandler
    {
        public static PlaylistObject? selected_playlist = null; 

        public static string LoadConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
        }

        public static void SavePlaylist(PlaylistObject playlist)
        {
            /*
             * playlist_items is a string separated by the follwing characters: &&**&&(-- that contains the songs/videos to be then loaded into the playlist list.
             * 
             */

            try
            {
                using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    cnn.Execute("INSERT INTO Playlists (name, playlist_items, item_playcount) VALUES (@Name, @PlaylistItems, @ItemPlaycount)", new { Name = playlist.name, PlaylistItems = UtilityHandler.GeneratePlaylistItemString(playlist.playlist_items), ItemPlaycount = UtilityHandler.GeneratePlaylistItemCount(playlist.item_playcount) });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Couldnt save "+ex.Message);
            }
        }

        public static List<PlaylistObject> GetPlaylists()
        {
            List<PlaylistObject> fetched = new List<PlaylistObject>();
            try
            {
                using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    cnn.Open();
                    string sql = $"SELECT * FROM Playlists";
                    Console.WriteLine(sql);
                    using var cmd = new SQLiteCommand(sql, cnn);
                    using var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        fetched.Add(new PlaylistObject(reader.GetString(0), UtilityHandler.GeneratePlaylistItemList(reader.GetString(1)), UtilityHandler.GeneratePlaylistItemCount(reader.GetString(2))));
                    }
                }
                return fetched;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Couldn't load " + ex.Message);
                return fetched;
            }
        }

        public static PlaylistObject ?LoadPlaylist(string playlist_name_to_load)
        {
            try
            {
                using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    cnn.Open();
                    string sql = $"SELECT * FROM Playlists WHERE name='{playlist_name_to_load}'";
                    Console.WriteLine(sql);
                    using var cmd = new SQLiteCommand(sql, cnn);
                    using var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        string name = reader.GetString(0);
                        string playlist_items_string = reader.GetString(1);
                        string item_count_string = reader.GetString(2);

                        return new PlaylistObject(name, UtilityHandler.GeneratePlaylistItemList(playlist_items_string), UtilityHandler.GeneratePlaylistItemCount(item_count_string));
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Couldn't load "+ex.Message);
                return null;
            }
        }

        public static void UpdatePlaylistItemCount(string name, string newItemCount)
        {
            try
            {
                using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    cnn.Execute("UPDATE Playlists SET item_playcount=@New WHERE name=@Name", new { New = newItemCount, Name = name });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public static List<PlaylistObject> LoadPlaylists()
        {
            try
            {
                using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    List<PlaylistObject> objs = new List<PlaylistObject>();
                    cnn.Open();
                    string sql = "SELECT * FROM Playlists";
                    using var cmd = new SQLiteCommand(sql, cnn);
                    using var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string name = reader.GetString(0);
                        Console.WriteLine(name);
                        string playlist_items = reader.GetString(1);
                        Console.WriteLine(playlist_items);
                        string item_counts = reader.GetString(2);
                        Console.WriteLine(item_counts);

                        PlaylistObject new_obj = new PlaylistObject(name, UtilityHandler.GeneratePlaylistItemList(playlist_items), UtilityHandler.GeneratePlaylistItemCount(item_counts));
                        objs.Add(new_obj);
                    }
                    return objs;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not fetch all playlists: {ex.Message}");
                return new List<PlaylistObject>();
            }
        }

        public static void DeletePlaylist(string playlist_name)
        {
            try
            {
                using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    cnn.Execute("DELETE FROM Playlists WHERE name=@Name", new { Name = playlist_name });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Couldn't delete "+ex.Message);
            }
        }

        public static bool NameAlreadyExists(string name)
        {
            try
            {
                using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    cnn.Open();
                    string sql = "SELECT 1 FROM Playlists WHERE name = @name LIMIT 1";
                    using var command = new SQLiteCommand(sql, cnn);
                    command.Parameters.AddWithValue("@name", name);

                    using var reader = command.ExecuteReader();
                    return reader.Read();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not check name: " + ex.Message);
                return true;
            }
        }
    }
}
