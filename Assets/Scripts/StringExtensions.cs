using System;

public static class StringExtensions
{
    public static bool InvariantEquals(this string haystack, string needle)
    {
        return haystack.Equals(needle, StringComparison.InvariantCultureIgnoreCase);
    }
    public static bool InvariantStartsWith(this string haystack, string needle)
    {
        return haystack.StartsWith(needle, StringComparison.InvariantCultureIgnoreCase);
    }
}