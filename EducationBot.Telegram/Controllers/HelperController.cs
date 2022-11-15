using EducationBot.EfData;
using EducationBot.EfData.Entities;
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

            var oldLessons = await _context.Lesson.ToListAsync(ct);
            if (oldLessons.Any())
            {
                _context.Lesson.RemoveRange(_context.Lesson);
                await _context.SaveChangesAsync(ct);
            }

            await _context.Lesson.AddRangeAsync(modelList);
            await _context.SaveChangesAsync(ct);

            var newLessons = await _context.Lesson.ToListAsync(ct);
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
            List<long> sendTo = await _userChatService.GetUserChatToLessonShedule(ct);

            var nowUTC = DateTime.Now.ToUniversalTime();
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
                        sb.Append($"%E2%8C%9A через {timeDiff} мин {Environment.NewLine}");
                    else
                    if (timeDiff == 0)
                        sb.Append($"сейчас {Environment.NewLine}");

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
    }
}
