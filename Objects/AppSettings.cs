using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media_Player.Objects
{
    public class AppSettings
    {
        public bool resume_on_enter { get; set; } = false;
        public bool save_files { get; set; } = false;
        public string theme { get; set; } = "light";
        public bool drp { get; set; } = true;
    }
}
