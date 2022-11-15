using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EducationBot.EfData.Entities
{
    public class LessonType
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }

        public string? Color { get; set; }

        public string? TypeStr { get; set; }

        public int LessonId { get; set; }
        public Lesson Lesson { get; set; }

        public LessonType()
        {
            Title = "none";
            Color = null;
            TypeStr = null;
        }

        public LessonType(string title, string color, string typeStr)
        {
            Title = title;
            Color = color;
            TypeStr = typeStr;
        }
    }
}
