using EducationBot.EfData;
using EducationBot.EfData.Entities.Education;
using EducationBot.EfData.Model;
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
            TimeZoneInfo samaraTZI = TimeZoneInfo.CreateCustomTimeZone("Samara Time", new(4, 0, 0), "(GMT+04:00) Samara Time", "Samara Time");

            var path = Path.Combine(AppContext.BaseDirectory, "Saved.json");

            var text = System.IO.File.ReadAllText(path);
            List<ParsedLesson>? modelList = JsonConvert.DeserializeObject<List<ParsedLesson>>(text);

            if (modelList == null || !modelList.Any()) throw new Exception("Lesson is Empty");

            var teachers = modelList.Select(x => x.LessonTeacher);
            var teachersDist = teachers.GroupBy(x => x.Name).Select(x => x.First()).ToList();
            var teacherToIns = teachersDist.Select(x => new Teacher() { Link = x.Link, Name = x.Name });
            foreach (var teacher in teacherToIns)
            {
                var check = await _context.Teacher.FirstOrDefaultAsync(x => x.Name.Equals(teacher.Name), ct);
                if (check == null)
                {
                    await _context.Teacher.AddAsync(teacher, ct);
                    await _context.SaveChangesAsync(ct);
                }
            }

            var lessonTypes = modelList.Select(x => x.LessonType);
            var lessonTypesDist = lessonTypes.GroupBy(x => x.TypeName).Select(x => x.First()).ToList();
            var disciplineTypeToIns = lessonTypesDist.Select(x => new DisciplineType() { Name = x.TypeName, Color = x.TypeColor }).ToList();
            foreach (var disciplineType in disciplineTypeToIns)
            {
                var check = await _context.DisciplineType.FirstOrDefaultAsync(x => x.Name.Equals(disciplineType.Name), ct);
                if (check == null)
                {
                    await _context.DisciplineType.AddAsync(disciplineType, ct);
                    await _context.SaveChangesAsync(ct);
                }
            }

            var disciplines = modelList.Select(x => x.Discipline).Distinct();
            var disciplinesToIns = disciplines.Select(x => new Discipline() { Name = x }).ToList();
            foreach (var discipline in disciplinesToIns)
            {
                var check = await _context.Discipline.FirstOrDefaultAsync(x => x.Name.Equals(discipline.Name), ct);
                if (check == null)
                {
                    await _context.Discipline.AddAsync(discipline, ct);
                    await _context.SaveChangesAsync(ct);
                }
            }

            _context.Lesson.RemoveRange(_context.Lesson);
            await _context.SaveChangesAsync(ct);

            var groupedModel = modelList.GroupBy(x => new { x.Discipline, x.LessonType.TypeName, x.LessonTeacher.Name });
            foreach (var model in groupedModel)
            {
                List<LessonShedulle> lessonShedulle = new();
                foreach (var modelItem in model)
                {
                    var dateTimeEnd = modelItem.LessonWeekDay.Day.Add(modelItem.LessonTime.End);
                    var dateTimeStrt = modelItem.LessonWeekDay.Day.Add(modelItem.LessonTime.Start);
                    lessonShedulle.Add(new LessonShedulle()
                    {
                        Date = modelItem.LessonWeekDay.Day,
                        End = modelItem.LessonTime.End,
                        EndDateTimeUTC = TimeZoneInfo.ConvertTimeToUtc(dateTimeEnd, samaraTZI),
                        Start = modelItem.LessonTime.Start,
                        StartDateTimeUTC = TimeZoneInfo.ConvertTimeToUtc(dateTimeStrt, samaraTZI)
                    });
                }

                var teacher = await _context.Teacher.FirstOrDefaultAsync(x => x.Name.Equals(model.Key.Name), ct);
                var discipline = await _context.Discipline.FirstOrDefaultAsync(x => x.Name.Equals(model.Key.Discipline), ct);
                var disciplineType = await _context.DisciplineType.FirstOrDefaultAsync(x => x.Name.Equals(model.Key.TypeName), ct);
                Lesson studyLesson = new()
                {
                    Teacher = teacher,
                    Discipline = discipline,
                    DisciplineType = disciplineType,
                    Shedulles = lessonShedulle,
                    LinkToRoom = null,
                    Place = model.First().Place,
                    Groups = model.First().Groups,
                };
                await _context.Lesson.AddAsync(studyLesson, ct);
                await _context.SaveChangesAsync(ct);
            }
        }

        [HttpGet("lessons")]
        public async Task<IActionResult> GetLessons(CancellationToken ct = default)
        {
            var lessons = await _context.Lesson
                .Select(x => new { Discipline = x.Discipline.Name, DisciplineType = x.DisciplineType.Name, Teacher = x.Teacher.Name })
                .ToListAsync(ct);
            return Ok(lessons.OrderBy(x => x.Discipline));
        }

        [HttpGet("feet-link")]
        public async Task SetLinkToLesson([FromQuery] string discipline, [FromQuery] string typeLesson, [FromQuery] string link, CancellationToken ct = default)
        {
            var lessons = await _context.Lesson
                .Where(x => x.Discipline.Name.Trim().ToLower().Equals(discipline.Trim().ToLower())
                    && x.DisciplineType.Name.Trim().ToLower().Equals(typeLesson.Trim().ToLower()))
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
