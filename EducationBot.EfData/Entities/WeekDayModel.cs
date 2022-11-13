using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EducationBot.EfData.Entities
{
    public class WeekDay
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public DateTime Day { get; set; }

        public string DayOfWeek { get; set; }

        public int LessonId { get; set; }
        public Lesson Lesson { get; set; }

        public WeekDay(DateTime day, string dayOfWeek)
        {
            Day = day;
            DayOfWeek = dayOfWeek;
        }
    }
}
