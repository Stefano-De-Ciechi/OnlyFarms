namespace OnlyFarms.Core.Infrastructure;

public static class StringExtensions
{
    public static string Capitalize(this string str)
    {
        ArgumentNullException.ThrowIfNull(str);

        return str switch
        {
            "" => "",
            _ => $"{str[0].ToString().ToUpper()}{str[1..]}"
        };
    }

    public static string UnCapitalize(this string str)
    {
        ArgumentNullException.ThrowIfNull(str);

        return str switch
        {
            "" => "",
            _ => $"{str[0].ToString().ToLower()}{str[1..]}"
        };
    }
    
    public static T ParseEnum<T>(this string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }
}