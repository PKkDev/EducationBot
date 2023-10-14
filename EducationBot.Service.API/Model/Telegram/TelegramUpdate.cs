using System.Text.Json.Serialization;

namespace EducationBot.Service.API.Model.Telegram;

public class TelegramUpdateMessage
{
    [JsonPropertyName("update_id")]
    public long UpdateId { get; set; }

    [JsonPropertyName("my_chat_member")]
    public MyChatMember MyChatMember { get; set; }

    [JsonPropertyName("message")]
    public Message? Message { get; set; }

    [JsonPropertyName("poll")]
    public Poll? Poll { get; set; }

    [JsonPropertyName("poll_answer")]
    public PollAnswer? PollAnswer { get; set; }

    [JsonPropertyName("callback_query")]
    public CallbackQuery? CallbackQuery { get; set; }
}

public class MyChatMember
{
    [JsonPropertyName("chat")]
    public Chat Chat { get; set; }

    [JsonPropertyName("from")]
    public From From { get; set; }

    [JsonPropertyName("old_chat_member")]
    public ChatMember OldChatMember { get; set; }

    [JsonPropertyName("new_chat_member")]
    public ChatMember NewChatMember { get; set; }
}
public class ChatMember
{
    [JsonPropertyName("user")]
    public User User { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }
}
public class User
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("is_bot")]
    public bool IsBot { get; set; }

    [JsonPropertyName("first_name")]
    public string FirstName { get; set; }

    [JsonPropertyName("username")]
    public string Username { get; set; }
}

public class Poll
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("question")]
    public string Question { get; set; }

    [JsonPropertyName("options")]
    public List<PollOptions> Options { get; set; }

    [JsonPropertyName("total_voter_count")]
    public int TotalVoterCount { get; set; }

    [JsonPropertyName("is_closed")]
    public bool IsClosed { get; set; }

    [JsonPropertyName("is_anonymous")]
    public bool IsAnonymous { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

}
public class PollOptions
{
    [JsonPropertyName("text")]
    public string Text { get; set; }

    [JsonPropertyName("voter_count")]
    public int VoterCount { get; set; }

}

public class PollAnswer
{
    [JsonPropertyName("poll_id")]
    public string PollId { get; set; }

    [JsonPropertyName("user")]
    public From User { get; set; }

    [JsonPropertyName("option_ids")]
    public List<long> OptionIds { get; set; }

}

public class Message
{
    [JsonPropertyName("message_id")]
    public long MessageId { get; set; }

    [JsonPropertyName("from")]
    public From From { get; set; }

    [JsonPropertyName("chat")]
    public Chat Chat { get; set; }

    [JsonPropertyName("text")]
    public string Text { get; set; }

    [JsonPropertyName("group_chat_created")]
    public bool GroupChatCreated { get; set; }

    [JsonPropertyName("new_chat_participant")]
    public ChatParticipant NewChatParticipant { get; set; }

    [JsonPropertyName("new_chat_member")]
    public ChatParticipant NewChatMember { get; set; }

    [JsonPropertyName("new_chat_members")]
    public List<ChatParticipant> NewChatMembers { get; set; }

    [JsonPropertyName("left_chat_participant")]
    public ChatParticipant LeftChatParticipant { get; set; }

    [JsonPropertyName("left_chat_member")]
    public ChatParticipant LeftChatMember { get; set; }
}

public class From
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("is_bot")]
    public bool IsBot { get; set; }

    [JsonPropertyName("first_name")]
    public string FirstName { get; set; }

    [JsonPropertyName("last_name")]
    public string? LastName { get; set; }
}

public class Chat
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }
}

public class ChatParticipant
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("is_bot")]
    public bool IsBot { get; set; }

    [JsonPropertyName("first_name")]
    public string FirstName { get; set; }

    [JsonPropertyName("username")]
    public string Username { get; set; }
}

public class CallbackQuery
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("from")]
    public From From { get; set; }

    [JsonPropertyName("message")]
    public Message Message { get; set; }

    [JsonPropertyName("chat_instance")]
    public string ChatInstance { get; set; }

    [JsonPropertyName("data")]
    public string Data { get; set; }

}
