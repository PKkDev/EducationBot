using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EducationBot.EfData.Entities.Cient
{
    [Index((nameof(ChatIdent)), IsUnique = true)]
    public class TelegramChat
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public string? Title { get; set; }

        public string ChatType { get; set; }

        public long ChatIdent { get; set; }

        public List<TelegramChatUser> ChatUsers { get; set; }
        public List<TelegramUser> Users { get; set; }

        public TelegramChat()
        {
            ChatUsers = new();
            Users = new();
        }
    }
}
