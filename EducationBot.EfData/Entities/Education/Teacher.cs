using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EducationBot.EfData.Entities.Education
{
    [Index(nameof(Name), IsUnique = true)]
    public class Teacher
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string? Link { get; set; }

        public List<Lesson> StudyLesson { get; set; }

        public Teacher()
        {
            StudyLesson = new();
        }
    }
}
