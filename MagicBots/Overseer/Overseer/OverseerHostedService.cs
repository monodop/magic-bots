using Discord;
using Discord.WebSocket;
using MagicBots.Overseer.Framework;
using MagicBots.Overseer.Framework.Discord;
using MagicBots.Overseer.Framework.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SimpleInjector;
using System;
using System.Threading.Tasks;

namespace MagicBots.Overseer.Overseer
{
    public class OverseerHostedService : DiscordClientHostedService
    {
        private readonly TriggeringService _triggeringService;

        public OverseerHostedService(ILogger logger, IConfiguration configuration, Container container,
            TriggeringService triggeringService) : base(logger, configuration, container)
        {
            _triggeringService = triggeringService;
        }

        protected override Type DiscordServiceType => typeof(OverseerDiscordService);
        protected override string ConfigSectionName => "DiscordOverseer";
        
        protected override Task ConfigureClientAsync(DiscordSocketClient client)
        {
            if (DiscordService.Client == null)
                return Task.CompletedTask;

            DiscordService.Client!.MessageReceived += MessageReceivedAsync;

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            // Ignore system messages, or messages from other bots
            // TODO: Consider making this part of a trigger configurable per trigger somehow
            if (!(rawMessage is SocketUserMessage message))
                return;
            if (message.Source != MessageSource.User)
                return;

            // TODO: Investigate if this will cause deadlocks
            var context = new DiscordTriggerContext(DiscordService.Client!, message);
            await _triggeringService.ProcessMessage(context);
        }
    }
}