using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EducationBot.Data.Ef.Entities.Telegram;

[Table("telegram_chat")]
[Index((nameof(ChatIdent)), IsUnique = true)]
public class TelegramChat
{
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; set; }

    [Column("title")]
    public string? Title { get; set; }

    [Column("chat_type")]
    public string ChatType { get; set; }

    [Column("chat_ident")]
    public long ChatIdent { get; set; }

    public List<TelegramChatUser> ChatUsers { get; set; }
    public List<TelegramUser> Users { get; set; }

    public TelegramChat()
    {
        ChatUsers = new();
        Users = new();
    }
}

