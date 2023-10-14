using Newtonsoft.Json;

namespace EducationBot.Service.API.Model.Telegram;

public class InlineKeyboardMarkup
{
    [JsonProperty("inline_keyboard")]
    public List<List<InlineKeyboardButton>> InlineKeyboard { get; set; }

    public InlineKeyboardMarkup()
    {
        InlineKeyboard = new List<List<InlineKeyboardButton>>();
    }
}

public class InlineKeyboardButton
{
    [JsonProperty("text")]
    public string Text { get; set; }

    [JsonProperty("callback_data", NullValueHandling = NullValueHandling.Ignore)]
    public string? CallbackData { get; set; }

    [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
    public string? Url { get; set; }

    public InlineKeyboardButton(string text, string? callbackData = null, string? url = null)
    {
        Text = text;
        CallbackData = callbackData;
        Url = url;
    }
}
