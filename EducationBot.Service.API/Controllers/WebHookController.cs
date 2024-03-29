﻿using EducationBot.Service.API.Model.Telegram;
using EducationBot.Service.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EducationBot.Service.API.Controllers;

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
        var message = System.Text.Json.JsonSerializer.Deserialize<TelegramUpdateMessage>(update);
        await _telegramService.ParseTelegramMessageAsync(message, ct);
        GC.Collect(GC.MaxGeneration);
    }

    [HttpGet("set-commands")]
    [Authorize(Policy = "ApiKeyPolicy")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> SetBotCommands(CancellationToken ct = default)
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

        return Ok();
    }

    /// <summary>
    /// send telegrm message
    /// </summary>
    /// <param name="chanel"></param>
    /// <param name="message"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpGet("send-message")]
    [Authorize(Policy = "ApiKeyPolicy")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> SendMessage(
        [FromQuery] string chanel, [FromQuery] string message, CancellationToken ct = default)
    {
        await _telegramService.SendMessageToUser(Convert.ToInt32(chanel), message, ct);

        return Ok();
    }
}
