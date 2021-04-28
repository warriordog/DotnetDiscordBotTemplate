using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using DiscordBot.Bot.Commands;
using DSharpPlus;
using DSharpPlus.CommandsNext;
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
        [Required]
        public IEnumerable<string> CommandPrefixes { get; init; }

        /// <summary>
        /// Flag that enables the full server member list. Off by default.
        /// Setting this to true will ensure that the full list of server members is available to your bot at all times.
        /// Please note, however, that this is a "Privileged Intent" that has special rules enforced by Discord.
        /// For this to work, you must enable the "Guild Members" intent in the Discord Developer Portal.
        /// Once your bot connects to 100 or more servers, this intent will be disabled and you will need to be verified by Discord support.
        /// </summary>
        public bool UseMemberList { get; init; } = false;

        /// <summary>
        /// Flag that enables logging the guilds that this bot joins.
        /// If true (default), then an INFORMATION log entry will be written any time that the bot joins a new server or connects to an existing one after logging in.
        /// Setting this to false can reduce network traffic if guild logging is not needed/wanted.
        /// </summary>
        public bool LogGuilds { get; init; } = true;

        /// <summary>
        /// Flag that enables interaction through DMs (Direct Messages).
        /// If true (default), then DMs will be routed to the bot's command logic.
        /// If false, then DMs are ignored and only guild (server) chat is routed to the command logic.
        /// Setting this to false can reduce network traffic if DM support is not needed/wanted.
        /// </summary>
        public bool AllowDMs { get; init; } = true;
    }

    public class BotMain
    {
        private readonly DiscordClient        _discord;
        private readonly ILogger<BotMain>     _logger;

        public BotMain(IServiceProvider serviceProvider, ILoggerFactory loggerFactory, IOptions<BotOptions> botOptions, IOptions<DiscordAuthOptions> authOptions, ILogger<BotMain> logger)
        {
            _logger = logger;

            // Create discord client
            _discord = new DiscordClient(
                new DiscordConfiguration
                {
                    Token = authOptions.Value.DiscordToken,
                    TokenType = TokenType.Bot,
                    LoggerFactory = loggerFactory,
                    Intents = _BuildRequiredIntents(botOptions.Value)
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
            
            // Log when we join servers, if enabled.
            if (botOptions.Value.LogGuilds)
            {
                // Log when we join a new server
                _discord.GuildCreated += (_, e) =>
                {
                    _logger.LogInformation($"Joined new server {e.Guild.Id} ({e.Guild.Name}).");
                    return Task.CompletedTask;
                };

                // Log when we join an existing server
                _discord.GuildAvailable += (_, e) =>
                {
                    _logger.LogInformation($"Logged into existing server {e.Guild.Id} ({e.Guild.Name}).");
                    return Task.CompletedTask;
                };
            }
            
            // Preload the members list for all servers.
            if (botOptions.Value.UseMemberList)
            {
                _discord.GuildCreated += (_, e) => e.Guild.RequestMembersAsync();
                _discord.GuildAvailable += (_, e) => e.Guild.RequestMembersAsync();
            }
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

        private static DiscordIntents _BuildRequiredIntents(BotOptions botOptions)
        {
            // We always need Guilds and GuildMessages intent to receive chat in servers.
            var intents = DiscordIntents.Guilds | DiscordIntents.GuildMessages;
            
            // We need DirectMessages intent to receive DMs.
            if (botOptions.AllowDMs) intents |= DiscordIntents.DirectMessages;
            
            // We need Guilds *and* GuildMembers intents to receive the guild members list.
            if (botOptions.UseMemberList) intents |= DiscordIntents.Guilds | DiscordIntents.GuildMembers;

            return intents;
        }
    }
}