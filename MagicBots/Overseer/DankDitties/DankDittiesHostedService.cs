using Discord;
using Discord.WebSocket;
using MagicBots.Overseer.Framework.Discord;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SimpleInjector;
using System;
using System.Threading.Tasks;

namespace MagicBots.Overseer.DankDitties
{
    public class DankDittiesHostedService : DiscordClientHostedService
    {
        public DankDittiesHostedService(ILogger logger, IConfiguration configuration, Container container) : base(logger, configuration, container)
        {
        }

        protected override Type DiscordServiceType => typeof(DankDittiesDiscordService);
        protected override string ConfigSectionName => "DiscordDankDitties";
        
        protected override Task ConfigureClientAsync(DiscordSocketClient client)
        {
            // TODO: add handlers here
            return Task.CompletedTask;
        }
    }
}