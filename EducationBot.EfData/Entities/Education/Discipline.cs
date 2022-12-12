using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EducationBot.EfData.Entities.Education
{
    [Index(nameof(Name), IsUnique = true)]
    public class Discipline
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public List<Lesson> StudyLesson { get; set; }

        public Discipline()
        {
            StudyLesson = new();
        }
    }
}
