using Discord;
using Discord.WebSocket;
using MagicBots.Overseer.Framework.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace MagicBots.Overseer.Framework.Discord
{
    public class OverseerHostedService : DiscordClientHostedService
    {
        private readonly TriggeringService _triggeringService;

        public OverseerHostedService(ILogger logger, IConfiguration configuration,
            TriggeringService triggeringService) : base(logger, configuration)
        {
            _triggeringService = triggeringService;
        }

        protected override string ConfigSectionName => "DiscordOverseer";

        private async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            // Ignore system messages, or messages from other bots
            // TODO: Consider making this part of a trigger configurable per trigger somehow
            if (!(rawMessage is SocketUserMessage message))
                return;
            if (message.Source != MessageSource.User)
                return;

            // TODO: Investigate if this will cause deadlocks
            var context = new DiscordTriggerContext(Client!, message);
            await _triggeringService.ProcessMessage(context);
        }

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
            if (Client == null)
                return Task.CompletedTask;

            Client!.MessageReceived += MessageReceivedAsync;

            return Task.CompletedTask;
        }
    }
}