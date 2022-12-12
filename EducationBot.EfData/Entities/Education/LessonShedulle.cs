using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EducationBot.EfData.Entities.Education
{
    public class LessonShedulle
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public TimeSpan Start { get; set; }
        public DateTime StartDateTimeUTC { get; set; }

        public TimeSpan End { get; set; }
        public DateTime EndDateTimeUTC { get; set; }

        public string GetStartStr() => $"{Start.Hours}:{Start.Minutes}";

        public string GetEndStr() => $"{End.Hours}:{End.Minutes}";

        public int LessonId { get; set; }
        public Lesson Lesson { get; set; }
    }
}
