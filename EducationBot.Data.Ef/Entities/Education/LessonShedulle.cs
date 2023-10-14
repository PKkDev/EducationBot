using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EducationBot.Data.Ef.Entities.Education;

[Table("lessons_shedulle")]
public class LessonShedulle
{
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; set; }

    [Column("date")]
    public DateOnly Date { get; set; }

    [Column("start")]
    public TimeOnly Start { get; set; }
    [Column("start_utc")]
    public DateTime StartDateTimeUTC { get; set; }

    [Column("end")]
    public TimeOnly End { get; set; }
    [Column("starend_utc")]
    public DateTime EndDateTimeUTC { get; set; }

    public string GetStartStr() => $"{Start.Hour}:{Start.Minute}";

    public string GetEndStr() => $"{End.Hour}:{End.Minute}";

    public int LessonId { get; set; }
    public Lesson Lesson { get; set; }
}
