using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EducationBot.Data.Ef.Entities.Telegram;

[Table("telegram_user")]
[Index((nameof(UserIdent)), IsUnique = true)]
public class TelegramUser
{
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; set; }

    [Column("first_name")]
    public string? FirstName { get; set; }

    [Column("last_name")]
    public string? LastName { get; set; }

    [Column("user_ident")]
    public long UserIdent { get; set; }

    [Column("is_get_lesson_shedulle")]
    public bool IsGetLessonShedulle { get; set; }

    public List<TelegramChatUser> ChatUsers { get; set; }
    public List<TelegramChat> Chats { get; set; }

    public List<TelegramUserShedullers> Shedullers { get; set; }

    public TelegramUser()
    {
        IsGetLessonShedulle = true;
        ChatUsers = new();
        Chats = new();

        Shedullers = new();
    }
}

