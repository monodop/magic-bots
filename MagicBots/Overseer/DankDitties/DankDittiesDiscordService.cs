using Discord;
using Discord.WebSocket;
using MagicBots.Overseer.Framework.Discord;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MagicBots.Overseer.DankDitties
{
    public class DankDittiesDiscordService : DiscordService
    {
        protected override string ConfigSectionName => "DiscordDankDitties";

        public DankDittiesDiscordService(ILogger logger, IConfiguration configuration) : base(logger, configuration)
        {}
        
        protected override DiscordSocketConfig BuildSettings(IConfiguration configuration)
        {
            var settings = base.BuildSettings(configuration);

            settings.LogLevel = LogSeverity.Debug;
            settings.AlwaysDownloadUsers = true;
            settings.MessageCacheSize = 200;

            return settings;
        }
    }
}