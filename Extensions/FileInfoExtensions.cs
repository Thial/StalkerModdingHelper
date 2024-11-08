using System.Security.AccessControl;
using System.Security.Principal;

namespace StalkerModdingHelper.Extensions;

public static class FileInfoExtensions
{
    public static bool FileEquals(this FileInfo first, FileInfo second)
    {
        var firstHash = MD5.Create().ComputeHash(first.OpenRead());
        var secondHash = MD5.Create().ComputeHash(second.OpenRead());
        return !firstHash.Where((t, i) => t != secondHash[i]).Any();
    }
    
    public static async Task Copy(this FileInfo first, FileInfo second, string modName)
    {
        Directory.CreateDirectory(second.DirectoryName);
        var success = false;
        while (success == false)
        {
            try
            {
                using var inputFileStream = new FileStream(first.FullName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                using var streamReader = new StreamReader(inputFileStream, Encoding.Default);
                var fileData = await streamReader.ReadToEndAsync();
                using var outputFileStream = new FileStream(second.FullName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                using var streamWriter = new StreamWriter(outputFileStream, Encoding.Default);
                await streamWriter.WriteAsync(fileData);
                success = true;
                ConsoleHelper.LogInformation(modName, $"Copied {first.FullName}");
            }
            catch(Exception e)
            {
                ConsoleHelper.LogWarning(modName, $"Failed to copy file: {e.Message} {e.InnerException?.Message}");
            }
        }
    }
}