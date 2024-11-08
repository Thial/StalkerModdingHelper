namespace StalkerModdingHelper.Static;

public static class ConsoleHelper
{
    public static void LogInformation(string source, string message)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"[{source}] {message}");
    }
    
    public static void LogWarning(string source, string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[{source}] {message}");
    }
    
    public static void LogError(string source, string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[{source}] {message}");
        Console.ReadKey();
        Environment.Exit(0);
    }
}