using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EducationBot.EfData.Entities.Education
{
    public class Lesson
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public string? LinkToRoom { get; set; }

        public string? Place { get; set; }

        public string? Groups { get; set; }

        public int DisciplineId { get; set; }
        public Discipline Discipline { get; set; }

        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; }

        public int DisciplineTypeId { get; set; }
        public DisciplineType DisciplineType { get; set; }

        public List<LessonShedulle> Shedulles { get; set; }

        public Lesson()
        {
            Shedulles = new();
        }
    }
}
