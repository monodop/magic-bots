using MagicBots.Overseer.DankDitties;
using MagicBots.Overseer.Framework.Discord;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Threading.Tasks;
using SimpleInjector;

namespace MagicBots.Overseer
{
    class Program
    {
        static async Task Main()
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

                        options.AddLogging();
                    });
                })
                .UseConsoleLifetime()
                .Build()
                .UseSimpleInjector(container);
            
            // Verify
            container.Verify();

            await host.RunAsync();
        }
    }
}
