using EducationBot.Telegram.Model.Telegram;
using EducationBot.Telegram.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EducationBot.Telegram.Controllers
{
    [Route("telegram")]
    [ApiController]
    public class WebHookController : ControllerBase
    {
        private readonly TelegramService _telegramService;

        public WebHookController(TelegramService service)
        {
            _telegramService = service;
        }

        [HttpPost("hook")]
        public async Task ParsTelegram(
            [FromBody] dynamic update, CancellationToken ct = default)
        {
            var updateStr = update.ToString();
            var message = JsonConvert.DeserializeObject<TelegramUpdateMessage>(updateStr);
            await _telegramService.ParseTelegramMessageAsync(message, ct);
            GC.Collect(GC.MaxGeneration);
        }

        [HttpGet("set-commands")]
        public async Task SetBotCommands(CancellationToken ct = default)
        {
            List<TelegramBotCommand> privateCommands = new()
            {
                new TelegramBotCommand("help", "view help information"),
                new TelegramBotCommand("menu", "view menu"),
                new TelegramBotCommand("chatinfo", "get chat info"),
                new TelegramBotCommand("todaylessons", "get today lesson info")
            };
            var privateScope = new TelegramBotCommandScope("all_private_chats");
            await _telegramService.SetBotCommandsAsync(privateCommands, privateScope, ct);

            List<TelegramBotCommand> groupCommands = new()
            {
                new TelegramBotCommand("help", "view help information"),
                new TelegramBotCommand("menu", "view menu"),
                new TelegramBotCommand("chatinfo", "get chat info"),
                new TelegramBotCommand("todaylessons", "get today lesson info")
            };
            var groupScope = new TelegramBotCommandScope("all_group_chats");
            await _telegramService.SetBotCommandsAsync(groupCommands, groupScope, ct);
        }

        /// <summary>
        /// send telegrm message
        /// </summary>
        /// <param name="chanel"></param>
        /// <param name="message"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpGet("send-message")]
        public async Task SendMessage(
            [FromQuery] string chanel, [FromQuery] string message, CancellationToken ct = default)
            => await _telegramService.SendMessageToUser(Convert.ToInt32(chanel), message, ct);
    }
}
