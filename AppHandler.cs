using Media_Player.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Text.Json;
using System.Drawing.Imaging;
using System.Windows;

namespace Media_Player
{
    public static class AppHandler
    {
        private static string config_path = Path.Combine(AppContext.BaseDirectory, "app_config.json");
        public static AppSettings InitSettings()
        {
            try
            {
                using (StreamReader sr = new StreamReader(config_path))
                {
                    string json_string = sr.ReadToEnd();
                    AppSettings app_settings = JsonSerializer.Deserialize<AppSettings>(json_string);
                    return app_settings;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load settings error #1: {ex.Message}");
                return new AppSettings();
            }
        }

        public static void SaveFile(AppSettings new_settings)
        {
            try
            {
                string json_text = JsonSerializer.Serialize(new_settings);
                File.WriteAllText(config_path, json_text);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load settings error #2: {ex.Message}");
            }
        }
    }
}
