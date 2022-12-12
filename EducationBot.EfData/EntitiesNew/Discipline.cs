using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EducationBot.EfData.EntitiesNew
{
    [Index(nameof(Name), IsUnique = true)]
    public class Discipline
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public List<Lesson> Lessons { get; set; }

        public Discipline()
        {
            Lessons = new();
        }
    }
}
