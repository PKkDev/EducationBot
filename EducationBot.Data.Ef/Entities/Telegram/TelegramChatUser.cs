using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EducationBot.Data.Ef.Entities.Telegram;

[Table("telegram_chat_user")]
public class TelegramChatUser
{
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; set; }

    [Column("telegram_chat_id")]
    public int TelegramChatId { get; set; }
    public TelegramChat TelegramChat { get; set; }

    [Column("telegram_user_id")]
    public int TelegramUserId { get; set; }
    public TelegramUser TelegramUser { get; set; }

    [Column("last_action")]
    public ChatAction LastAction { get; set; }

    public TelegramChatUser()
    {
        LastAction = ChatAction.None;
    }
}

public enum ChatAction
{
    None,
    AddReminderDate,
    AddReminderDesc
};

