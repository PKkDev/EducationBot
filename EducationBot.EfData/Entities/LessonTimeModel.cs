using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EducationBot.EfData.Entities
{
    public class LessonTime
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public TimeSpan Start { get; set; }

        public TimeSpan End { get; set; }

        public int LessonId { get; set; }
        public Lesson Lesson { get; set; }

        public string GetStartStr() => $"{Start.Hours}:{Start.Minutes}";

        public string GetEndStr() => $"{End.Hours}:{End.Minutes}";
    }
}
