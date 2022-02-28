using MagicBots.Overseer.DankDitties;
using MagicBots.Overseer.Framework;
using MagicBots.Overseer.Framework.Services;
using MagicBots.Overseer.Modules.Acromean;
using MagicBots.Overseer.Overseer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using SimpleInjector;
using System.Threading.Tasks;

namespace MagicBots.Overseer
{
    internal static class Program
    {
        private static async Task Main()
        {
            var container = new Container();

            //Log is available everywhere, useful for places where it isn't practical to use ILogger injection
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            //CreateDefaultBuilder configures a lot of stuff for us automatically.
            //See: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-5.0#default-builder-settings
            var host = Host.CreateDefaultBuilder()
                .UseSerilog()
                .ConfigureServices((context, services) =>
                {
                    services.AddSimpleInjector(container, options =>
                    {
                        options.AddHostedService<OverseerHostedService>();
                        options.AddHostedService<DankDittiesHostedService>();
                        
                        // Register help services
                        container.RegisterSingleton<FirestoreService>();

                        options.AddLogging();
                    });
                    
                    services.AddHttpClient();
                })
                .UseOverseerModules(container)
                .UseConsoleLifetime()
                .Build()
                .UseSimpleInjector(container);

            // Verify
            container.Verify();

            await host.RunAsync();
        }
    }
}