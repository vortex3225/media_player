using Media_Player.Objects;
using Media_Player.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static Media_Player.Scripts.Externals;

namespace Media_Player.Scripts
{
    public static class Externals
    {
        private const string SYSTEM_VERSION = "1.0.0";

        public const string PLAYLIST_EXPORT_FILE_EXTENSION = ".mp";
        public const string STATISTICS_EXPORT_FILE_EXTENSION = ".mps";

        public struct ExportParams(string p1, bool p2, bool save_count_param, List<PlaylistObject> p3 )
        {
            public List<PlaylistObject> export_list = new List<PlaylistObject>(p3);
            public bool single_file = p2;
            public bool save_play_counts = save_count_param;
            public string export_path = p1;
        }

        private static FileStream MakeFile(string export_path)
        {
            // export_path is a directory path so we scan every file in the directory to increment the file name
            string file_path = Path.Combine(export_path, $"mp_export{PLAYLIST_EXPORT_FILE_EXTENSION}");
            int tries = 0;
            while (File.Exists(file_path))
            {
                tries++;
                file_path = Path.Combine(export_path, $"mp_export_{tries}{PLAYLIST_EXPORT_FILE_EXTENSION}");
            }

            return File.Open(file_path, FileMode.CreateNew);
        }

        private static async Task WriteHeader(StreamWriter streamWriter)
        {
            try
            {
                await streamWriter.WriteLineAsync($"[EXPORT HEADER: VERSION={SYSTEM_VERSION}, EXPORTED AT={DateTime.Now.ToString("dd MM yyyy @ HH:mm")}]");
                await streamWriter.WriteLineAsync();
                await streamWriter.WriteLineAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not write header --> {ex.StackTrace} --- {ex.Message}");
            }
        }

        private static async Task WriteFileContents(PlaylistObject playlist, StreamWriter writer, bool save_play_counts)
        {
            try
            {
                await writer.WriteLineAsync($"[{playlist.name}]");
                await writer.WriteLineAsync();
                await writer.WriteLineAsync();

                foreach (KeyValuePair<string, int> counts in playlist.item_playcount)
                {
                    string to_write = $"{counts.Key} | ";
                    if (save_play_counts) to_write += counts.Value.ToString();
                    else to_write += "0";

                    await writer.WriteLineAsync(to_write);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not write file contents --> {ex.StackTrace} --- {ex.Message}");
            }
        }

        public static async Task<bool> Export(ExportParams exportParams)
        {
            try
            {
                if (exportParams.export_list.Count == 0) throw new Exception("Cannot export empty list");

                if (exportParams.single_file)
                {
                    using (var file_stream = MakeFile(exportParams.export_path))
                    using (var writer = new StreamWriter(file_stream))
                    {
                        await WriteHeader(writer);
                        foreach (PlaylistObject playlist in exportParams.export_list)
                           await WriteFileContents(playlist, writer, exportParams.save_play_counts);
                    }
                }
                else
                {
                    foreach (PlaylistObject playlist in exportParams.export_list)
                    {
                        using (var file_stream = MakeFile(exportParams.export_path))
                        using (var writer = new StreamWriter(file_stream))
                        {
                           await WriteHeader(writer);
                           await WriteFileContents(playlist, writer, exportParams.save_play_counts);
                        }
                    }

                }

                MessageBox.Show($"Successfully exported: {exportParams.export_list.Count} playlists to {exportParams.export_path}", $"Export successfully", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not export --> {ex.StackTrace} --- {ex.Message}");
                return false;
            }
        }

        public static async Task<bool> ExportStatistics(string export_path)
        {
            try
            {
                string name = $"mp_statistics";
                string file_path = Path.Combine(export_path, $"{name}{STATISTICS_EXPORT_FILE_EXTENSION}");

                int tries = 0;
                while (File.Exists(file_path))
                {
                    tries++;
                    file_path = Path.Combine(export_path, $"{name}_{tries}{STATISTICS_EXPORT_FILE_EXTENSION}");
                }

                string export_contents = 
                   $"""
                   [EXPORT HEADER: VERSION={SYSTEM_VERSION}, EXPORTED AT={DateTime.Now.ToString("dd MM yyyy @ HH:mm")}]

                   install_date= {((DateTimeOffset)StatisticsObject.InstallationDate).ToUnixTimeSeconds()}
                   time_listened= {StatisticsObject.TimeListened}
                   highest_s_time= {StatisticsObject.HighestSessionTime}
                   average_s_time= {StatisticsObject.AverageSessionTime}
                   sessions= {StatisticsObject.Sessions}
                   tracks_played= {StatisticsObject.TracksPlayed}
                   most_listened_track= {StatisticsObject.MostListenedTrack}
                   most_listened_t_plays= {StatisticsObject.MostListenedTrackPlays}
                   total_playlists= {StatisticsObject.TotalPlaylists}
                   total_tracks_playlists= {StatisticsObject.TotalTracksInPlaylists}
                   """;

                await File.WriteAllTextAsync(file_path, export_contents);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not export statistics --> {ex.StackTrace} --- {ex.Message}");
                return false;
            }
        }

        public static async Task<bool> ImportStatistics(string file_path)
        {
            try
            {
                if (!File.Exists(file_path)) throw new FileNotFoundException($"Could not find statistics file to import --> {file_path}");

                using (var fileStream = File.OpenRead(file_path))
                using (var reader = new StreamReader(fileStream))
                {
                    string? line = await reader.ReadLineAsync();
                    while (!reader.EndOfStream)
                    {
                        if (string.IsNullOrEmpty(line) || line.Contains("[EXPORT HEADER"))
                        {
                            line = await reader.ReadLineAsync();
                            continue;
                        }

                        string[] split = line.Split("=");

                        if (split.Length == 2)
                        {
                            split[0] = split[0].Trim();
                            split[1] = split[1].Trim();

                            switch (split[0])
                            {
                                case "install_date":
                                    // MessageBox.Show(split[0]);
                                    StatisticsObject.InstallationDate = DateTimeOffset.FromUnixTimeSeconds(Int64.Parse(split[1])).LocalDateTime;
                                    break;
                                case "time_listened":
                                    StatisticsObject.TimeListened = double.Parse(split[1]);
                                    break;
                                case "highest_s_time":
                                    StatisticsObject.HighestSessionTime = double.Parse(split[1]);
                                    break;
                                case "average_s_time":
                                    StatisticsObject.AverageSessionTime = double.Parse(split[1]);
                                    break;
                                case "sessions":
                                    StatisticsObject.Sessions = int.Parse(split[1]);
                                    break;
                                case "tracks_played":
                                    StatisticsObject.TracksPlayed = int.Parse(split[1]);
                                    break;
                                case "most_listened_track":
                                    StatisticsObject.MostListenedTrack = split[1];
                                    break;
                                case "most_listened_t_plays":
                                    StatisticsObject.MostListenedTrackPlays = int.Parse(split[1]);
                                    break;
                                case "total_playlists":
                                    StatisticsObject.TotalPlaylists = int.Parse(split[1]);
                                    break;
                                case "total_tracks_playlists":
                                    StatisticsObject.TotalTracksInPlaylists = int.Parse(split[1]);
                                    break;
                            }
                        }

                        line = await reader.ReadLineAsync();
                    }

                    StatisticsObject.Save();
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not import statistics --> {ex.StackTrace} --- {ex.Message}");
                return false;
            }
        }

        public static async Task<bool> Import(string[] file_paths)
        {
            try
            {
                Dictionary<string, string> renamed = new Dictionary<string, string>();
                List<PlaylistObject> playlists_ToSave = new List<PlaylistObject>();

                foreach (string file_path in file_paths)
                {
                    if (!File.Exists(file_path)) throw new FileNotFoundException($"Could not find the specified file to import --> {file_path}");

                    using (var fileStream = File.OpenRead(file_path))
                    using (var reader = new StreamReader(fileStream))
                    {
                        string current_playlist_name = string.Empty;
                        List<string> items = new List<string>();
                        Dictionary<string, int> plays = new Dictionary<string, int>();

                        string? line = await reader.ReadLineAsync();
                        while (true)
                        {

                            if (reader.EndOfStream)
                            {
                                if (items.Count > 0 && !string.IsNullOrEmpty(current_playlist_name) && plays.Count > 0)
                                    playlists_ToSave.Add(new PlaylistObject(current_playlist_name, items, plays));
                                
                                break;
                            }

                            if (string.IsNullOrEmpty(line) || line.Contains("[EXPORT"))
                            {
                                line = await reader.ReadLineAsync();
                                continue;
                            }

                            if (line.StartsWith("[") && line.EndsWith("]"))
                            {
                                if (!string.IsNullOrEmpty(current_playlist_name))
                                {                                    
                                    playlists_ToSave.Add(new PlaylistObject(current_playlist_name, new List<string>(items), new Dictionary<string, int>(plays)));
                                    items.Clear();
                                    plays.Clear();
                                }

                                current_playlist_name = line.Substring(1, line.Length - 2);
                            } 
                            else if (!string.IsNullOrEmpty(line))
                            {
                                string[] split = line.Split(" | ");
                                if (split.Length == 2)
                                {
                                    items.Add(split[0]);
                                    plays.TryAdd(split[0], int.Parse(split[1]));
                                }
                            }
                            line = await reader.ReadLineAsync();
                        }
                    }
                }

                foreach (PlaylistObject playlist in playlists_ToSave)
                {
                    int tries = 0;
                    string original = playlist.name;
                    while (PlaylistHandler.NameAlreadyExists(playlist.name))
                    {
                        tries++;
                        playlist.name = $"{original}_{tries}";
                    }
                    if (original != playlist.name) renamed.Add(original, playlist.name);

                    PlaylistHandler.SavePlaylist(playlist);
                    // Console.WriteLine($"saved {playlist.name} | {playlist.playlist_items.Count} | {playlist.item_playcount.Count}");
                }

                string f = string.Empty;
                foreach (KeyValuePair<string, string> kvp in renamed)
                {
                    f += $"RENAMED {kvp.Key} TO {kvp.Value}\n";
                }
                if (!string.IsNullOrEmpty(f)) MessageBox.Show(f, "Solved naming conflicts...", MessageBoxButton.OK, MessageBoxImage.Information);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Something went wrong while attempting to import file --> {ex.StackTrace} --- {ex.Message}");
                return false;
            }
        }
    }
}