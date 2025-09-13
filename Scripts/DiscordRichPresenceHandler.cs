using DiscordRPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media_Player.Scripts
{
    public static class DiscordRichPresenceHandler
    {
        private const string APP_ID = "1416421276700115014";
        private static DiscordRpcClient client;

        public static void InitialiseClient()
        {
            client = new DiscordRpcClient(APP_ID);
            client.Initialize();
        }

        public static (bool, string?) UpdatePresence(string status_text, string detail_text, TimeSpan timespan, ActivityType activityType, Assets assets)
        {
            try
            {
                client.Update(p =>
                {
                    p.Details = detail_text;
                    p.State = status_text;
                    p.Assets = assets;
                    p.Timestamps = Timestamps.FromTimeSpan(timespan);
                    p.Type = activityType;
                });
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public static void Dispose()
        {
            if (client != null) client.Dispose();
        }
    }
}
