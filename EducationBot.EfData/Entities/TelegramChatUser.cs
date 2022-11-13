namespace EducationBot.EfData.Entities
{
    public class TelegramChatUser
    {
        public int TelegramChatId { get; set; }
        public TelegramChat TelegramChat { get; set; }

        public int TelegramUserId { get; set; }
        public TelegramUser TelegramUser { get; set; }
    }
}
