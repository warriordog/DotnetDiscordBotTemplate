using System.Threading.Tasks;
using DiscordBot.Bot;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DiscordBot.Main
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            // Create environment
            using var host = CreateHost(args);

            // Run application
            await host.RunAsync();
        }

        private static IHost CreateHost(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices(
                    (ctx, services) =>
                    {
                        // Inject config
                        services.AddOptions<BotOptions>()
                            .Bind(ctx.Configuration.GetSection("BotOptions"))
                            .ValidateDataAnnotations();
                        services.AddOptions<DiscordAuthOptions>()
                            .Bind(ctx.Configuration.GetSection("DiscordAuth"))
                            .ValidateDataAnnotations();

                        // Inject main app logic
                        services.AddScoped<BotMain>();
                        services.AddHostedService<BotService>();
                    }
                )
                .Build();
        }
    }
}