using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EducationBot.EfData.EntitiesNew
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

        public List<Lesson> Lessons { get; set; }

        public Teacher()
        {
            Lessons = new();
        }
    }
}
