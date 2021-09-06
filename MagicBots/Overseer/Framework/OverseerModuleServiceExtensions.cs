using MagicBots.Overseer.Framework.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimpleInjector;
using System;
using System.Linq;
using System.Reflection;

namespace MagicBots.Overseer.Framework
{
    public static class OverseerModuleServiceExtensions
    {
        public static IHostBuilder UseOverseerModules(this IHostBuilder builder, Container container)
        {
            builder.ConfigureServices((Action<HostBuilderContext, IServiceCollection>) ((context, collection) =>
            {
                var modules = Assembly.GetEntryAssembly()!.DefinedTypes
                    .Where(info =>
                        ModuleHelper.IsLoadableModule(info) || ModuleHelper.IsLoadableCooldown(info) ||
                        ModuleHelper.IsLoadableTrigger(info))
                    .Select(x => x.AsType()).ToArray();
                foreach (var module in modules)
                    container.RegisterSingleton(module, module);

                // Add required services
                container.RegisterSingleton<TriggeringService>();
            }));
            return builder;
        }
    }
}