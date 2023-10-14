using EducationBot.EfData.Context;
using EducationBot.Service.API.Services;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace EducationBot.Service.API.BackJobs;

public class CheckLessonWorker
{
    private readonly DataBaseContext _context;
    private readonly UserChatService _userChatService;
    private readonly TelegramService _telegramService;
    private readonly LessonHelperService _lessonHelperService;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="context"></param>
    /// <param name="userChatService"></param>
    /// <param name="telegramService"></param>
    /// <param name="lessonHelperService"></param>
    public CheckLessonWorker(
        DataBaseContext context,
        UserChatService userChatService,
        TelegramService telegramService,
        LessonHelperService lessonHelperService)
    {
        _context = context;
        _userChatService = userChatService;
        _telegramService = telegramService;
        _lessonHelperService = lessonHelperService;
    }

    public async Task DoWork(CancellationToken ct)
    {
        var nowUTC = DateTime.Now.ToUniversalTime();

        await CheckLessons(nowUTC, ct);

        await CheckUserShedulle(nowUTC, ct);
    }

    private async Task CheckLessons(DateTime nowUTC, CancellationToken ct)
    {
        List<long> sendTo = await _userChatService.GetUserChatToLessonShedule(ct);
        var leasons = await _lessonHelperService.GetLessonShedulled(ct);

        foreach (var lesson in leasons)
        {
            var time = lesson.StartDateTimeUTC - nowUTC;
            var timeDiff = (int)time.TotalMinutes;
            if (timeDiff >= 0)
            {
                StringBuilder sb = new();
                sb.Append($"\uD83C\uDF93 {lesson.Lesson.Discipline.Name} {Environment.NewLine}");

                if (lesson.Lesson.LinkToRoom != null)
                    sb.Append($"%F0%9F%9A%AA  <a href='{lesson.Lesson.LinkToRoom}'>Перейти в конференцию</a> {Environment.NewLine}");

                if (timeDiff > 0)
                    sb.Append($"\uD83D\uDD51 через {timeDiff} мин {Environment.NewLine}");
                else
                if (timeDiff == 0)
                    sb.Append($"\uD83D\uDD51 сейчас {Environment.NewLine}");

                if (lesson.Lesson.Teacher.Link != null)
                    sb.Append($"%F0%9F%91%A4  <a href='{lesson.Lesson.Teacher.Link}'>{lesson.Lesson.Teacher.Name}</a> {Environment.NewLine}");
                else
                    sb.Append($"%F0%9F%91%A4 {lesson.Lesson.Teacher.Name} {Environment.NewLine}");

                sb.Append($"%F0%9F%92%BB {lesson.Lesson.DisciplineType.Name} {Environment.NewLine}");

                foreach (var chat in sendTo)
                    await _telegramService.SendMessageToUser(chat, sb.ToString(), ct, "HTML");
            }
        }
    }

    private async Task CheckUserShedulle(DateTime nowUTC, CancellationToken ct)
    {
        var filterEnd = nowUTC.AddMinutes(10);
        var filterStart = nowUTC.AddMinutes(-10);

        var shedullers = await _context.TelegramUserShedullers
            .Include(x => x.TelegramUser)
            .Where(x => x.DateTimeUtc >= filterStart && x.DateTimeUtc <= filterEnd)
            .ToListAsync(ct);

        foreach (var shedulle in shedullers)
        {
            var time = shedulle.DateTimeUtc - nowUTC;
            var timeDiff = (int)time.TotalMinutes;
            if (timeDiff >= 0)
            {
                StringBuilder sb = new();

                sb.Append($"{shedulle.Title} {Environment.NewLine}");

                if (timeDiff > 0)
                    sb.Append($"\uD83D\uDD51 через {timeDiff} мин {Environment.NewLine}");
                else
                if (timeDiff == 0)
                    sb.Append($"\uD83D\uDD51 сейчас {Environment.NewLine}");

                await _telegramService.SendMessageToUser(shedulle.TelegramUser.UserIdent, sb.ToString(), ct, "HTML");
            }
        }
    }
}
