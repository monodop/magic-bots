using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SimpleInjector;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MagicBots.Overseer.Framework.Discord
{
    public abstract class DiscordClientHostedService : IHostedService
    {
        public readonly DiscordService DiscordService;

        public DiscordClientHostedService(ILogger logger, IConfiguration configuration, DiscordService discordService)
        {
            Logger = logger;
            Configuration = configuration;
            DiscordService = discordService;
        }

        protected ILogger Logger { get; }
        protected IConfiguration Configuration { get; }
        protected abstract string ConfigSectionName { get; }

        public virtual async Task StartAsync(CancellationToken cancellationToken)
        {
            // TODO: error handling
            var c = Configuration.GetSection(ConfigSectionName);
            var enabled = !c.GetValue<bool>("disabled");

            if (!enabled || DiscordService.Client == null)
                return;

            Logger.LogInformation($"Discord.NET hosted service `{ConfigSectionName}` is starting");

            try
            {
                // TODO: cancellation token
                await ConfigureClientAsync(DiscordService.Client);
                await DiscordService.LoginAsync();
                await DiscordService.StartAsync();
            }
            catch (OperationCanceledException)
            {
                Logger.LogWarning("Startup has been aborted. Exiting...");
            }
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation($"Discord.NET hosted service `{ConfigSectionName}` is stopping");
            try
            {
                // TODO: cancellation token
                await DiscordService.StopAsync();
            }
            catch (OperationCanceledException)
            {
                Logger.LogWarning(
                    $"Discord.NET client `{ConfigSectionName}` could not be stopped within the given timeout and may have permanently deadlocked");
            }
        }

        protected abstract Task ConfigureClientAsync(DiscordSocketClient client);
    }
}