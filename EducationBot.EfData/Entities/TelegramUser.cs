using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EducationBot.EfData.Entities
{
    [Index((nameof(UserIdent)), IsUnique = true)]
    public class TelegramUser
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public long UserIdent { get; set; }

        public bool IsGetLessonShedulle { get; set; }

        public List<TelegramChatUser> ChatUsers { get; set; }
        public List<TelegramChat> Chats { get; set; }

        public TelegramUser()
        {
            IsGetLessonShedulle = true;
            ChatUsers = new();
            Chats = new();
        }
    }
}
