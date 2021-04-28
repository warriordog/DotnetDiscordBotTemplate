using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DiscordBot.Bot.Commands
{
    public class HelloCommandModule : BaseCommandModule
    {
        private readonly ILogger<HelloCommandModule> _logger;
        private readonly IOptions<BotOptions> _botOptions;

        public HelloCommandModule(ILogger<HelloCommandModule> logger, IOptions<BotOptions> botOptions)
        {
            _logger = logger;
            _botOptions = botOptions;
        }


        [Command("hello")]
        [RequirePermissions(Permissions.SendMessages)]
        public async Task HelloCommand(CommandContext ctx)
        {
            // Group all logs generated as a result of this command
            using (_logger.BeginScope($"HelloCommand@{ctx.Message.Id.ToString()}"))
            {
                try
                {
                    _logger.LogDebug("Invoked by [{user}]", ctx.User);
                    
                    // Skip DMs if they are not enabled
                    if (ctx.Channel.IsPrivate && !_botOptions.Value.AllowDMs)
                    {
                        _logger.LogDebug("Skipping - this is a DM and they are not enabled.");
                        return;
                    }
                    
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