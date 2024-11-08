namespace StalkerModdingHelper.Static;

public static class Launcher
{
    public static void Launch(ConfigDto config)
    {
        if (config.AutoRun == false)
            return;

        if (IsStalkerRunning(config))
        {
            ConsoleHelper.LogInformation(ConfigParameterName.StalkerModdingHelper, "Reloading S.T.A.L.K.E.R.");
            TriggerHelper.CreateTriggerScript(config);
            TriggerHelper.CreateTriggerFile(config);
        }
        else
        {
            ConsoleHelper.LogInformation(ConfigParameterName.StalkerModdingHelper,"Launching S.T.A.L.K.E.R.");
            TriggerHelper.CreateTriggerScript(config);
            var launchAction = config.LaunchType switch
            {
                LaunchType.Game => (Action<ConfigDto>)StartStalker,
                LaunchType.ModOrganizer => (Action<ConfigDto>)StartModOrganizer,
                _ => default(Action<ConfigDto>)
            };
            
            launchAction?.Invoke(config);
        }
    }
    
    static bool IsStalkerRunning(ConfigDto config)
    {
        var processlist = Process.GetProcesses();
        var processName = config.LaunchType switch
        {
            LaunchType.Game => config.ExecutableName,
            LaunchType.ModOrganizer => ModOrganizerConfig.GetStalkerExecutableName(config)
        };
        var stalkerProcess = processlist.FirstOrDefault(p => p.ProcessName == processName);
        return stalkerProcess != null;
    }
    
    static void StartStalker(ConfigDto config)
    {
        var stalkerProcess = new Process();
        stalkerProcess.StartInfo.FileName = $"{config.LaunchPath}\\bin\\{config.ExecutableName}.exe";
        stalkerProcess.StartInfo.Arguments = $"-dbg -nocache -cls -start server({config.SaveName}/single/alife/load) client(localhost)";
        stalkerProcess.Start();
    }
    
    static void StartModOrganizer(ConfigDto config)
    {
        ModOrganizerConfig.EnableMods(config);
        ModOrganizerConfig.UpdateSaveName(config);
        var stalkerProcess = new Process();
        stalkerProcess.StartInfo.FileName = $"{config.LaunchPath}\\ModOrganizer.exe";
        stalkerProcess.StartInfo.Arguments = $"\"moshortcut://:{config.ExecutableName}\"";
        stalkerProcess.Start();
    }
}