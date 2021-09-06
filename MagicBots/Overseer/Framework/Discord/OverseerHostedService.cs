using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicBots.Overseer.Framework.Discord
{
    public class OverseerHostedService : DiscordClientHostedService
    {
        public OverseerHostedService(ILogger<OverseerHostedService> logger, IConfiguration configuration) : base(logger, configuration)
        {
        }

        protected override string ConfigSectionName => "DiscordOverseer";

        protected override DiscordSocketConfig BuildSettings(IConfiguration configuration)
        {
            var settings = base.BuildSettings(configuration);

            settings.LogLevel = LogSeverity.Debug;
            settings.AlwaysDownloadUsers = true;
            settings.MessageCacheSize = 200;

            return settings;
        }

        protected override Task ConfigureClientAsync(DiscordSocketClient client)
        {
            // TODO: add handlers here
            return Task.CompletedTask;
        }
    }
}
