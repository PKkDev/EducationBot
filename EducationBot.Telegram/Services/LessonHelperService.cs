using EducationBot.EfData;
using EducationBot.EfData.Entities.Education;
using Microsoft.EntityFrameworkCore;

namespace EducationBot.Telegram.Services
{
    public class LessonHelperService
    {
        private readonly DataBaseContext _context;

        public LessonHelperService(DataBaseContext context)
        {
            _context = context;
        }

        public async Task<List<LessonShedulle>> GetLessonShedulled(CancellationToken ct)
        {
            var nowUTC = DateTime.Now.ToUniversalTime();
            // nowUTC = new DateTime(2022, 12, 13, 13, 25, 0, DateTimeKind.Local).ToUniversalTime();
            var today = nowUTC.Date;

            var filterEnd = nowUTC.AddMinutes(10);
            var filterStart = nowUTC.AddMinutes(-10);

            var shedullers = await _context.LessonShedulle
                .Include(x => x.Lesson)
                .ThenInclude(x => x.Discipline)
                .Include(x => x.Lesson)
                .ThenInclude(x => x.Teacher)
                .Include(x => x.Lesson)
                .ThenInclude(x => x.DisciplineType)
                .Where(x => (x.StartDateTimeUTC >= filterStart && x.StartDateTimeUTC <= filterEnd) && x.Date == today)
                .ToListAsync(ct);

            return shedullers;
        }


        public async Task<List<LessonShedulle>> GetLessonByDay(DateTime date, CancellationToken ct)
        {
            var shedullers = await _context.LessonShedulle
                .Include(x => x.Lesson)
                .ThenInclude(x => x.Discipline)
                .Include(x => x.Lesson)
                .ThenInclude(x => x.Teacher)
                .Include(x => x.Lesson)
                .ThenInclude(x => x.DisciplineType)
                .Where(x => x.Date == date)
                .ToListAsync(ct);

            return shedullers;
        }
    }
}
