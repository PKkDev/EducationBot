using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EducationBot.EfData.Entities
{
    public class LessonWeekDay
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public DateTime Day { get; set; }

        public string DayOfWeek { get; set; }

        public int LessonId { get; set; }
        public Lesson Lesson { get; set; }

        public LessonWeekDay(DateTime day, string dayOfWeek)
        {
            Day = day;
            DayOfWeek = dayOfWeek;
        }
    }
}
