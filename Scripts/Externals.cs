using Media_Player.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        private static async void WriteHeader(StreamWriter streamWriter)
        {
            await streamWriter.WriteLineAsync($"[EXPORT HEADER: VERSION={SYSTEM_VERSION}, EXPORTED AT={DateTime.Now.ToString("dd MM yyyy @ HH:mm")}");
            await streamWriter.WriteLineAsync();
            await streamWriter.WriteLineAsync();
        }

        private static async void WriteFileContents(PlaylistObject playlist, StreamWriter writer, bool save_play_counts)
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

        public static async Task<bool> Export(ExportParams exportParams)
        {
            try
            {

                if (exportParams.single_file)
                {
                    using (var file_stream = MakeFile(exportParams.export_path))
                    using (var writer = new StreamWriter(file_stream))
                    {
                        WriteHeader(writer);
                        foreach (PlaylistObject playlist in exportParams.export_list)
                            WriteFileContents(playlist, writer, exportParams.save_play_counts);
                    }
                }
                else
                {
                    foreach (PlaylistObject playlist in exportParams.export_list)
                    {
                        using (var file_stream = MakeFile(exportParams.export_path))
                        using (var writer = new StreamWriter(file_stream))
                        {
                            WriteHeader(writer);
                            WriteFileContents(playlist, writer, exportParams.save_play_counts);
                        }
                    }

                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not export --> {ex.StackTrace}");
                return false;
            }
        }

        
    }
}