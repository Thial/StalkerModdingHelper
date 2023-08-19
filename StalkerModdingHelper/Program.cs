using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using StalkerModdingHelperLib.Enums;
using StalkerModdingHelperLib.Model;
using StalkerModdingHelperLib.Static;
using StalkerModdingHelperLib.Validators;

namespace StalkerModdingHelper
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                await new Helper().Run();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadKey();
            }
        }
    }

    public class Helper
    {
        public async Task Run()
        {
            var config = await Config.LoadConfigAsync();

            if (config.Instance.Copy is true || config.Mods.Any(mod => mod.Copy is true))
            {
                Console.WriteLine("Copying Files.");
                await CopyFiles(config);
                Console.WriteLine("Done.");
            }

            if (config.Instance.AutoLoad == false)
                return;
            
            var copyMethod = config.Instance.CopyMethod ?? CopyMethod.FolderContents;

            if (Instance.IsRunning(config.Instance.ExecutablePath))
            {
                Console.WriteLine("Reloading S.T.A.L.K.E.R.");
                await Script.CreateTriggerScriptAsync(config.Instance.InstancePath, copyMethod);
                await Script.CreateTriggerFileAsync(config.Instance.InstancePath, copyMethod, config.Instance.SaveName);
            }
            else
            {
                Console.WriteLine("Launching S.T.A.L.K.E.R.");
                await Script.CreateTriggerScriptAsync(config.Instance.InstancePath, copyMethod);
                Instance.Start(config.Instance.ExecutablePath, config.Instance.Type ?? InstanceType.Unknown, config.Instance.SaveName);
            }
        }

        #region Implementation
        async Task CopyFiles(ConfigDto config)
        {
            if (config.Mods.Any() == false)
                return;

            var modTasks = config.Mods.Select(mod => CopyMod(config, mod));
            await Task.WhenAll(modTasks);
        }

        async Task CopyMod(ConfigDto config, ModDto mod)
        {
            if (mod.Copy is false && config.Instance.Copy is false)
                return;
            
            var modFiles = Directory
                .GetFiles(mod.ModPath, "*.*", SearchOption.AllDirectories)
                .Select(file => file.TrimStart($"{mod.ModPath.TrimEnd('\\')}\\"))
                .ToArray();

            var blacklistedExtensions = mod.BlacklistedExtensions != null && mod.BlacklistedExtensions.Any()
                ? mod.BlacklistedExtensions
                : config.Instance.BlacklistedExtensions != null && config.Instance.BlacklistedExtensions.Any()
                    ? config.Instance.BlacklistedExtensions
                    : new List<string>();
            
            var whitelistedFolders = mod.WhitelistedFolders != null && mod.WhitelistedFolders.Any()
                ? mod.BlacklistedExtensions
                : config.Instance.WhitelistedFolders != null && config.Instance.WhitelistedFolders.Any()
                    ? config.Instance.WhitelistedFolders
                    : new List<string>();

            if (blacklistedExtensions.Any())
            {
                var filteredModFiles = (from modFile in modFiles let skip = blacklistedExtensions
                    .Any(modFile.EndsWith) where skip == false select modFile)
                    .ToArray();

                modFiles = filteredModFiles;
            }
            
            if (whitelistedFolders.Any())
            {
                var filteredModFiles = (from modFile in modFiles let include = whitelistedFolders
                    .Any(modFile.StartsWith) where include select modFile)
                    .ToArray();

                modFiles = filteredModFiles;
            }

            var copyTasks = modFiles.Select(file => CopyFile(config, mod, file));
            await Task.WhenAll(copyTasks);
        }

        async Task CopyFile(ConfigDto config, ModDto mod, string fileRelativePath)
        {
            var copyMethod = mod.CopyMethod ?? config.Instance.CopyMethod;
            
            var modFilePath = $"{mod.ModPath}\\{fileRelativePath}";
            
            var destinationPath = string.IsNullOrEmpty(mod.DestinationPath) == false
                ? mod.DestinationPath
                : config.Instance.InstancePath;
            
            var destinationFilePath = copyMethod == CopyMethod.Folder
                ? $"{destinationPath}\\{new DirectoryInfo(mod.ModPath).Name}\\{fileRelativePath}"
                : $"{destinationPath}\\{fileRelativePath}";

            var destinationDirectoryPath = Path.GetDirectoryName(destinationFilePath);
            Directory.CreateDirectory(destinationDirectoryPath);

            var destinationFileInfo = new FileInfo(destinationFilePath);
            if (destinationFileInfo.Exists == false || modFilePath.FileEquals(destinationFilePath) == false)
            {
                var happy = false;
                while (happy == false)
                {
                    try
                    {
                        var modFileData = await IO.ReadFileAsync(modFilePath);
                        await IO.WriteFileAsync(destinationFilePath, modFileData);
                        happy = true;
                        Console.WriteLine($"Copied {modFilePath} to {destinationFilePath}");
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine($"Failed to copy {modFilePath}. Retrying...\n{e.Message}");
                    }
                }
            }
        }
        #endregion
        
    }
}