using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MagicBots.Overseer.Framework.Discord
{
    public abstract class DiscordClientHostedService : IHostedService
    {
        private readonly string _token;
        protected DiscordSocketClient Client;
        protected ILogger Logger { get; }
        protected IConfiguration Configuration { get; }

        public DiscordClientHostedService(ILogger logger, IConfiguration configuration)
        {
            Logger = logger;
            Configuration = configuration;

            // TODO: error handling
            var c = configuration.GetSection(ConfigSectionName);
            _token = c["token"];
            Client = new DiscordSocketClient(BuildSettings(c));
            Client.Log += OnLog;
        }

        protected virtual Task OnLog(LogMessage arg)
        {
            var logLevel = arg.Severity switch
            {
                LogSeverity.Verbose => LogLevel.Trace,
                LogSeverity.Debug => LogLevel.Debug,
                LogSeverity.Info => LogLevel.Information,
                LogSeverity.Warning => LogLevel.Warning,
                LogSeverity.Error => LogLevel.Error,
                LogSeverity.Critical => LogLevel.Critical,
                _ => LogLevel.Information,
            };
            Logger.Log(logLevel, $"{ConfigSectionName}: {arg.Source}: {arg.Message} {arg.Exception}");
            return Task.FromResult(0);
        }

        protected abstract string ConfigSectionName { get; }
        protected abstract Task ConfigureClientAsync(DiscordSocketClient client);
        protected virtual DiscordSocketConfig BuildSettings(IConfiguration configuration)
        {
            return new DiscordSocketConfig()
            {
            };
        }

        public virtual async Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation($"Discord.NET hosted service `{ConfigSectionName}` is starting");

            try
            {
                // TODO: cancellation token
                await ConfigureClientAsync(Client);
                await Client.LoginAsync(TokenType.Bot, _token);
                await Client.StartAsync();
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
                await Client.StopAsync();
            }
            catch (OperationCanceledException)
            {
                Logger.LogWarning($"Discord.NET client `{ConfigSectionName}` could not be stopped within the given timeout and may have permanently deadlocked");
            }
        }
    }
}
