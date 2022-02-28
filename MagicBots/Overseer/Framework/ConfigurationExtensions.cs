using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace MagicBots.Overseer.Framework
{
    public static class ConfigurationExtensions
    {
        public static T? GetFromConfig<T>(this IConfiguration config, string sectionName, ILogger? logger)
        {
            var foundValue = config.GetSection(sectionName).Get<T>();
            if (foundValue == null && logger != null)
                logger.LogWarning($"Could not find config section '{sectionName}'");
            return foundValue;
        }
    }
}