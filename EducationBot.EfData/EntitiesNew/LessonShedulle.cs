using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EducationBot.EfData.EntitiesNew
{
    public class LessonShedulle
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public int LessonId { get; set; }
        public Lesson Lesson { get; set; }
    }
}
