using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MagicBots.Overseer.Framework
{
    public abstract class OverseerModule
    {
        protected readonly IConfiguration Config;
        protected readonly DiscordSocketClient Discord;
        protected readonly ILogger Logger;

        // TODO: Find an alternative to passing DiscordSocketClient here, (make it clear which client we're getting).
        protected OverseerModule(DiscordSocketClient discord, ILogger logger,
            IConfiguration config)
        {
            Discord = discord;
            Logger = logger;
            Config = config;
        }

        protected T? GetFromConfig<T>(string sectionName)
        {
            var foundValue = Config.GetSection(sectionName).Get<T>();
            if (foundValue == null)
                Logger.LogWarning($"Could not find config section '{sectionName}'");
            return foundValue;
        }
    }
}