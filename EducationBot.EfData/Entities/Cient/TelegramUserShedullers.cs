using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EducationBot.EfData.Entities.Cient
{
    [Index(nameof(TelegramUserId), nameof(DateTimeUtc), IsUnique = true)]
    public class TelegramUserShedullers
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public DateTime DateTimeUtc { get; set; }

        public string? Title { get; set; }

        [NotMapped]
        public bool IsOld => DateTimeUtc < DateTime.UtcNow;

        public int TelegramUserId { get; set; }
        public TelegramUser TelegramUser { get; set; }

    }
}
