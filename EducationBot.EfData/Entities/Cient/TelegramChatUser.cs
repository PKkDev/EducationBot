using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EducationBot.EfData.Entities.Cient
{
    public class TelegramChatUser
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public int TelegramChatId { get; set; }
        public TelegramChat TelegramChat { get; set; }

        public int TelegramUserId { get; set; }
        public TelegramUser TelegramUser { get; set; }

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
}
