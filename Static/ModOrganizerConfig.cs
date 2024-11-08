namespace StalkerModdingHelper.Static;

public static class ModOrganizerConfig
{
    public static void EnableMods(ConfigDto config)
    {
        var filePath = $"{config.LaunchPath}\\profiles\\{config.ProfileName}\\modlist.txt";
            
        if (File.Exists(filePath) == false)
            ConsoleHelper.LogError(ConfigParameterName.StalkerModdingHelper, 
                $"The file {filePath} is missing. Unable to adjust the ModOrganizer mod list.");

        var modList = config.Mods
            .Select(m => m.ModName)
            .ToList();

        //we need to make sure that the trigger script mod is added too
        modList.Add(ConfigParameterName.StalkerModdingHelper); 
            
        var lines = File.ReadLines(filePath).ToList();

        foreach (var modName in modList)
        {
            if (lines.Any(l => l.Contains(modName)) == false)
                lines.Insert(0, $"+{modName}");
        }
        
        File.WriteAllLines(filePath, lines);
        
        ConsoleHelper.LogInformation(ConfigParameterName.StalkerModdingHelper, $"Updated modlist.txt for profile {config.ProfileName}.");
    }

    public static void UpdateSaveName(ConfigDto config)
    {
        var filePath = $"{config.LaunchPath}\\ModOrganizer.ini";
            
        if (File.Exists(filePath) == false)
            ConsoleHelper.LogError(ConfigParameterName.StalkerModdingHelper, 
                $"The file {filePath} is missing. Unable to adjust the ModOrganizer executable arguments.");
            
        var lines = File.ReadLines(filePath).ToList();
        var length = lines.Count;

        var titleLine = lines.FirstOrDefault(l => l.Contains("title") && l.Contains(config.ExecutableName));
        if (titleLine is null)
            ConsoleHelper.LogError(ConfigParameterName.StalkerModdingHelper, 
                $"Could not find a \"title\" line for executable {config.ExecutableName} in ModOrganizer.ini.\n" +
                $"Please make sure that the name is entered correctly and that the entry exists.");

        var lineSplit = titleLine.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
        if (int.TryParse(lineSplit?[0], out int index) == false)
            ConsoleHelper.LogError(ConfigParameterName.StalkerModdingHelper, 
                $"Could not extract the index of the executable by the name of {config.ExecutableName}.\n" +
                $"Please make sure that the ModOrganizer.ini file is properly formatted.");

        var argumentLine = lines.FirstOrDefault(l => l.StartsWith(index.ToString()) && l.Contains("arguments"));
        if (argumentLine is null)
            ConsoleHelper.LogError(ConfigParameterName.StalkerModdingHelper, 
                $"Could not find an \"arguments\" line for executable {config.ExecutableName} in ModOrganizer.ini.\n" +
                $"Please make sure that the name is entered correctly and that the entry exists.");

        var argumentLineIndex = lines.FindIndex(l => l == argumentLine);
        lines[argumentLineIndex] = $"{index}\\arguments=-dbg -nocache -cls -start server({config.SaveName}/single/alife/load) client(localhost)";
        
        File.WriteAllLines(filePath, lines);
        
        ConsoleHelper.LogInformation(ConfigParameterName.StalkerModdingHelper, $"Updated ModOrganizer.ini with the SaveName {config.SaveName}.");
    }
    
    public static string GetStalkerExecutableName(ConfigDto config)
    {
        var filePath = $"{config.LaunchPath}\\ModOrganizer.ini";
            
        if (File.Exists(filePath) == false)
            ConsoleHelper.LogError(ConfigParameterName.StalkerModdingHelper, 
                $"The file {filePath} is missing. Unable to get Stalker executable name.");
            
        var lines = File.ReadLines(filePath).ToList();
        var length = lines.Count;

        var titleLine = lines.FirstOrDefault(l => l.Contains("title") && l.Contains(config.ExecutableName));
        if (titleLine is null)
            ConsoleHelper.LogError(ConfigParameterName.StalkerModdingHelper, 
                $"Could not find a \"title\" line for executable {config.ExecutableName} in ModOrganizer.ini.\n" +
                $"Please make sure that the name is entered correctly and that the entry exists.");

        var lineSplit = titleLine.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
        if (int.TryParse(lineSplit?[0], out int index) == false)
            ConsoleHelper.LogError(ConfigParameterName.StalkerModdingHelper, 
                $"Could not extract the index of the executable by the name of {config.ExecutableName}.\n" +
                $"Please make sure that the ModOrganizer.ini file is properly formatted.");

        var binaryLine = lines.FirstOrDefault(l => l.StartsWith(index.ToString()) && l.Contains("binary"));
        if (binaryLine is null)
            ConsoleHelper.LogError(ConfigParameterName.StalkerModdingHelper, 
                $"Could not find an \"binary\" line for executable {config.ExecutableName} in ModOrganizer.ini.\n" +
                $"Please make sure that the name is entered correctly and that the entry exists.");

        var binarySplit = binaryLine.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
        var fileInfo = new FileInfo(binarySplit[1]);
        return fileInfo.Name.TrimEnd(".exe");
    }
}