using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MagicBots.Overseer.Framework.Discord
{
    public abstract class DiscordService
    {
        private readonly string _token;
        public readonly DiscordSocketClient? Client;

        public DiscordService(ILogger logger, IConfiguration configuration)
        {
            Logger = logger;
            Configuration = configuration;

            // TODO: error handling
            var c = configuration.GetSection(ConfigSectionName);
            _token = c["token"];
            var enabled = !c.GetValue<bool>("disabled");

            if (!enabled)
                return;

            Client = new DiscordSocketClient(BuildSettings(c));
            Client.Log += OnLog;
        }

        protected ILogger Logger { get; }
        protected IConfiguration Configuration { get; }
        protected abstract string ConfigSectionName { get; }
        
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
                _ => LogLevel.Information
            };
            Logger.Log(logLevel, $"{ConfigSectionName}: {arg.Source}: {arg.Message} {arg.Exception}");
            return Task.FromResult(0);
        }
        
        protected virtual DiscordSocketConfig BuildSettings(IConfiguration configuration)
        {
            return new DiscordSocketConfig();
        }

        public async Task LoginAsync()
        {
            if (Client == null)
            {
                return;
            }
            await Client.LoginAsync(TokenType.Bot, _token);
        }

        public async Task StartAsync()
        {
            if (Client == null)
            {
                return;
            }
            await Client.StartAsync();
        }

        public async Task StopAsync()
        {
            if (Client == null)
            {
                return;
            }
            await Client.StopAsync();
        }
    }
}