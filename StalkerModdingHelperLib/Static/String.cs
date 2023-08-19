namespace StalkerModdingHelperLib.Static;

public static class String
{
    public static string TrimStart(this string source, string pattern)
    {
        if (string.IsNullOrEmpty(pattern))
            return source;

        if (source.StartsWith(pattern))
            return source.Substring(pattern.Length);

        return source;
    }
        
    public static string TrimEnd(this string source, string pattern)
    {
        if (string.IsNullOrEmpty(pattern))
            return source;

        if (source.EndsWith(pattern))
            return source.Substring(0, source.Length - pattern.Length);

        return source;
    }
}