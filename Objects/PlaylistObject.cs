using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media_Player.Objects
{
    public class PlaylistObject
    {
        public string name {  get; set; }
        public List<string> playlist_items { get; set; }
        public Dictionary<string, int> item_playcount { get; set; }

        public PlaylistObject(string n, List<string> pi, Dictionary<string, int> ip)
        {
            this.name = n;
            this.playlist_items = pi;
            this.item_playcount = ip;
        }
    }
}
