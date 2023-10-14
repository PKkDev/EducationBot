using Newtonsoft.Json;

namespace EducationBot.Service.API.Model.Telegram;

public class ReplyKeyboardMarkup
{
    [JsonProperty("keyboard")]
    public List<List<KeyboardButton>> Keyboard { get; set; }

    [JsonProperty("one_time_keyboard")]
    public bool OneTimeKeyboard { get; set; }

    public ReplyKeyboardMarkup()
    {
        Keyboard = new List<List<KeyboardButton>>();
        OneTimeKeyboard = true;
    }
}

public class KeyboardButton
{
    [JsonProperty("text")]
    public string Text { get; set; }

    public KeyboardButton(string text)
    {
        Text = text;
    }
}
