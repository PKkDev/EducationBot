using EducationBot.EfData;
using EducationBot.EfData.Entities;
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

        public async Task<List<Lesson>> GetLessonShedulled(CancellationToken ct)
        {
            var nowUTC = DateTime.Now.ToUniversalTime();
            // nowUTC = new DateTime(2022, 12, 20, 8, 5, 0, DateTimeKind.Local).ToUniversalTime();
            var today = nowUTC.Date;

            var filterEnd = nowUTC.AddMinutes(10);
            var filterStart = nowUTC.AddMinutes(-10);

            var leasons = await _context.Lesson
                .Include(x => x.Teacher)
                .Include(x => x.Day)
                .Include(x => x.Time)
                .Include(x => x.TypeLesson)
                .Where(x => (x.DateTimeStartUtc >= filterStart && x.DateTimeStartUtc <= filterEnd) && x.Day.Day == today)
                .ToListAsync(ct);

            return leasons;
        }


        public async Task<List<Lesson>> GetLessonByDay(DateTime date, CancellationToken ct)
        {
            var leasons = await _context.Lesson
                .Include(x => x.Teacher)
                .Include(x => x.Day)
                .Include(x => x.Time)
                .Include(x => x.TypeLesson)
                .Where(x => x.Day.Day == date)
                .ToListAsync(ct);

            return leasons;
        }
    }
}
