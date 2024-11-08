using System.Security.AccessControl;

namespace StalkerModdingHelper.Static;

public static class ModProcessor
{
    static readonly string[] AllowedFolders = { "\\bin\\", "\\gamedata\\", "\\db\\" };
    static readonly string[] DisallowedFolders = { "\\.git\\" };
    
    public static async Task Process(ConfigDto config, ConfigModDto mod)
    {
        var paths = GetRelativeFilePaths(mod);
        var fileTasks = paths.Select(path => ProcessModFile(config, mod, path));
        await Task.WhenAll(fileTasks);
    }
    
    static string[] GetRelativeFilePaths(ConfigModDto mod)
    {
        var allFilePaths = Directory.EnumerateFiles(mod.ModPath, "*.*", SearchOption.AllDirectories);
        var allowedFolderFilePaths = allFilePaths.Where(path => AllowedFolders.Any(path.Contains) && DisallowedFolders.Any(path.Contains) == false);
        var filteredExtensionFilePaths = allowedFolderFilePaths.Where(path => mod.SkipExtensions.Any(path.EndsWith) == false);
        var trimmedFilePaths = filteredExtensionFilePaths.Select(path => path.TrimStart(mod.ModPath).TrimStart('\\')).ToArray();

        return trimmedFilePaths;
    }
    
    static async Task ProcessModFile(ConfigDto config, ConfigModDto mod, string path)
    {
        var sourceFileInfo = new FileInfo(@$"{mod.ModPath}\{path}");
        
        var targetPath = config.LaunchType switch
        {
            LaunchType.Game => @$"{config.LaunchPath}\{path}",
            LaunchType.ModOrganizer => @$"{config.LaunchPath}\mods\{mod.ModName}\{path}",
            _ => string.Empty
        };

        var targetFileInfo = new FileInfo(targetPath);
                    
        switch (targetFileInfo.Exists)
        {
            case true:
                if (sourceFileInfo.FileEquals(targetFileInfo) == false)
                    await sourceFileInfo.Copy(targetFileInfo, mod.ModName);
                break;
            case false:
                await sourceFileInfo.Copy(targetFileInfo, mod.ModName);
                break;
        }
    }
}