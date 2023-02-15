namespace Blog.Domain.Enums;

public class Language
{
    public const string Chinese = "zh";
    public const string English = "en";
    public const string Japanese = "ja";

    public static IList<string> Languages = new List<string>
    {
        Chinese,
        English,
        Japanese
    };

    public static string Displayname(string language)
    {
        return language switch
        {
            Chinese => "中文",
            English => "英语",
            Japanese => "日语",
            _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
        };
    }
}