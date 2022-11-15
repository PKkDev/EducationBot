using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EducationBot.EfData.Entities
{
    public class LessonTeacher
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string? Link { get; set; }

        public int LessonId { get; set; }
        public Lesson Lesson { get; set; }

        public LessonTeacher()
        {
            Name = "none";
            Link = null;
        }
    }
}
