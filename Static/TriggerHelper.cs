namespace StalkerModdingHelper.Static;

public static class TriggerHelper
{
    
    public static void CreateTriggerScript(ConfigDto config)
    {
        var triggerPath = config.LaunchType switch
        {
            LaunchType.Game => @$"{config.LaunchPath}",
            LaunchType.ModOrganizer => $@"{config.LaunchPath}\mods\{ConfigParameterName.StalkerModdingHelper}"
        };
        
        var code = $@"
local first_update = false
local next_update = time_global() + 1000
local unpaused = false

function on_game_start()  
    get_console():execute('keypress_on_start off') 
    get_console():execute('g_always_active on') 
    level.add_call(on_update,function() end)
end

function on_update()
    if device():is_paused() and unpaused == false then
        device():pause(false)
        unpaused = true
        return
    end
    
    if db.actor == nil then
        return
    end

    update(time_global())
end

function update(time)
    if time < next_update then
        return
    end   

    next_update = time + 1000

    local path = [[{triggerPath}\bin\stalker_modding_helper.txt]]
    local file = io.open(path,'r')
    if file == nil then
        return
    end

    local save = file:read()
    file:close()

    if save == nil or save == """" then
        return
    end

    file = io.open(path,'w')
    if file == nil then
        return
    end

    file:write("""")
    file:close()

    get_console():execute('load ' .. save)
end";

        var scriptsPath = config.LaunchType switch
        {
            LaunchType.Game => $"{config.LaunchPath}\\gamedata\\scripts",
            LaunchType.ModOrganizer => $"{config.LaunchPath}\\mods\\StalkerModdingHelper\\gamedata\\scripts",
            _ => string.Empty
        };

        if (string.IsNullOrEmpty(scriptsPath))
            ConsoleHelper.LogError(ConfigParameterName.StalkerModdingHelper,
                $"The scripts path required for creating a trigger script is empty.\n" +
                $"This is a result of the {ConfigParameterName.LaunchType} being 0 (Unknown)");
        
        Directory.CreateDirectory(scriptsPath);
            
        using Stream script = File.Open($"{scriptsPath}\\stalker_modding_helper.script", FileMode.Create);
        var encoding = Encoding.GetEncoding(1252);
        var saveNameBuffer = encoding.GetBytes(code);
        script.Write(saveNameBuffer, 0, saveNameBuffer.Length);
    }
    
    public static void CreateTriggerFile(ConfigDto config)
    {
        var binPath = config.LaunchType switch
        {
            LaunchType.Game => $"{config.LaunchPath}\\bin",
            LaunchType.ModOrganizer => $"{config.LaunchPath}\\mods\\StalkerModdingHelper\\bin",
            _ => string.Empty
        };
        
        if (string.IsNullOrEmpty(binPath))
            ConsoleHelper.LogError(ConfigParameterName.StalkerModdingHelper,
                $"The bin path required for creating a trigger file is empty.\n" +
                $"This is a result of the {ConfigParameterName.LaunchType} being 0 (Unknown)");
        
        Directory.CreateDirectory(binPath);
        
        using Stream trigger = File.Open($"{binPath}\\stalker_modding_helper.txt", FileMode.Create);
        var saveNameBuffer = Encoding.UTF8.GetBytes(config.SaveName);
        trigger.Write(saveNameBuffer, 0, saveNameBuffer.Length);
    }
}