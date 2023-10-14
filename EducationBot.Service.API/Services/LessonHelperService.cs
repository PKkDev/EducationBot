using EducationBot.Data.Ef.Entities.Education;
using EducationBot.EfData.Context;
using Microsoft.EntityFrameworkCore;

namespace EducationBot.Service.API.Services;

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
        var dateOnly = DateOnly.FromDateTime(today);

        var filterEnd = nowUTC.AddMinutes(10);
        var filterStart = nowUTC.AddMinutes(-10);

        var shedullers = await _context.LessonShedulle
            .Include(x => x.Lesson)
            .ThenInclude(x => x.Discipline)
            .Include(x => x.Lesson)
            .ThenInclude(x => x.Teacher)
            .Include(x => x.Lesson)
            .ThenInclude(x => x.DisciplineType)
            .Where(x => (x.StartDateTimeUTC >= filterStart && x.StartDateTimeUTC <= filterEnd) && x.Date == dateOnly)
            .OrderBy(x => x.StartDateTimeUTC)
            .ToListAsync(ct);

        return shedullers;
    }


    public async Task<List<LessonShedulle>> GetLessonByDay(DateTime date, CancellationToken ct)
    {
        var dateOnly = DateOnly.FromDateTime(date);

        var shedullers = await _context.LessonShedulle
            .Include(x => x.Lesson)
            .ThenInclude(x => x.Discipline)
            .Include(x => x.Lesson)
            .ThenInclude(x => x.Teacher)
            .Include(x => x.Lesson)
            .ThenInclude(x => x.DisciplineType)
            .Where(x => x.Date == dateOnly)
            .OrderBy(x => x.StartDateTimeUTC)
            .ToListAsync(ct);

        return shedullers;
    }
}
