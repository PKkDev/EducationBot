using Newtonsoft.Json;

namespace EducationBot.Telegram.Model.Telegram
{
    public class TelegramUpdateMessage
    {
        [JsonProperty("update_id")]
        public long UpdateId { get; set; }

        [JsonProperty("my_chat_member")]
        public MyChatMember MyChatMember { get; set; }

        [JsonProperty("message")]
        public Message? Message { get; set; }

        [JsonProperty("poll")]
        public Poll? Poll { get; set; }

        [JsonProperty("poll_answer")]
        public PollAnswer? PollAnswer { get; set; }

        [JsonProperty("callback_query")]
        public CallbackQuery? CallbackQuery { get; set; }
    }

    public class MyChatMember
    {
        [JsonProperty("chat")]
        public Chat Chat { get; set; }

        [JsonProperty("from")]
        public From From { get; set; }

        [JsonProperty("old_chat_member")]
        public ChatMember OldChatMember { get; set; }

        [JsonProperty("new_chat_member")]
        public ChatMember NewChatMember { get; set; }
    }
    public class ChatMember
    {
        [JsonProperty("user")]
        public User User { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }
    public class User
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("is_bot")]
        public bool IsBot { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }
    }

    public class Poll
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("question")]
        public string Question { get; set; }

        [JsonProperty("options")]
        public List<PollOptions> Options { get; set; }

        [JsonProperty("total_voter_count")]
        public int TotalVoterCount { get; set; }

        [JsonProperty("is_closed")]
        public bool IsClosed { get; set; }

        [JsonProperty("is_anonymous")]
        public bool IsAnonymous { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

    }
    public class PollOptions
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("voter_count")]
        public int VoterCount { get; set; }

    }

    public class PollAnswer
    {
        [JsonProperty("poll_id")]
        public string PollId { get; set; }

        [JsonProperty("user")]
        public From User { get; set; }

        [JsonProperty("option_ids")]
        public List<long> OptionIds { get; set; }

    }

    public class Message
    {
        [JsonProperty("message_id")]
        public long MessageId { get; set; }

        [JsonProperty("from")]
        public From From { get; set; }

        [JsonProperty("chat")]
        public Chat Chat { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("group_chat_created")]
        public bool GroupChatCreated { get; set; }

        [JsonProperty("new_chat_participant")]
        public ChatParticipant NewChatParticipant { get; set; }

        [JsonProperty("new_chat_member")]
        public ChatParticipant NewChatMember { get; set; }

        [JsonProperty("new_chat_members")]
        public List<ChatParticipant> NewChatMembers { get; set; }

        [JsonProperty("left_chat_participant")]
        public ChatParticipant LeftChatParticipant { get; set; }

        [JsonProperty("left_chat_member")]
        public ChatParticipant LeftChatMember { get; set; }
    }

    public class From
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("is_bot")]
        public bool IsBot { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }
    }

    public class Chat
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public class ChatParticipant
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("is_bot")]
        public bool IsBot { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }
    }

    public class CallbackQuery
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("from")]
        public From From { get; set; }

        [JsonProperty("message")]
        public Message Message { get; set; }

        [JsonProperty("chat_instance")]
        public string ChatInstance { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; }

    }
}
