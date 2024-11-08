namespace StalkerModdingHelper.Static;

public static class ConfigReader
{
    public static ConfigDto ReadConfig()
    {
        var configLines = GetConfigLines();
        var sectionDict = ParseSections(configLines);

        var headerExists = sectionDict.Any(kvp => kvp.Key == ConfigParameterName.StalkerModdingHelper);
        if (headerExists == false)
            ConsoleHelper.LogError(ConfigParameterName.StalkerModdingHelper, 
                "The [StalkerModdingHelper] section is missing from the config file.\n" +
                "Please refer to the GitHub page for instructions on how to configure the config file.");

        var config = ParseHeaderSection(sectionDict.First(kvp => kvp.Key == ConfigParameterName.StalkerModdingHelper).Value);

        if (sectionDict.Count(kvp => kvp.Key != ConfigParameterName.StalkerModdingHelper) > 0)
        {
            var mods = sectionDict
                .Where(kvp => kvp.Key != ConfigParameterName.StalkerModdingHelper)
                .Select(kvp => ParseModSection(kvp.Key, kvp.Value))
                .ToList();

            config.Mods = mods.ToList();
        }

        return config;
    }

    #region Implementation

    static readonly Regex HeaderRegex = new("\\[(.*?)\\]");

    static IEnumerable<string> GetConfigLines()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var fileDirectory = Path.GetDirectoryName(assembly.Location);
        var filePath = $"{fileDirectory}\\{ConfigParameterName.StalkerModdingHelper}.ini";
            
        if (File.Exists(filePath) == false)
            ConsoleHelper.LogError(ConfigParameterName.StalkerModdingHelper, 
                $"The config file is missing at {filePath}.\n" +
                $"A new default config file has been generated.\n" +
                $"Please configure it and restart the application.");
            
        var lines = File.ReadLines(filePath).ToArray();
        if (lines.Any() == false)
            ConsoleHelper.LogError(ConfigParameterName.StalkerModdingHelper, "The config file has no data");
        
        return lines.Select(l => l.Trim());
    }

    static Dictionary<string,List<Tuple<string, string>>> ParseSections(IEnumerable<string> lines)
    {
        var result = new Dictionary<string, List<Tuple<string, string>>>();
        var headerFound = false;
        var currentHeaderName = string.Empty;
        foreach (var line in lines)
        {
            var match = HeaderRegex.Match(line);
            if (match.Success)
            {
                headerFound = true;
                currentHeaderName = match.Groups[1].Value;
                if (result.ContainsKey(currentHeaderName) == false)
                {
                    result[currentHeaderName] = new List<Tuple<string, string>>();
                }
                else
                {
                    ConsoleHelper.LogError(ConfigParameterName.StalkerModdingHelper, 
                        $"The config contains a duplicate entry for section {currentHeaderName}");
                }
            }
            else if (headerFound)
            {
                var split = line.Split(new[] {'='}, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length != 2)
                    continue;
                
                result[currentHeaderName].Add(new Tuple<string, string>(split[0].Trim(), split[1].Trim()));
            }
        }

        return result;
    }

    static ConfigDto ParseHeaderSection(List<Tuple<string, string>> lines)
    {
        var launchTypeRaw = lines.FirstOrDefault(kvp => kvp.Item1 == ConfigParameterName.LaunchType)?.Item2;
        var launchRootPath = lines.FirstOrDefault(kvp => kvp.Item1 == ConfigParameterName.LaunchPath)?.Item2;
        var saveName = lines.FirstOrDefault(kvp => kvp.Item1 == ConfigParameterName.SaveName)?.Item2;
        var autoRunRaw = lines.FirstOrDefault(kvp => kvp.Item1 == ConfigParameterName.AutoRun)?.Item2;
        
        var stalkerExeName = lines.FirstOrDefault(kvp => kvp.Item1 == ConfigParameterName.ExecutableName)?.Item2;
        var modOrganizerProfile = lines.FirstOrDefault(kvp => kvp.Item1 == ConfigParameterName.ProfileName)?.Item2;
        var launchType = launchTypeRaw switch
        {
            "1" => LaunchType.Game,
            "2" => LaunchType.ModOrganizer,
            _ => LaunchType.Unknown
        };
        var autoRun = autoRunRaw?.ToLower() is "1" or "y" or "yes" or "t" or "true";

        var config = new ConfigDto()
        {
            LaunchType = launchType,
            LaunchPath = launchRootPath,
            SaveName = saveName,
            AutoRun = autoRun,
            ExecutableName = stalkerExeName,
            ProfileName = modOrganizerProfile,
            Mods = new List<ConfigModDto>()
        };

        return config;
    }

    static ConfigModDto ParseModSection(string modName, List<Tuple<string, string>> lines)
    {
        var modPath = lines.FirstOrDefault(kvp => kvp.Item1 == ConfigParameterName.ModPath)?.Item2;
        var skipExtensionsRaw = lines.FirstOrDefault(kvp => kvp.Item1 == ConfigParameterName.SkipExtension)?.Item2;
        var skipExtensionsSplit = skipExtensionsRaw?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        var skipExtensions = skipExtensionsSplit?.Select(e => e.Trim())?.ToList() ?? new List<string>();

        var mod = new ConfigModDto()
        {
            ModName = modName,
            ModPath = modPath,
            SkipExtensions = skipExtensions
        };

        return mod;
    }

    #endregion
}