namespace Evrane.Core.Helper;

public static class EnumExtensions
{
    public static string IntString<T>(this T source) where T : IConvertible //enum
    {
        if (!typeof(T).IsEnum)
            throw new ArgumentException("T must be an enumerated type");

        return ((int)(IConvertible)source).ToString();
    }
}