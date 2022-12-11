using EducationBot.EfData;
using EducationBot.EfData.Entities;
using EducationBot.Telegram.Model.Telegram;
using EducationBot.Telegram.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;

namespace EducationBot.Telegram.Controllers
{
    [Route("helper")]
    [ApiController]
    public class HelperController : ControllerBase
    {
        private readonly DataBaseContext _context;
        private readonly UserChatService _userChatService;
        private readonly TelegramService _telegramService;
        private readonly LessonHelperService _lessonHelperService;

        public HelperController(
            DataBaseContext context,
            UserChatService userChatService, TelegramService telegramService, LessonHelperService lessonHelperService)
        {
            _context = context;
            _userChatService = userChatService;
            _telegramService = telegramService;
            _lessonHelperService = lessonHelperService;
        }

        [HttpGet("feet")]
        public async Task FeetSavedEducationData(CancellationToken ct = default)
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Saved.json");

            var text = System.IO.File.ReadAllText(path);
            List<Lesson>? modelList = JsonConvert.DeserializeObject<List<Lesson>>(text);

            if (modelList == null || !modelList.Any()) throw new Exception("Lesson is Empty");

            var gr = modelList.GroupBy(x => x.Teacher.Name);

            var gr2 = modelList.GroupBy(x => x.Discipline);
            var gr2L = gr2.Select(x => x.Key).Distinct().ToList();

            var gr3 = modelList.GroupBy(x => x.TypeLesson.Title);
            var gr3L = gr3.Select(x => x.Key).Distinct().ToList();

            var groups = modelList.GroupBy(x => new { x.Discipline, x.TypeLesson.Title });

            //var oldLessons = await _context.Lesson.ToListAsync(ct);
            //if (oldLessons.Any())
            //{
            //    _context.Lesson.RemoveRange(_context.Lesson);
            //    await _context.SaveChangesAsync(ct);
            //}

            //await _context.Lesson.AddRangeAsync(modelList);
            //await _context.SaveChangesAsync(ct);

            //var newLessons = await _context.Lesson.ToListAsync(ct);
        }

        [HttpGet("lessons")]
        public async Task<IActionResult> GetLessons(CancellationToken ct = default)
        {
            var lessons = await _context.Lesson
                .Select(x => new { x.Discipline, x.TypeLesson.Title })
                .ToListAsync(ct);
            return Ok(lessons.Distinct().OrderBy(x => x.Title));
        }

        [HttpGet("feet-link")]
        public async Task SetLinkToLesson([FromQuery] string discipline, [FromQuery] string typeLesson, [FromQuery] string link, CancellationToken ct = default)
        {
            var lessons = await _context.Lesson
                .Where(x => x.Discipline.Trim().ToLower().Equals(discipline.Trim().ToLower())
                    && x.TypeLesson.Title.Trim().ToLower().Equals(typeLesson.Trim().ToLower()))
                .ToListAsync(ct);

            foreach (var lesson in lessons)
                lesson.LinkToRoom = link;

            _context.Lesson.UpdateRange(lessons);
            await _context.SaveChangesAsync(ct);
        }

        [HttpGet("chats")]
        public async Task<IActionResult> GetAllChats(CancellationToken ct = default) => Ok(await _userChatService.GetAllChats(ct));


        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers(CancellationToken ct = default) => Ok(await _userChatService.GetAllUser(ct));

        [HttpGet("check-lesson")]
        public async Task CheckLesson(CancellationToken ct = default)
        {
            var nowUTC = DateTime.Now.ToUniversalTime();

            #region lessons
            await CheckLessons(nowUTC, ct);
            #endregion lessons

            #region user shedullers
            await CheckUSerSHedulle(nowUTC, ct);
            #endregion user shedullers
        }

        private async Task CheckLessons(DateTime nowUTC, CancellationToken ct)
        {
            List<long> sendTo = await _userChatService.GetUserChatToLessonShedule(ct);
            var leasons = await _lessonHelperService.GetLessonShedulled(ct);

            foreach (var lesson in leasons)
            {
                var time = lesson.DateTimeStartUtc - nowUTC;
                var timeDiff = (int)time.TotalMinutes;
                if (timeDiff >= 0)
                {
                    StringBuilder sb = new();
                    sb.Append($"\uD83C\uDF93 {lesson.Discipline} {Environment.NewLine}");

                    if (lesson.LinkToRoom != null)
                        sb.Append($"%F0%9F%9A%AA  <a href='{lesson.LinkToRoom}'>Перейти в конференцию</a> {Environment.NewLine}");

                    if (timeDiff > 0)
                        sb.Append($"\uD83D\uDD51 через {timeDiff} мин {Environment.NewLine}");
                    else
                    if (timeDiff == 0)
                        sb.Append($"\uD83D\uDD51 сейчас {Environment.NewLine}");

                    if (lesson.Teacher.Link != null)
                        sb.Append($"%F0%9F%91%A4  <a href='{lesson.Teacher.Link}'>{lesson.Teacher.Name}</a> {Environment.NewLine}");
                    else
                        sb.Append($"%F0%9F%91%A4 {lesson.Teacher.Name} {Environment.NewLine}");

                    sb.Append($"%F0%9F%92%BB {lesson.TypeLesson.Title} {Environment.NewLine}");

                    foreach (var chat in sendTo)
                        await _telegramService.SendMessageToUser(chat, sb.ToString(), ct, "HTML");
                }
            }
        }

        private async Task CheckUSerSHedulle(DateTime nowUTC, CancellationToken ct)
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
}
