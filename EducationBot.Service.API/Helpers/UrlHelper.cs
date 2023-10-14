using System.Text;

namespace EducationBot.Service.API.Helpers;

public class UrlHelper
{
    public static string GetUriWithQueryString(string requestUri, Dictionary<string, string> queryStringParams)
    {
        bool startingQuestionMarkAdded = false;
        StringBuilder sb = new();
        sb.Append(requestUri);

        foreach (var parameter in queryStringParams)
        {
            if (parameter.Value == null)
                continue;

            sb.Append(startingQuestionMarkAdded ? '&' : '?');
            sb.Append(parameter.Key);
            sb.Append('=');
            sb.Append(parameter.Value);
            startingQuestionMarkAdded = true;
        }

        return sb.ToString();
    }
}
