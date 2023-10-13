using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EducationBot.Data.Ef.Entities.Telegram;

[Table("telegram_user_shedullers")]
[Index(nameof(TelegramUserId), nameof(DateTimeUtc), IsUnique = true)]
public class TelegramUserShedullers
{
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; set; }

    [Column("date_time_utc")]
    public DateTime DateTimeUtc { get; set; }

    [Column("title")]
    public string? Title { get; set; }

    [NotMapped]
    public bool IsOld => DateTimeUtc < DateTime.UtcNow;

    [Column("telegram_user_id")]
    public int TelegramUserId { get; set; }
    public TelegramUser TelegramUser { get; set; }

}

