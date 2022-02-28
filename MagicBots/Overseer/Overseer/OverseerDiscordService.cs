using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MagicBots.Overseer.Framework.Discord
{
    public class OverseerDiscordService : DiscordService
    {
        protected override string ConfigSectionName => "DiscordOverseer";

        public OverseerDiscordService(ILogger logger, IConfiguration configuration) : base(logger, configuration)
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