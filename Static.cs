using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace StalkerModdingHelper
{
    public static class Static
    {
        public static Dictionary<string,string> ReadConfig()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fileDirectory = Path.GetDirectoryName(assembly.Location);
            var filePath = $"{fileDirectory}\\Config.ini";
            
            if (File.Exists(filePath) == false)
                PrintError("The Config.ini file is missing. Please run Config.exe to generate it.");
            
            var lines = File.ReadLines(filePath);
            var dict = new Dictionary<string, string>();
            
            foreach (var line in lines)
            {
                var split = line.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);

                if (split.Length != 2)
                    continue;
                
                if (dict.ContainsKey(split[0]) == false)
                    dict.Add(split[0], split[1]);
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
                PrintError(validationMessage.ToString());
            }

            return dict;
        }

        public static void ValidatePaths(Dictionary<string, string> config)
        {
            if (Directory.Exists(config["StalkerPath"]) == false)
                PrintError($"The StalkerPath {config["StalkerPath"]} does not exist.");

            foreach (var kvp in config)
            {
                if (kvp.Key.StartsWith("ModPath") == false)
                    continue;
                
                if (Directory.Exists(kvp.Value) == false)
                    PrintError($"The {kvp.Key} {kvp.Value} does not exist.");
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
            using (Stream source = File.Open(first.FullName, FileMode.Open))
            {
                using(Stream destination = File.Create(second.FullName))
                {
                    await source.CopyToAsync(destination);
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
        
        static void PrintError(string message)
        {
            Console.WriteLine(message);
            Console.ReadKey();
            Environment.Exit(0);
        }
        #endregion
    }
}