using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EducationBot.EfData.Entities
{
    public class Lesson
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public Teacher Teacher { get; set; }

        public DateTime DateTimeStartUtc { get; set; }

        public DateTime DateTimeEndUtc { get; set; }

        public WeekDay Day { get; set; }

        public LessonTime Time { get; set; }

        public string WeekNunmber { get; set; }

        public string Discipline { get; set; }

        public TypeLesson TypeLesson { get; set; }

        public string? Groups { get; set; }

        public string? Room { get; set; }

        public string? LinkToRoom { get; set; }

        public Lesson()
        {
            Teacher = new();
            Groups = null;
            LinkToRoom = null;
        }
    }
}
