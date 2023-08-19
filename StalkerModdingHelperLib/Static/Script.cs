using System.IO;
using System.Text;
using System.Threading.Tasks;
using StalkerModdingHelperLib.Enums;

namespace StalkerModdingHelperLib.Static;

public static class Script
{
    public static async Task CreateTriggerScriptAsync(string destinationPath, CopyMethod copyMethod)
    {
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

    local path = [[{destinationPath}\bin\stalker_modding_helper.txt]]
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

        var destinationDirectoryPath = copyMethod == CopyMethod.Folder
            ? $"{destinationPath}\\StalkerModdingHelper\\gamedata\\scripts"
            : $"{destinationPath}\\gamedata\\scripts";

        var destinationFilePath = $"{destinationDirectoryPath}\\stalker_modding_helper.script";
        Directory.CreateDirectory(destinationDirectoryPath);
        await IO.WriteFileAsync(destinationFilePath, code);
    }
    
    public static async Task CreateTriggerFileAsync(string destinationPath, CopyMethod copyMethod, string saveName)
    {
        var destinationDirectoryPath = copyMethod == CopyMethod.Folder
            ? $"{destinationPath}\\StalkerModdingHelper\\bin"
            : $"{destinationPath}\\bin";

        var destinationFilePath = $"{destinationDirectoryPath}\\stalker_modding_helper.txt";
        Directory.CreateDirectory(destinationDirectoryPath);
        await IO.WriteFileAsync(destinationFilePath, saveName.TrimEnd(".sav"));
    }
}