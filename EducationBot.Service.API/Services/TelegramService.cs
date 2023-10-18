using EducationBot.Data.Ef.Entities.Education;
using EducationBot.Data.Ef.Entities.Telegram;
using EducationBot.Service.API.Helpers;
using EducationBot.Service.API.Model;
using EducationBot.Service.API.Model.Telegram;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;

namespace EducationBot.Service.API.Services;

public class TelegramService
{
    private readonly IHttpClientFactory _httpCLientFacory;

    private readonly IHttpContextAccessor _httpContextAccessor;

    private readonly LessonHelperService _lessonHelperService;
    private readonly UserChatService _userChatService;

    private readonly string source = "https://ssau.ru/rasp?groupId=799360768";

    private readonly TelegramSettings _telegramSettings;

    public TelegramService(
        IHttpClientFactory httpCLientFacory,
        LessonHelperService lessonHelperService, UserChatService userChatService,
        IHttpContextAccessor httpContextAccessor,
        IOptions<TelegramSettings> telegramSettings)
    {
        _httpCLientFacory = httpCLientFacory;

        _httpContextAccessor = httpContextAccessor;

        _lessonHelperService = lessonHelperService;
        _userChatService = userChatService;

        _telegramSettings = telegramSettings.Value ?? throw new ArgumentNullException($"{nameof(TelegramSettings)}");
    }

    public async Task ParseTelegramMessageAsync(TelegramUpdateMessage message, CancellationToken ct)
    {
        if (message.MyChatMember != null) return;

        // бота добавили в группу при создании
        //await SendMessageToUser(message.Message.Chat.Id, "Спасибо за добавление в группу", ct);
        if (message.Message != null && message.Message.GroupChatCreated) return;

        // is_anonymous = false
        if (message.PollAnswer != null) return;

        // is_anonymous = true
        if (message.Poll != null) return;

        Chat chat;
        From from;
        string messageText;
        TelegramChatUser dialog;

        if (message.CallbackQuery != null)
        {
            chat = message.CallbackQuery.Message.Chat;
            from = message.CallbackQuery.From;
            messageText = message.CallbackQuery.Data;

            await _userChatService.SetChatUser(chat, from, ct);

            dialog = await _userChatService.GetUserDialog(chat, from, ct);

            try
            {
                switch (messageText.Trim().ToLower())
                {
                    case "подписка":
                        {
                            #region dub 2
                            var user = await _userChatService.GetUserByChatId(message.CallbackQuery.From.Id, ct);

                            StringBuilder sb = new();
                            var nowStatus = user.IsGetLessonShedulle ? "сообщения приходят" : "сообщения не приходят";
                            sb.Append($"статус: {nowStatus}");

                            var buttons = new InlineKeyboardMarkup();
                            if (user.IsGetLessonShedulle)
                                buttons.InlineKeyboard.Add(new() { new InlineKeyboardButton("%E2%AD%95 отключить", "отключить") });
                            else
                                buttons.InlineKeyboard.Add(new() { new InlineKeyboardButton("%E2%AD%90 включить", "включить") });

                            await SendMessageToUser(chat.Id, sb.ToString(), ct, null, buttons);
                            #endregion dub 2

                            break;
                        }
                    case "отключить":
                        {
                            await _userChatService.ChangeUserSubs(message.CallbackQuery.From.Id, false, ct);

                            #region dub 2
                            var user = await _userChatService.GetUserByChatId(message.CallbackQuery.From.Id, ct);

                            StringBuilder sb = new();
                            var nowStatus = user.IsGetLessonShedulle ? "сообщения приходят" : "сообщения не приходят";
                            sb.Append($"статус: {nowStatus}");

                            var buttons = new InlineKeyboardMarkup();
                            if (user.IsGetLessonShedulle)
                                buttons.InlineKeyboard.Add(new() { new InlineKeyboardButton("%E2%AD%95 отключить", "отключить") });
                            else
                                buttons.InlineKeyboard.Add(new() { new InlineKeyboardButton("%E2%AD%90 включить", "включить") });

                            await SendMessageToUser(chat.Id, sb.ToString(), ct, null, buttons);
                            #endregion dub 2

                            break;
                        }
                    case "включить":
                        {
                            await _userChatService.ChangeUserSubs(message.CallbackQuery.From.Id, true, ct);

                            #region dub 2
                            var user = await _userChatService.GetUserByChatId(message.CallbackQuery.From.Id, ct);

                            StringBuilder sb = new();
                            var nowStatus = user.IsGetLessonShedulle ? "сообщения приходят" : "сообщения не приходят";
                            sb.Append($"статус: {nowStatus}");

                            var buttons = new InlineKeyboardMarkup();
                            if (user.IsGetLessonShedulle)
                                buttons.InlineKeyboard.Add(new() { new InlineKeyboardButton("%E2%AD%95 отключить", "отключить") });
                            else
                                buttons.InlineKeyboard.Add(new() { new InlineKeyboardButton("%E2%AD%90 включить", "включить") });

                            await SendMessageToUser(chat.Id, sb.ToString(), ct, null, buttons);
                            #endregion dub 2

                            break;
                        }

                    case "напоминания":
                        {
                            var buttons = new InlineKeyboardMarkup();
                            buttons.InlineKeyboard.Add(new() { new InlineKeyboardButton("%F0%9F%93%9D создать новое", "создать_новое_напоминание") });
                            buttons.InlineKeyboard.Add(new() { new InlineKeyboardButton("%F0%9F%93%A6 архив", "архив_напоминаний"), new InlineKeyboardButton("%E2%9D%8C удалить все", "удалить_все_напоминания") });
                            await SendMessageToUser(chat.Id, "раздел - напоминания", ct, null, buttons);
                            break;
                        }
                    case "создать_новое_напоминание":
                        {
                            StringBuilder sb = new();
                            sb.AppendLine("введине дату(UTC)");
                            sb.AppendLine($"пример: {DateTime.Now.ToUniversalTime():dd.MM.yy HH:mm} - сейчас в UTC");
                            await SendMessageToUser(chat.Id, sb.ToString(), ct);

                            dialog.LastAction = ChatAction.AddReminderDate;
                            await _userChatService.UpdateUserDialog(dialog, ct);
                            break;
                        }
                    case "архив_напоминаний":
                        {
                            var shedullers = await _userChatService.GetAllUserShedullers(message.CallbackQuery.From.Id, ct);
                            StringBuilder sb = new();
                            sb.AppendLine($"архив напоминаний {Environment.NewLine}");
                            if (!shedullers.Any())
                                sb.AppendLine("у вас нет напоминаний");
                            else
                            {
                                foreach (var shedulle in shedullers.OrderByDescending(x => x.DateTimeUtc))
                                {
                                    var logo = shedulle.IsOld ? "%E2%9D%84" : "%F0%9F%94%A5";
                                    sb.Append($"{shedulle.Title} {Environment.NewLine}");
                                    sb.Append($"\uD83D\uDD51 {shedulle.DateTimeUtc} {logo} {Environment.NewLine}");
                                    sb.Append($"{Environment.NewLine}");
                                }
                            }
                            await SendMessageToUser(chat.Id, sb.ToString(), ct);
                            break;
                        }
                    case "удалить_все_напоминания":
                        {
                            var count = await _userChatService.DeleteAllUserShedullers(message.CallbackQuery.From.Id, ct);
                            StringBuilder sb = new();
                            await SendMessageToUser(chat.Id, "%E2%9C%85 готово", ct);
                            break;
                        }


                    case "расписание":
                        {
                            var buttons = new InlineKeyboardMarkup();
                            buttons.InlineKeyboard.Add(new() { new InlineKeyboardButton("%F0%9F%93%8C сегодня", "сегодня") });
                            buttons.InlineKeyboard.Add(new() { new InlineKeyboardButton("%F0%9F%94%BD вчера", "вчера"), new InlineKeyboardButton("%F0%9F%94%BC завтра", "завтра") });
                            await SendMessageToUser(chat.Id, "раздел - расписание", ct, null, buttons);
                            break;
                        }
                    case "вчера":
                        {
                            var day = DateTime.Today.AddDays(-1);
                            var lessons = await _lessonHelperService.GetLessonByDay(day, ct);
                            await SendLessons(chat.Id, lessons, day, ct);
                            break;
                        }
                    case "завтра":
                        {
                            var day = DateTime.Today.AddDays(1);
                            var lessons = await _lessonHelperService.GetLessonByDay(day, ct);
                            await SendLessons(chat.Id, lessons, day, ct);
                            break;
                        }
                    case "сегодня":
                        {
                            var day = DateTime.Today;
                            var lessons = await _lessonHelperService.GetLessonByDay(day, ct);
                            await SendLessons(chat.Id, lessons, day, ct);
                            break;
                        }

                    default:
                        {
                            break;
                        }
                }
                return;
            }
            catch (Exception e)
            {
                await SendMessageToUser(chat.Id, e.Message, ct);
                throw;
            }
        }

        // single: Chat.Id = From.Id
        if (message.Message != null)
        {
            // добавление в чат
            if (message.Message.NewChatMember != null)
            {
                //if (message.Message.NewChatMember.Username == _configuration["TelegramSettings:BotUserName"])
                if (message.Message.NewChatMember.Username == _telegramSettings.BotUserName)
                {
                    // бота добавили в чат
                }
                else
                {
                    // кого-то добавили в чат
                    await SendMessageToUser(message.Message.Chat.Id, $"{message.Message.NewChatMember.FirstName}, привет", ct);
                }
                return;
            }

            // исключение из чата
            if (message.Message.LeftChatMember != null)
            {
                //if (message.Message.LeftChatMember.Username == _configuration["TelegramSettings:BotUserName"])
                if (message.Message.LeftChatMember.Username == _telegramSettings.BotUserName)
                {
                    // бота исключли из чата
                }
                else
                {
                    // кого-то исключли из чата
                    await SendMessageToUser(message.Message.Chat.Id, $"{message.Message.LeftChatMember.FirstName}, пока", ct);
                }
                return;
            }

            // private - group
            if (message.Message.Text != null)
            {
                chat = message.Message.Chat;
                from = message.Message.From;
                messageText = message.Message.Text;

                await _userChatService.SetChatUser(chat, from, ct);

                dialog = await _userChatService.GetUserDialog(chat, from, ct);

                //if (messageText.StartsWith('/') && messageText.Contains("@") && messageText.Contains(_configuration["TelegramSettings:BotUserName"]))
                if (messageText.StartsWith('/') && messageText.Contains("@") && messageText.Contains(_telegramSettings.BotUserName))
                    messageText = messageText.Split('@').First();

                if (dialog.LastAction == ChatAction.AddReminderDate)
                {
                    try
                    {
                        //string[] formats = new string[] { "dd.MM.yy HH:mm", "dd.MM.yyyy HH:mm" };
                        //CultureInfo provider = new CultureInfo("ru-RU");
                        //var date = DateTime.ParseExact(messageText, formats, provider);

                        var date = Convert.ToDateTime(messageText.Trim());

                        var dateUtc = DateTime.SpecifyKind(date, DateTimeKind.Utc);
                        await _userChatService.AddUserSheduller(dialog, dateUtc, ct);
                    }
                    catch (Exception e)
                    {
                        StringBuilder sb = new();
                        sb.AppendLine("некорректный формат - попробуйте снова");
                        sb.AppendLine($"пример: {DateTime.Now.ToUniversalTime():dd.MM.yy HH:mm} - сейчас в UTC");
                        await SendMessageToUser(chat.Id, sb.ToString(), ct);
                        return;
                    }

                    await SendMessageToUser(chat.Id, "введине описание", ct);

                    dialog.LastAction = ChatAction.AddReminderDesc;
                    await _userChatService.UpdateUserDialog(dialog, ct);
                    return;
                }

                if (dialog.LastAction == ChatAction.AddReminderDesc)
                {
                    var description = messageText;
                    await _userChatService.FeetUserSheduller(dialog, description, ct);

                    await SendMessageToUser(chat.Id, "%E2%9C%85 готово", ct);

                    dialog.LastAction = ChatAction.None;
                    await _userChatService.UpdateUserDialog(dialog, ct);
                    return;
                }

                try
                {
                    switch (messageText.Trim().ToLower())
                    {
                        case "/start":
                            {
                                #region dub start
                                StringBuilder sb = new();
                                //sb.Append($"Вас приветствует {_configuration["TelegramSettings:BotFirstName"]} {Environment.NewLine}");
                                sb.Append($"Вас приветствует {_telegramSettings.BotFirstName} {Environment.NewLine}");
                                sb.Append($"бот создан для помощи с расписанием в институте для группы 6132-020402D {Environment.NewLine}"); ;
                                sb.Append($"<a href='{source}'>источник расписания</a> {Environment.NewLine}");
                                await SendMessageToUser(chat.Id, sb.ToString(), ct, "HTML");
                                #endregion dub start

                                #region dub menu
                                StringBuilder sb1 = new();
                                sb1.Append("доступные команды:");
                                var buttons = new InlineKeyboardMarkup();
                                buttons.InlineKeyboard.Add(new() { new InlineKeyboardButton("подписка", "подписка") });
                                buttons.InlineKeyboard.Add(new() { new InlineKeyboardButton("расписание", "расписание") });
                                buttons.InlineKeyboard.Add(new() { new InlineKeyboardButton("напоминания", "напоминания") });
                                await SendMessageToUser(chat.Id, sb1.ToString(), ct, null, buttons);
                                #endregion dub menu

                                break;
                            }
                        case "/help":
                            {
                                #region dub start
                                StringBuilder sb = new();
                                //sb.Append($"Вас приветствует {_configuration["TelegramSettings:BotFirstName"]} {Environment.NewLine}");
                                sb.Append($"Вас приветствует {_telegramSettings.BotFirstName} {Environment.NewLine}");
                                sb.Append($"бот создан для помощи с расписанием в институте для группы 6132-020402D {Environment.NewLine}"); ;
                                sb.Append($"<a href='{source}'>источник расписания</a> {Environment.NewLine}");
                                await SendMessageToUser(chat.Id, sb.ToString(), ct, "HTML");
                                #endregion dub start

                                #region dub menu
                                StringBuilder sb1 = new();
                                sb1.Append("доступные команды:");
                                var buttons = new InlineKeyboardMarkup();
                                buttons.InlineKeyboard.Add(new() { new InlineKeyboardButton("подписка", "подписка") });
                                buttons.InlineKeyboard.Add(new() { new InlineKeyboardButton("расписание", "расписание") });
                                buttons.InlineKeyboard.Add(new() { new InlineKeyboardButton("напоминания", "напоминания") });
                                await SendMessageToUser(chat.Id, sb1.ToString(), ct, null, buttons);
                                #endregion dub menu

                                break;
                            }
                        case "/menu":
                            {
                                #region dub menu
                                StringBuilder sb = new();
                                sb.Append("доступные команды:");
                                var buttons = new InlineKeyboardMarkup();
                                buttons.InlineKeyboard.Add(new() { new InlineKeyboardButton("подписка", "подписка") });
                                buttons.InlineKeyboard.Add(new() { new InlineKeyboardButton("расписание", "расписание") });
                                buttons.InlineKeyboard.Add(new() { new InlineKeyboardButton("напоминания", "напоминания") });
                                await SendMessageToUser(chat.Id, sb.ToString(), ct, null, buttons);
                                #endregion dub menu

                                break;
                            }
                        case "/chatinfo":
                            {
                                StringBuilder sb = new();

                                if (message.Message.Chat.Type == "group")
                                    sb.Append($"Group: {message.Message.Chat.Title} {Environment.NewLine}");
                                else
                                    sb.Append($"First Name: {message.Message.From.FirstName} {Environment.NewLine}");
                                sb.Append($"Chat Type: {message.Message.Chat.Type} {Environment.NewLine}");
                                sb.Append($"Chat Id: {message.Message.Chat.Id} {Environment.NewLine}");
                                await SendMessageToUser(chat.Id, sb.ToString(), ct);
                                break;
                            }
                        case "/todaylessons":
                            {
                                var day = DateTime.Today;
                                var lessons = await _lessonHelperService.GetLessonByDay(day, ct);
                                await SendLessons(chat.Id, lessons, day, ct);
                                break;
                            }
                        case "подписка":
                            {
                                #region dub 2
                                var user = await _userChatService.GetUserByChatId(message.Message.From.Id, ct);

                                StringBuilder sb = new();
                                var nowStatus = user.IsGetLessonShedulle ? "сообщения приходят" : "сообщения не приходят";
                                sb.Append($"статус: {nowStatus}");

                                var buttons = new InlineKeyboardMarkup();
                                if (user.IsGetLessonShedulle)
                                    buttons.InlineKeyboard.Add(new() { new InlineKeyboardButton("%E2%AD%95 отключить", "отключить") });
                                else
                                    buttons.InlineKeyboard.Add(new() { new InlineKeyboardButton("%E2%AD%90 включить", "включить") });

                                await SendMessageToUser(chat.Id, sb.ToString(), ct, null, buttons);
                                #endregion dub 2

                                break;
                            }

                        case "расписание":
                            {
                                var buttons = new InlineKeyboardMarkup();
                                buttons.InlineKeyboard.Add(new() { new InlineKeyboardButton("%F0%9F%93%8C сегодня", "сегодня") });
                                buttons.InlineKeyboard.Add(new() { new InlineKeyboardButton("%F0%9F%94%BD вчера", "вчера"), new InlineKeyboardButton("%F0%9F%94%BC завтра", "завтра") });
                                await SendMessageToUser(chat.Id, "раздел - расписание", ct, null, buttons);
                                break;
                            }
                        case "вчера":
                            {
                                var day = DateTime.Today.AddDays(-1);
                                var lessons = await _lessonHelperService.GetLessonByDay(day, ct);
                                await SendLessons(chat.Id, lessons, day, ct);
                                break;
                            }
                        case "завтра":
                            {
                                var day = DateTime.Today.AddDays(1);
                                var lessons = await _lessonHelperService.GetLessonByDay(day, ct);
                                await SendLessons(chat.Id, lessons, day, ct);
                                break;
                            }
                        case "сегодня":
                            {
                                var day = DateTime.Today;
                                var lessons = await _lessonHelperService.GetLessonByDay(day, ct);
                                await SendLessons(chat.Id, lessons, day, ct);
                                break;
                            }

                        default:
                            {
                                break;
                            }
                    }
                }
                catch (Exception e)
                {
                    await SendMessageToUser(chat.Id, e.Message, ct);
                    throw;
                }
                return;
            }
        }
    }

    private async Task SendLessons(long chatId, List<LessonShedulle> lessons, DateTime date, CancellationToken ct)
    {
        StringBuilder sb = new();
        sb.AppendLine($"%F0%9F%93%86 {date:dddd, dd MMMM}");
        sb.Append($"{Environment.NewLine}");
        if (lessons.Any())
        {
            foreach (var lesson in lessons)
            {
                sb.Append($"\uD83C\uDF93 {lesson.Lesson.Discipline.Name} {Environment.NewLine}");
                sb.Append($"%F0%9F%92%BB {lesson.Lesson.DisciplineType.Name} {Environment.NewLine}");
                sb.Append($"\uD83D\uDD51 {lesson.GetStartStr()} - {lesson.GetEndStr()} {Environment.NewLine}");
                if (lesson.Lesson.LinkToRoom != null)
                    sb.Append($"%F0%9F%9A%AA  <a href='{lesson.Lesson.LinkToRoom}'>Перейти в конференцию</a> {Environment.NewLine}");
                if (lesson.Lesson.Teacher.Link != null)
                    sb.Append($"%F0%9F%91%A4  <a href='{lesson.Lesson.Teacher.Link}'>{lesson.Lesson.Teacher.Name}</a> {Environment.NewLine}");
                else
                    sb.Append($"%F0%9F%91%A4 {lesson.Lesson.Teacher.Name} {Environment.NewLine}");

                if (!string.IsNullOrEmpty(lesson.Lesson.Groups))
                {
                    sb.Append($"%F0%9F%93%A6 группы: {Environment.NewLine}");
                    sb.Append(lesson.Lesson.Groups);
                }

                sb.Append($"{Environment.NewLine}");
            }
        }
        else
        {
            sb.Append($"пар нет");
        }
        await SendMessageToUser(chatId, sb.ToString(), ct, "HTML");
    }

    private async Task SendWelcomeMessage(int chatId)
    {

        var request = _httpContextAccessor.HttpContext.Request;
        var location = new Uri($"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}");

        var filePath = Path.Combine(request.Host.Value, "Resources", "Images", "help_logo_150_143.jpg");

        var client = _httpCLientFacory.CreateClient();

        var text = @"Вас приветствует бот%0D%0AЭксперементальный бот, напсианный на ASP .NET от microsoft.%0D%0AЧто умею делать:";

        var buttons = new InlineKeyboardMarkup();
        List<InlineKeyboardButton> row1 = new() { new InlineKeyboardButton("Сгенерировать число", "сгенерировать число") };
        List<InlineKeyboardButton> row2 = new() { new InlineKeyboardButton("Дай картинку", "дай картинку") };
        List<InlineKeyboardButton> row3 = new() { new InlineKeyboardButton("Chat info", "Chat info") };
        List<InlineKeyboardButton> row4 = new() { new InlineKeyboardButton("Автор", null, "https://github.com/PKkDev") };
        buttons.InlineKeyboard.Add(row1);
        buttons.InlineKeyboard.Add(row2);
        buttons.InlineKeyboard.Add(row3);
        buttons.InlineKeyboard.Add(row4);

        Dictionary<string, string> queryParam = new();
        queryParam["chat_id"] = $"{chatId}";
        queryParam["caption"] = $"{text}";
        queryParam["photo"] = $"{filePath}";
        queryParam["reply_markup"] = JsonConvert.SerializeObject(buttons);

        //var urlBase = $"{_configuration["TelegramSettings:BaseUrl"]}/sendPhoto";
        var urlBase = $"{_telegramSettings.BaseUrl}/sendPhoto";
        var url = UrlHelper.GetUriWithQueryString(urlBase, queryParam);

        var response = await client.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            var responseMessage = await response.Content.ReadAsStringAsync();
        }
    }

    private async Task SendUserKeyBoardButtons(int chatId)
    {
        var client = _httpCLientFacory.CreateClient();

        var buttons = new ReplyKeyboardMarkup();
        List<KeyboardButton> row1 = new() { new KeyboardButton("сгенерировать число") };
        List<KeyboardButton> row2 = new() { new KeyboardButton("дай картинку") };
        List<KeyboardButton> row3 = new() { new KeyboardButton("сhat info") };
        buttons.Keyboard.Add(row1);
        buttons.Keyboard.Add(row2);
        buttons.Keyboard.Add(row3);

        Dictionary<string, string> queryParam = new()
        {
            ["chat_id"] = $"{chatId}",
            ["text"] = $"Что прикажешь, хозяин?",
            ["reply_markup"] = JsonConvert.SerializeObject(buttons)
        };

        //var urlBase = $"{_configuration["TelegramSettings:BaseUrl"]}/sendMessage";
        var urlBase = $"{_telegramSettings.BaseUrl}/sendMessage";
        var url = UrlHelper.GetUriWithQueryString(urlBase, queryParam);

        var response = await client.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            var responseMessage = await response.Content.ReadAsStringAsync();
        }
    }

    private async Task SendPhotoToUser(long chatId, string caption, string imageUrl)
    {
        var client = _httpCLientFacory.CreateClient();

        Dictionary<string, string> queryParam = new();
        queryParam["chat_id"] = $"{chatId}";
        queryParam["caption"] = $"{caption}";
        queryParam["photo"] = $"{imageUrl}";

        //var urlBase = $"{_configuration["TelegramSettings:BaseUrl"]}/sendPhoto";
        var urlBase = $"{_telegramSettings.BaseUrl}/sendPhoto";
        var url = UrlHelper.GetUriWithQueryString(urlBase, queryParam);

        var response = await client.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            var responseMessage = await response.Content.ReadAsStringAsync();
        }
    }

    public async Task SendMessageToUser(long chatId, string message, CancellationToken ct, string? parseMode = null, object? replyMarkup = null)
    {
        var client = _httpCLientFacory.CreateClient();

        Dictionary<string, string> queryParam = new();
        queryParam["chat_id"] = $"{chatId}";
        queryParam["text"] = $"{message}";

        if (replyMarkup != null)
            queryParam["reply_markup"] = JsonConvert.SerializeObject(replyMarkup);

        if (parseMode != null)
            queryParam["parse_mode"] = $"{parseMode}";

        //var urlBase = $"{_configuration["TelegramSettings:BaseUrl"]}/sendMessage";
        var urlBase = $"{_telegramSettings.BaseUrl}/sendMessage";
        var url = UrlHelper.GetUriWithQueryString(urlBase, queryParam);

        var response = await client.GetAsync(url, ct);

        if (!response.IsSuccessStatusCode)
        {
            var responseMessage = await response.Content.ReadAsStringAsync(ct);
            throw new Exception(responseMessage);
        }
    }

    #region send poll

    public async Task SendPollAsync(int chatId, string question, List<string> options)
    {
        var client = _httpCLientFacory.CreateClient();

        Dictionary<string, string> queryParam = new();
        queryParam["chat_id"] = $"{chatId}";
        queryParam["question"] = $"{question}";
        queryParam["options"] = JsonConvert.SerializeObject(options);
        queryParam["is_anonymous"] = $"{false}";

        //var urlBase = $"{_configuration["TelegramSettings:BaseUrl"]}/sendPoll";
        var urlBase = $"{_telegramSettings.BaseUrl}/sendPoll";
        var url = UrlHelper.GetUriWithQueryString(urlBase, queryParam);

        var response = await client.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            var responseMessage = await response.Content.ReadAsStringAsync();
        }
    }

    #endregion send poll

    #region bot commands

    public async Task SetBotCommandsAsync(
        List<TelegramBotCommand> commands, TelegramBotCommandScope? scope = null, CancellationToken ct = default)
    {
        var client = _httpCLientFacory.CreateClient();

        Dictionary<string, string> queryParam = new();
        if (scope != null)
            queryParam["scope"] = JsonConvert.SerializeObject(scope);
        queryParam["commands"] = JsonConvert.SerializeObject(commands);

        //var urlBase = $"{_configuration["TelegramSettings:BaseUrl"]}/setMyCommands";
        var urlBase = $"{_telegramSettings.BaseUrl}/setMyCommands";
        var url = UrlHelper.GetUriWithQueryString(urlBase, queryParam);

        var response = await client.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            throw new Exception(await response.Content.ReadAsStringAsync(ct));
    }

    #endregion bot commands

    #region web hook

    public async Task SetBotWebhookAsync(string webhook, CancellationToken ct)
    {
        var client = _httpCLientFacory.CreateClient();

        Dictionary<string, string> queryParam = new()
        {
            ["url"] = $"{webhook}"
        };

        //var urlBase = $"{_configuration["TelegramSettings:BaseUrl"]}/setWebhook";
        var urlBase = $"{_telegramSettings.BaseUrl}/setWebhook";
        var url = UrlHelper.GetUriWithQueryString(urlBase, queryParam);

        var response = await client.GetAsync(url, ct);

        if (!response.IsSuccessStatusCode)
            throw new Exception(await response.Content.ReadAsStringAsync(ct));
    }

    #endregion web hook
}
