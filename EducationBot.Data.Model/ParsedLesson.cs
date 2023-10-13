namespace EducationBot.Data.Model;

public class ParsedLesson
{
    public string Discipline { get; set; }
    public ParsedLessonTeacher LessonTeacher { get; set; }
    public ParsedLessonTime LessonTime { get; set; }
    public ParsedLessonType LessonType { get; set; }
    public ParsedLessonWeekDay LessonWeekDay { get; set; }
    public string? Place { get; set; }
    public string? Groups { get; set; }
}

public class ParsedLessonType
{
    public string? TypeName { get; set; }

    public string? TypeColor { get; set; }

    public string? TypeInDoc { get; set; }

    public ParsedLessonType(string typeName, string typeColor, string typeInDoc)
    {
        TypeName = typeName;
        TypeColor = typeColor;
        TypeInDoc = typeInDoc;
    }

    public ParsedLessonType()
    {
        TypeName = null;
        TypeColor = null;
        TypeInDoc = null;
    }
}

public class ParsedLessonTeacher
{
    public string? Name { get; set; }

    public string? Link { get; set; }

    public ParsedLessonTeacher()
    {
        Name = null;
        Link = null;
    }
}

public class ParsedLessonTime
{
    public TimeSpan Start { get; set; }

    public TimeSpan End { get; set; }
}

public class ParsedLessonWeekDay
{
    public DateTime Day { get; set; }

    public string DayOfWeek { get; set; }

    public string WeekNunmber { get; set; }

    public ParsedLessonWeekDay(DateTime day, string dayOfWeek)
    {
        Day = day;
        DayOfWeek = dayOfWeek;
    }
}
