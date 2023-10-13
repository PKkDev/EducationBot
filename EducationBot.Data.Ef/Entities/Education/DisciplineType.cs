using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EducationBot.Data.Ef.Entities.Education;

[Table("discipline_type")]
[Index(nameof(Name), IsUnique = true)]
public class DisciplineType
{
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; }

    [Column("color")]
    public string Color { get; set; }

    public List<Lesson> StudyLesson { get; set; }

    public DisciplineType()
    {
        StudyLesson = new();
    }
}
