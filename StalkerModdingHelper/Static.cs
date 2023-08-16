using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace StalkerModdingHelper
{
    public static class Static
    {
        [DllImport("user32.dll", CharSet=CharSet.Auto,ExactSpelling=true)]
        public static extern IntPtr SetFocus(HandleRef hWnd);
        
        public static Dictionary<string,string> ReadConfig()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fileDirectory = Path.GetDirectoryName(assembly.Location);
            var filePath = $"{fileDirectory}\\Config.ini";
            
            if (File.Exists(filePath) == false)
                throw new Exception($"The Config.ini file is missing at {filePath}.\nPlease run Config.exe to generate it.");
            
            var lines = File.ReadLines(filePath);
            var dict = new Dictionary<string, string>();
            
            foreach (var line in lines)
            {
                var split = line.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);

                if (split.Length != 2)
                    continue;
                
                if (dict.ContainsKey(split[0].Trim()) == false)
                    dict.Add(split[0].Trim(), split[1].Trim());
            }

            var validationMessage = new StringBuilder();
            if (dict.ContainsKey("StalkerPath") == false)
                validationMessage.AppendLine("The StalkerPath value is empty or missing.");
            if (dict.ContainsKey("AutoRun") == false)
                validationMessage.AppendLine("The AutoRun value is empty or missing.");
            if (dict.ContainsKey("SaveName") == false)
                validationMessage.AppendLine("The SaveName value is empty or missing.");
            if (dict.Keys.Any(key => key.StartsWith("ModPath")) == false)
                validationMessage.AppendLine("The ModPath values are empty or missing.");

            if (validationMessage.Length > 0)
            {
                validationMessage.AppendLine("Please run Config.exe to configure the application.");
                throw new Exception(validationMessage.ToString());
            }

            return dict;
        }

        public static void ValidatePaths(Dictionary<string, string> config)
        {
            if (Directory.Exists(config["StalkerPath"]) == false)
                throw new Exception($"The StalkerPath {config["StalkerPath"]} does not exist.");

            foreach (var kvp in config)
            {
                if (kvp.Key.StartsWith("ModPath") == false)
                    continue;
                
                if (Directory.Exists(kvp.Value) == false)
                    throw new Exception($"The {kvp.Key} {kvp.Value} does not exist.");
            }
        }

        public static StalkerDirectory[] GetModDirectories(Dictionary<string, string> config)
        {
            var modPaths = config["ModPath"]
                .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                .Select(value => value.Trim())
                .ToArray();
            
            var skipExtensions = config["SkipExtension"]
                .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                .Select(value => value.Trim())
                .ToArray();
            
            if (modPaths.Any() == false)
                return Array.Empty<StalkerDirectory>();

            var directories = new List<StalkerDirectory>();

            foreach (var modPath in modPaths)
            {
                Console.WriteLine($"Getting Mod Files: {modPath}");
                
                var modFiles = Directory.EnumerateFiles(modPath, "*.*", SearchOption.AllDirectories)
                    .Where(file => file.Contains("bin") || file.Contains("gamedata"))
                    .Select(file => file.TrimStart(modPath))
                    .ToArray();

                if (skipExtensions.Length > 0)
                {
                    var filteredModFiles = (from modFile in modFiles let skip = skipExtensions
                        .Any(modFile.EndsWith) where skip == false select modFile)
                        .ToArray();
                    
                    Console.WriteLine($"Found {filteredModFiles.Length} files after filtering");
                    directories.Add(new StalkerDirectory(modPath, filteredModFiles));
                    continue;
                }
                
                Console.WriteLine($"Found {modFiles.Length} files");
                directories.Add(new StalkerDirectory(modPath, modFiles));
            }

            return directories.ToArray();
        }
        
        public static async Task ProcessModDirectory(Dictionary<string,string> config, StalkerDirectory modDirectory)
        {
            var tasks = modDirectory.Files.Select(file => ProcessModFile(config, modDirectory, file));
            await Task.WhenAll(tasks);
        }

        public static StalkerExecutable GetStalkerExecutable(Dictionary<string,string> config)
        {
            var stalkerExecutable = config["StalkerExecutable"];
            var stalkerExecutableRawFileInfo = new FileInfo(stalkerExecutable);
            if (stalkerExecutableRawFileInfo.Exists)
            {
                return new StalkerExecutable(stalkerExecutable, stalkerExecutable.TrimEnd(".exe"));
            }

            var fileName = stalkerExecutable.TrimEnd(".exe");
            var filePath = $"{config["StalkerPath"]}\\bin\\{fileName}.exe";
            var stalkerExecutableConstructedFileInfo = new FileInfo(filePath);
            if (stalkerExecutableConstructedFileInfo.Exists)
            {
                return new StalkerExecutable(filePath, fileName);
            }
            
            throw new Exception($"Could not locate the Stalker executable \"{stalkerExecutable}\". " +
                       $"Please update Config.ini and change the \"StalkerExecutable\" " +
                       $"value to either a vaild Stalker executable name or a full path");
        }

        public static bool IsAutoRunEnabled(Dictionary<string, string> config)
        {
            if (config.ContainsKey("AutoRun") == false)
                return false;

            var value = config["AutoRun"].ToLower();
            
            return value is "true" or "t" or "yes" or "y" or "1";
        }

        public static bool IsStalkerRunning(Dictionary<string,string> config)
        {
            var stalkerProcess = GetStalkerProcess(config);
            return stalkerProcess != null;
        }

        public static Process GetStalkerProcess(Dictionary<string,string> config)
        {
            var stalkerExecutable = GetStalkerExecutable(config);
            var processlist = Process.GetProcesses();
            return processlist.FirstOrDefault(p => p.ProcessName == stalkerExecutable.Name);
        }

        public static void FocusStalkerWindow(Dictionary<string, string> config)
        {
            var stalkerProcess = GetStalkerProcess(config);
            if (stalkerProcess is null)
                return;

            SetFocus(new HandleRef(null, stalkerProcess.MainWindowHandle));
        }

        public static void CreateTriggerScript(Dictionary<string, string> config)
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

    local path = [[{config["StalkerPath"]}\bin\stalker_modding_helper.txt]]
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

            var scriptsPath = $"{config["StalkerPath"]}\\gamedata\\scripts";
            Directory.CreateDirectory(scriptsPath);
            
            using Stream script = File.Open($"{scriptsPath}\\stalker_modding_helper.script", FileMode.Create);
            var encoding = Encoding.GetEncoding(1252);
            var saveNameBuffer = encoding.GetBytes(code);
            script.Write(saveNameBuffer, 0, saveNameBuffer.Length);
        }

        public static void CreateTriggerFile(Dictionary<string, string> config)
        {
            using Stream trigger = File.Open($"{config["StalkerPath"]}\\bin\\stalker_modding_helper.txt", FileMode.Create);
            var saveNameBuffer = System.Text.Encoding.UTF8.GetBytes(config["SaveName"].TrimEnd(".sav"));
            trigger.Write(saveNameBuffer, 0, saveNameBuffer.Length);
        }

        public static void StartStalker(Dictionary<string,string> config)
        {
            var stalkerExecutable = GetStalkerExecutable(config);
            var saveName = config["SaveName"].TrimEnd(".sav");
            
            var stalkerProcess = new Process();
            stalkerProcess.StartInfo.FileName = stalkerExecutable.Path;
            stalkerProcess.StartInfo.Arguments = $"-dbg -nocache -cls -start server({saveName}/single/alife/load) client(localhost)";
            stalkerProcess.Start();
        }
        
        #region Implementation
        static bool FileEquals(this FileInfo first, FileInfo second)
        {
            var firstHash = MD5.Create().ComputeHash(first.OpenRead());
            var secondHash = MD5.Create().ComputeHash(second.OpenRead());
            return !firstHash.Where((t, i) => t != secondHash[i]).Any();
        }

        static async Task Copy(this FileInfo first, FileInfo second)
        {
            Directory.CreateDirectory(second.DirectoryName);
            Console.WriteLine($"Copying {first.FullName}");
            var happy = false;
            while (happy == false)
            {
                try
                {
                    using var inputFileStream = new FileStream(first.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    using var streamReader = new StreamReader(inputFileStream, Encoding.Default);
                    var fileData = await streamReader.ReadToEndAsync();
                    using var outputFileStream = new FileStream(second.FullName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                    using var streamWriter = new StreamWriter(outputFileStream, Encoding.Default);
                    await streamWriter.WriteAsync(fileData);
                    happy = true;
                }
                catch
                {
                    Console.WriteLine($"Failed to copy file: {first.FullName}. Retrying...");
                }
            }
        }
        
        static async Task ProcessModFile(Dictionary<string,string> config, StalkerDirectory modDirectory, string modFile)
        {
            var gameFileInfo = new FileInfo($"{config["StalkerPath"]}\\{modFile}");
            var modFileInfo = new FileInfo($"{modDirectory.Path}\\{modFile}");
                    
            switch (gameFileInfo.Exists)
            {
                case true:
                    if (modFileInfo.FileEquals(gameFileInfo) == false)
                        await modFileInfo.Copy(gameFileInfo);
                    break;
                case false:
                    await modFileInfo.Copy(gameFileInfo);
                    break;
            }
        }

        static string TrimStart(this string source, string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
                return source;

            if (source.StartsWith(pattern))
                return source.Substring(pattern.Length);

            return source;
        }
        
        static string TrimEnd(this string source, string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
                return source;

            if (source.EndsWith(pattern))
                return source.Substring(0, source.Length - pattern.Length);

            return source;
        }
        #endregion
    }
}