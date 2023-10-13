using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EducationBot.Data.Ef.Entities.Education;

[Table("lesson")]
public class Lesson
{
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; set; }

    [Column("link_to_room")]
    public string? LinkToRoom { get; set; }

    [Column("place")]
    public string? Place { get; set; }

    [Column("groups")]
    public string? Groups { get; set; }

    [Column("discipline_id")]
    public int DisciplineId { get; set; }
    public Discipline Discipline { get; set; }

    [Column("teacher_id")]
    public int TeacherId { get; set; }
    public Teacher Teacher { get; set; }

    [Column("discipline_type_id")]
    public int DisciplineTypeId { get; set; }
    public DisciplineType DisciplineType { get; set; }

    public List<LessonShedulle> Shedulles { get; set; }

    public Lesson()
    {
        Shedulles = new();
    }
}
