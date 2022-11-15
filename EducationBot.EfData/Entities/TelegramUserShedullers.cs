using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EducationBot.EfData.Entities
{
    public class TelegramUserShedullers
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public DateTime DateTimeUtc { get; set; }

        public string Title { get; set; }

        public bool IsOld => DateTimeUtc < DateTime.UtcNow;

        public int TelegramUserId { get; set; }
        public TelegramUser TelegramUser { get; set; }
    }
}
