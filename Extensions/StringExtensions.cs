namespace StalkerModdingHelper.Extensions;

public static class StringExtensions
{
    public static string TrimStart(this string source, string pattern)
    {
        if (string.IsNullOrEmpty(pattern))
            return source;

        return source.StartsWith(pattern) 
            ? source.Substring(pattern.Length) 
            : source;
    }
        
    public static string TrimEnd(this string source, string pattern)
    {
        if (string.IsNullOrEmpty(pattern))
            return source;

        return source.EndsWith(pattern) 
            ? source.Substring(0, source.Length - pattern.Length) 
            : source;
    }
}