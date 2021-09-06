using Discord;
using Discord.Addons.Hosting;
using Discord.WebSocket;
using MagicBots.Overseer.DankDitties;
using MagicBots.Overseer.Framework.Discord;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Net.Http;
using System.Threading.Tasks;

namespace MagicBots.Overseer
{
    class Program
    {
        static async Task Main()
        {
            //Log is available everywhere, useful for places where it isn't practical to use ILogger injection
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            //CreateDefaultBuilder configures a lot of stuff for us automatically.
            //See: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-5.0#default-builder-settings
            var hostBuilder = Host.CreateDefaultBuilder()
                .UseSerilog()
                .ConfigureServices((context, services) =>
                {
                    // Hosted services
                    services.AddHostedService<OverseerHostedService>();
                    services.AddSingleton<OverseerHostedService>();
                    services.AddHostedService<DankDittiesHostedService>();
                    services.AddSingleton<DankDittiesHostedService>();

                    // Core components
                    services.AddSingleton<HttpClient>();
                });

            await hostBuilder.RunConsoleAsync();
        }
    }
}
