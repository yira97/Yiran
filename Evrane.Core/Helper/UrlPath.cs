namespace Evrane.Core.Helper;

public class UrlPath
{
    // ref
    // topic: https://stackoverflow.com/questions/372865/path-combine-for-urls
    // ans: https://stackoverflow.com/a/1476563
    public static string Combine(string uri1, string uri2)
    {
        uri1 = uri1.TrimEnd('/');
        uri2 = uri2.TrimStart('/');
        return $"{uri1}/{uri2}";
    }

    public static string PrefixSlash(string urlPart)
    {
        return urlPart.StartsWith("/") ? urlPart : $"/{urlPart}";
    }
}

public static class StringExtensions
{
    public static string UrlPathCombine(this string uri1, string uri2)
    {
        return UrlPath.Combine(uri1, uri2);
    }
}