﻿using EducationBot.Data.Ef.Entities.Education;
using EducationBot.Data.Model;
using EducationBot.EfData.Context;
using EducationBot.Service.API.BackJobs;
using EducationBot.Service.API.Middleware;
using EducationBot.Service.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Runtime;

namespace EducationBot.Service.API.Controllers;

[Route("helper")]
[Authorize(Policy = "ApiKeyPolicy")]
[ApiController]
public class HelperController : ControllerBase
{
    private readonly DataBaseContext _context;
    private readonly UserChatService _userChatService;
    private readonly TelegramService _telegramService;
    private readonly LessonHelperService _lessonHelperService;

    private readonly CheckLessonWorker _checkLessonWorker;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="context"></param>
    /// <param name="userChatService"></param>
    /// <param name="telegramService"></param>
    /// <param name="lessonHelperService"></param>
    public HelperController(
        DataBaseContext context, UserChatService userChatService,
        TelegramService telegramService, LessonHelperService lessonHelperService,
        CheckLessonWorker checkLessonWorker)
    {
        _context = context;
        _userChatService = userChatService;
        _telegramService = telegramService;
        _lessonHelperService = lessonHelperService;

        _checkLessonWorker = checkLessonWorker;
    }

    [HttpGet("feet")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task FeetSavedEducationData(CancellationToken ct = default)
    {
        TimeZoneInfo samaraTZI = TimeZoneInfo.CreateCustomTimeZone("Samara Time", new(4, 0, 0), "(GMT+04:00) Samara Time", "Samara Time");

        var path = Path.Combine(AppContext.BaseDirectory, "Saved.json");

        var text = System.IO.File.ReadAllText(path);
        List<ParsedLesson>? modelList = JsonConvert.DeserializeObject<List<ParsedLesson>>(text);

        if (modelList == null || !modelList.Any())
            throw new Exception("Lesson is empty");

        var teacherToIns = modelList
            .Select(x => x.LessonTeacher)
            .GroupBy(x => x.Name)
            .Select(x => x.First())
            .Select(x => new Teacher()
            {
                Link = x.Link,
                Name = x.Name
            });

        foreach (var teacher in teacherToIns)
        {
            if (string.IsNullOrEmpty(teacher.Name))
                throw new Exception("Teacher.Name is empty");

            var check = await _context.Teacher
                .FirstOrDefaultAsync(x => x.Name.Equals(teacher.Name), ct);

            if (check == null)
            {
                await _context.Teacher.AddAsync(teacher, ct);
                await _context.SaveChangesAsync(ct);
            }
        }

        var disciplineTypeToIns = modelList
            .Select(x => x.LessonType)
            .GroupBy(x => x.TypeName).Select(x => x.First())
            .Select(x => new DisciplineType()
            {
                Name = x.TypeName,
                Color = x.TypeColor
            });

        foreach (var disciplineType in disciplineTypeToIns)
        {
            if (string.IsNullOrEmpty(disciplineType.Name))
                throw new Exception("DisciplineType.Name is empty");

            var check = await _context.DisciplineType
                .FirstOrDefaultAsync(x => x.Name.Equals(disciplineType.Name), ct);

            if (check == null)
            {
                await _context.DisciplineType.AddAsync(disciplineType, ct);
                await _context.SaveChangesAsync(ct);
            }
        }

        var disciplinesToIns = modelList
            .Select(x => x.Discipline)
            .Distinct()
            .Select(x => new Discipline(x));

        foreach (var discipline in disciplinesToIns)
        {
            var check = await _context.Discipline
                .FirstOrDefaultAsync(x => x.Name.Equals(discipline.Name), ct);

            if (check == null)
            {
                await _context.Discipline.AddAsync(discipline, ct);
                await _context.SaveChangesAsync(ct);
            }
        }

        _context.Lesson.RemoveRange(_context.Lesson);
        await _context.SaveChangesAsync(ct);

        var groupedModel = modelList
            .GroupBy(x => new
            {
                x.Discipline,
                x.LessonType.TypeName,
                x.LessonTeacher.Name,
                x.Groups
            });
        foreach (var model in groupedModel)
        {
            List<LessonShedulle> lessonShedulle = new();
            foreach (var modelItem in model)
            {
                var dateTimeEnd = modelItem.LessonWeekDay.Day.Add(modelItem.LessonTime.End);
                var dateTimeStrt = modelItem.LessonWeekDay.Day.Add(modelItem.LessonTime.Start);

                var dateTimeEndUtc = TimeZoneInfo.ConvertTimeToUtc(dateTimeEnd, samaraTZI);
                var dateTimeStrtUtc = TimeZoneInfo.ConvertTimeToUtc(dateTimeStrt, samaraTZI);

                lessonShedulle.Add(new LessonShedulle()
                {
                    Date = DateOnly.FromDateTime(modelItem.LessonWeekDay.Day),
                    End = TimeOnly.FromTimeSpan(modelItem.LessonTime.End),
                    EndDateTimeUTC = dateTimeEndUtc,
                    Start = TimeOnly.FromTimeSpan(modelItem.LessonTime.Start),
                    StartDateTimeUTC = dateTimeStrtUtc
                });
            }

            var teacher = await _context.Teacher
                .FirstOrDefaultAsync(x => x.Name.Equals(model.Key.Name), ct);

            var discipline = await _context.Discipline
                .FirstOrDefaultAsync(x => x.Name.Equals(model.Key.Discipline), ct);

            var disciplineType = await _context.DisciplineType
                .FirstOrDefaultAsync(x => x.Name.Equals(model.Key.TypeName), ct);

            if (teacher == null || discipline == null || disciplineType == null)
            {
                continue;
            }

            Lesson studyLesson = new()
            {
                Teacher = teacher,
                Discipline = discipline,
                DisciplineType = disciplineType,
                Shedulles = lessonShedulle,
                LinkToRoom = null,
                Place = model.First().Place,
                Groups = model.Key.Groups,
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
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SetLinkToLesson([FromQuery] string discipline, [FromQuery] string typeLesson, [FromQuery] string link, CancellationToken ct = default)
    {
        var lessons = await _context.Lesson
            .Where(x => x.Discipline.Name.Trim().ToLower().Equals(discipline.Trim().ToLower())
                && x.DisciplineType.Name.Trim().ToLower().Equals(typeLesson.Trim().ToLower()))
            .ToListAsync(ct);

        foreach (var lesson in lessons)
            lesson.LinkToRoom = link;

        _context.Lesson.UpdateRange(lessons);
        await _context.SaveChangesAsync(ct);

        return Ok();
    }

    [HttpGet("chats")]
    public async Task<IActionResult> GetAllChats(CancellationToken ct = default)
        => Ok(await _userChatService.GetAllChatsAsync(ct));

    [HttpGet("users")] 
    public async Task<IActionResult> GetAllUsers(CancellationToken ct = default)
        => Ok(await _userChatService.GetAllUserAsync(ct));

    [HttpGet("check-lesson")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CheckLesson(CancellationToken ct = default)
    {
        await _checkLessonWorker.DoWork(ct);
        return Ok();
    }
}
