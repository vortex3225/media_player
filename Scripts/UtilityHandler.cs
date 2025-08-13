using Media_Player.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media_Player.Scripts
{
    public static class UtilityHandler
    {
        public const string PLAYLIST_ITEM_SEPARATOR = "&&**&&(--"; // string which separates each file path in the playlist item string
        public const string PLAYLIST_ITEM_COUNT_SEPARATOR = "?:--+"; // string which separates the file path from the number of times it was played.

        public static string GeneratePlaylistItemString(List<string> playlist_items)
        {
            string generated = string.Empty;

            for (int i = 0; i < playlist_items.Count; i++)
            {
                string item = playlist_items[i];
                if (i <  playlist_items.Count - 1)
                {
                    generated += $"{item}{PLAYLIST_ITEM_SEPARATOR}";
                }
                else
                {
                    generated += item;
                }
            }
            Console.WriteLine(generated);
            return generated;
        }

        public static void IncrementSong(string song_name, PlaylistObject selected_playlist)
        {
            foreach (string key in selected_playlist.item_playcount.Keys)
            {
                if (key.ToLower().Contains(song_name) || key == song_name)
                {
                    int count = ++selected_playlist.item_playcount[key];
                    string converted = GeneratePlaylistItemCount(selected_playlist.item_playcount);
                    Console.WriteLine(converted);
                    PlaylistHandler.UpdatePlaylistItemCount(selected_playlist.name, converted);
                    break;
                }
            }
        }

        public static int GetSongPlays(string song_name, PlaylistObject playlist)
        {
            foreach (string key in playlist.item_playcount.Keys)
            {
                if (key.ToLower().Contains(song_name) || key == song_name)
                {
                    return playlist.item_playcount[key];
                }
            }
            return 0;
        }

        public static List<string> GeneratePlaylistItemList(string playlist_items)
        {
            List<string > generated = new List<string>();

            foreach (string s in playlist_items.Split(PLAYLIST_ITEM_SEPARATOR))
            {
                generated.Add(s);
            }

            return generated;
        }

        public static Dictionary<string, int> GeneratePlaylistItemCount(string item_counts)
        {
            Dictionary<string, int> generated = new Dictionary<string, int>();

            foreach (string s in item_counts.Split(PLAYLIST_ITEM_SEPARATOR))
            {
                string[] fetched = s.Split(PLAYLIST_ITEM_COUNT_SEPARATOR);
                string name = fetched[0];
                int count = 0;
                if (int.TryParse(fetched[1], out count))
                {
                    generated.Add(name, count);
                }
            }

            return generated;
        }

        public static string GeneratePlaylistItemCount(Dictionary<string, int> playlist_count)
        {
            string generated = string.Empty;

            int index = 0;
            foreach (KeyValuePair<string, int> kvp in playlist_count)
            {
                string name = kvp.Key;
                int value = kvp.Value;
                if (index <  playlist_count.Count - 1)
                {
                    generated += $"{name}{PLAYLIST_ITEM_COUNT_SEPARATOR}{value}{PLAYLIST_ITEM_SEPARATOR}";
                }
                else
                {
                    generated += $"{name}{PLAYLIST_ITEM_COUNT_SEPARATOR}{value}";
                }
                index++;
            }
            Console.WriteLine(generated);
            return generated;
        }
    }
}
