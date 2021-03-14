using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Bot.Commands
{
    public class HelloCommandModule : BaseCommandModule
    {
        private readonly ILogger<HelloCommandModule> _logger;

        public HelloCommandModule(ILogger<HelloCommandModule> logger)
        {
            _logger = logger;
        }


        [Command("hello")]
        [RequirePermissions(Permissions.SendMessages)]
        public async Task HelloCommand(CommandContext ctx)
        {
            // Setup logging context
            using (_logger.BeginScope($"HelloCommand@{ctx.Message.Id.ToString()}"))
            {
                _logger.LogDebug("Invoked by [{user}]", ctx.User);
                
                try
                {
                    // Reply
                    await ctx.RespondAsync("Hello, world!");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Uncaught exception");
                }
            }
        }
    }
}