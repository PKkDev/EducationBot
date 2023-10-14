using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EducationBot.Data.Ef.Entities.Education;

[Table("discipline")]
[Index(nameof(Name), IsUnique = true)]
public class Discipline
{
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; }

    public List<Lesson> StudyLesson { get; set; }

    public Discipline()
    {
        StudyLesson = new();
    }

    public Discipline(string name)
        : this()
    {
        Name = name;
    }
}
