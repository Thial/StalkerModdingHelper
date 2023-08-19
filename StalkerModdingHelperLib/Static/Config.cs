using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using FluentValidation;
using StalkerModdingHelperLib.Enums;
using StalkerModdingHelperLib.Model;
using StalkerModdingHelperLib.Validators;

namespace StalkerModdingHelperLib.Static
{
    public static class Config
    {
        /// <summary>
        /// Loads the config.
        /// </summary>
        /// <returns>A <see cref="ConfigDto"/> instance.</returns>
        public static async Task<ConfigDto> LoadConfigAsync()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fileDirectory = Path.GetDirectoryName(assembly.Location);
            var filePath = $"{fileDirectory}\\Config.json";
            
            if (File.Exists(filePath) == false)
                throw new Exception($"The Config.json file is missing at {filePath}.\nPlease run StalkerModdingHelperConfigurator.exe to generate it.");

            var json = await IO.ReadFileAsync(filePath);
            var config = Json.Deserialize<ConfigDto>(json);
            
            await new ConfigDtoValidator().ValidateAndThrowAsync(config);

            return config;
        }

        /// <summary>
        /// Saves the config.
        /// </summary>
        public static async Task SaveConfigAsync(ConfigDto config)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fileDirectory = Path.GetDirectoryName(assembly.Location);
            var filePath = $"{fileDirectory}\\Config.json";
            
            await new ConfigDtoValidator().ValidateAndThrowAsync(config);
            
            var json = Json.Serialize(config);
            
            await IO.WriteFileAsync(filePath, json);
        }

        /// <summary>
        /// Generates a new config
        /// </summary>
        /// <returns></returns>
        public static ConfigDto GenerateConfig()
        {
            return new ConfigDto()
            {
                Instance = new InstanceDto()
                {
                    Type = InstanceType.Stalker,
                    ExecutablePath = "D:\\Anomaly\\bin\\AnomalyDX11.exe",
                    InstancePath = "D:\\Anomaly",
                    Copy = true,
                    CopyMethod = CopyMethod.FolderContents,
                    AutoLoad = true,
                    SaveName = "test",
                    BlacklistedExtensions = new List<string>(),
                    WhitelistedFolders = new List<string>()
                    {
                        "bin", "gamedata"
                    }
                },
                Mods = new List<ModDto>()
                {
                    new()
                    {
                        ModPath = "D:\\Stalker Mods\\Some Example Mod",
                    }
                }
            };
        }
    }
}