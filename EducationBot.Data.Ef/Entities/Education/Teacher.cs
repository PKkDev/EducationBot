using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EducationBot.Data.Ef.Entities.Education;

[Table("teacher")]
[Index(nameof(Name), IsUnique = true)]
public class Teacher
{
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; }

    [Column("link")]
    public string? Link { get; set; }

    public List<Lesson> StudyLesson { get; set; }

    public Teacher()
    {
        StudyLesson = new();
    }
}
