using System.Threading;
using System.Threading.Tasks;
using DiscordBot.Bot;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DiscordBot.Main
{
    public class BotService : IHostedService
    {
        private readonly BotMain _botMain;

        public BotService(IServiceScopeFactory scopeFactory)
        {
            var scope = scopeFactory.CreateScope();
            _botMain = scope.ServiceProvider.GetRequiredService<BotMain>();
        }

        public Task StartAsync(CancellationToken _) => _botMain.StartAsync();
        public Task StopAsync(CancellationToken _) => _botMain.StopAsync();
    }
}