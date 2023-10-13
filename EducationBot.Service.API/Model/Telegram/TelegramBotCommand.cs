using Newtonsoft.Json;

namespace EducationBot.Telegram.Model.Telegram
{
    public class TelegramBotCommand
    {
        [JsonProperty("command")]
        public string Command { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        public TelegramBotCommand(string command, string description)
        {
            Command = command;
            Description = description;
        }
    }

    /// <summary>
    /// BotCommandScopeDefault - default
    /// </summary>
    public class TelegramBotCommandScope
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        public TelegramBotCommandScope(string type)
        {
            Type = type;
        }
    }
}
