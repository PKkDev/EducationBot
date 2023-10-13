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
    public DateTime Date { get; set; }

    [Column("start")]
    public TimeSpan Start { get; set; }
    [Column("start_utc")]
    public DateTime StartDateTimeUTC { get; set; }

    [Column("end")]
    public TimeSpan End { get; set; }
    [Column("starend_utc")]
    public DateTime EndDateTimeUTC { get; set; }

    public string GetStartStr() => $"{Start.Hours}:{Start.Minutes}";

    public string GetEndStr() => $"{End.Hours}:{End.Minutes}";

    public int LessonId { get; set; }
    public Lesson Lesson { get; set; }
}
