using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DiscordBot.Bot
{
    public class BotOptions
    {
        [Required]
        [NotNull]
        public string DiscordToken { get; set; }
    }
}