namespace StalkerModdingHelper.Static;

public static class ConfigValidator
{
    public static void ValidateAndCorrect(ConfigDto config)
    {
        ValidateAndCorrectConfig(config);
        ValidateAndCorrectMods(config);
    }
    
    #region Implementation
    
    static void ValidateAndCorrectConfig(ConfigDto config)
    {
        if (config.LaunchType is LaunchType.Unknown)
        {
            ConsoleHelper.LogError(ConfigParameterName.StalkerModdingHelper, 
                $"The {ConfigParameterName.LaunchType} parameter is 0 (Unknown). Please change it to 1 (Stalker) or 2 (Mod Organizer).");
        }
        
        if (config.LaunchType is LaunchType.Game)
        {
            if (string.IsNullOrEmpty(config.ExecutableName) == false)
            {
                config.ExecutableName = config.ExecutableName.TrimEnd(".exe");
            }
            else
            {
                config.ExecutableName = "AnomalyDX11";
                ConsoleHelper.LogWarning(ConfigParameterName.StalkerModdingHelper, 
                    $"{ConfigParameterName.ExecutableName} parameter was not provided. Defaulting to \"AnomalyDX11\"");
            }
        }
        else if (config.LaunchType is LaunchType.ModOrganizer && string.IsNullOrEmpty(config.ProfileName))
        {
            config.ProfileName = "Default";
            ConsoleHelper.LogWarning(ConfigParameterName.StalkerModdingHelper, 
                $"{ConfigParameterName.ProfileName} parameter was not provided. Defaulting to \"Default\"");
        }

        if (string.IsNullOrEmpty(config.LaunchPath))
        {
            ConsoleHelper.LogError(ConfigParameterName.StalkerModdingHelper, 
                $"[StalkerModdingHelper] The {ConfigParameterName.LaunchPath} is null or empty.\n" +
                "Please provide a valid path to either Stalker Root Directory or Mod Organizer root directory.");
        }

        if (Directory.Exists(config.LaunchPath) == false)
        {
            ConsoleHelper.LogError(ConfigParameterName.StalkerModdingHelper, 
                $"[StalkerModdingHelper] The {ConfigParameterName.LaunchPath} {config.LaunchPath} does not exist.");
        }

        if (string.IsNullOrEmpty(config.SaveName) == false)
        {
            config.SaveName = config.SaveName.TrimEnd(".sav").TrimEnd(".scop");
        }
        else
        {
            config.SaveName = "testing";
            ConsoleHelper.LogWarning(ConfigParameterName.StalkerModdingHelper, 
                $"{ConfigParameterName.SaveName} parameter was not provided. Defaulting to \"testing\"");
        }
    }

    static void ValidateAndCorrectMods(ConfigDto config)
    {
        if (config.Mods.Any())
        {
            var length = config.Mods.Count;
            for (var i = 0; i < length; i++)
            {
                ValidateAndCorrectMod(config.Mods[i]);
            }
        }
    }

    static void ValidateAndCorrectMod(ConfigModDto mod)
    {
        if (string.IsNullOrEmpty(mod.ModPath))
            ConsoleHelper.LogError(mod.ModName, $"The {ConfigParameterName.ModPath} is null or empty.\n" + 
                                             "Please provide a valid path to the mod's directory.");
        
        if (Directory.Exists(mod.ModPath) == false)
            ConsoleHelper.LogError(mod.ModName, $"The {ConfigParameterName.ModPath} {mod.ModPath} does not exist.");

        if (mod.SkipExtensions.Any())
        {
            var length = mod.SkipExtensions.Count;
            for (var i = 0; i < length; i++)
            {
                mod.SkipExtensions[i] = mod.SkipExtensions[i].TrimStart('.');
            }
        }
    }
    
    #endregion
}