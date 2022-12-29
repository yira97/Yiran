using System.Text;
using System.Text.Json;

namespace Blog.Api.Helper;

public static class PageTokenHelper
{
    public static string FromDict(Dictionary<string, string> dict)
    {
        var json = JsonSerializer.Serialize(dict);
        var b64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
        return b64;
    }

    public static Dictionary<string, string> ToDict(string pageToken)
    {
        var json = Encoding.UTF8.GetString(Convert.FromBase64String(pageToken));
        var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json)!;
        return dict;
    }
}