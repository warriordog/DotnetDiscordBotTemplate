using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using DiscordBot.Bot.Commands;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DiscordBot.Bot
{

    /// <summary>
    /// Discord authentication options
    /// </summary>
    public class DiscordAuthOptions
    {
        /// <summary>
        /// Discord API authentication token
        /// </summary>
        [Required]
        [NotNull]
        public string DiscordToken { get; init; }
    }
    
    /// <summary>
    /// Configuration options for the bot
    /// </summary>
    public class BotOptions
    {
        /// <summary>
        /// List of prefixes for discord commands
        /// </summary>
        [MinLength(1)]
        [NotNull]
        public IEnumerable<string> CommandPrefixes { get; init; } 
    }

    public class BotMain
    {
        private readonly DiscordClient        _discord;
        private readonly ILogger<BotMain>     _logger;

        public BotMain(IServiceProvider serviceProvider, ILoggerFactory loggerFactory, IOptions<BotOptions> botOptions, IOptions<DiscordAuthOptions> authOptions ,ILogger<BotMain> logger)
        {
            _logger = logger;

            // Create discord client
            _discord = new DiscordClient(
                new DiscordConfiguration
                {
                    Token = authOptions.Value.DiscordToken,
                    TokenType = TokenType.Bot,
                    LoggerFactory = loggerFactory,
                    Intents = DiscordIntents.Guilds | DiscordIntents.DirectMessages | DiscordIntents.GuildMessages,
                }
            );

            // Register commands
            _discord.UseCommandsNext(
                    new CommandsNextConfiguration
                    {
                        StringPrefixes = botOptions.Value.CommandPrefixes,
                        Services = serviceProvider
                    }
                )
                .RegisterCommands<HelloCommandModule>();
            
            // Log when bot joins a server.
            _discord.GuildCreated += (_, e) =>
            {
                _logger.LogInformation($"Joined server {e.Guild.Id} ({e.Guild.Name})");
                return Task.CompletedTask;
            };
        }
        
        public async Task StartAsync()
        {
            _logger.LogInformation($"Discord bot {GetType().Assembly.GetName().Version} starting");
            await _discord.ConnectAsync();
        }

        public async Task StopAsync()
        {
            _logger.LogInformation("Discord bot stopping");
            await _discord.DisconnectAsync();
        }
    }
}