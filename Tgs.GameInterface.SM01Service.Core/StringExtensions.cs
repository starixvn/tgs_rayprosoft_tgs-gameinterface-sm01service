namespace Tgs.GameInterface.SM01Service.Core;

public static class StringExtensions
{
    public static string NoLongerThan(this string str, int chars)
        => string.IsNullOrEmpty(str) || str.Length <= chars ? str : str[..chars];
}
