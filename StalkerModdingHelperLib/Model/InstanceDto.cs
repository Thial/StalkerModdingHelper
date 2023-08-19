using System.Collections.Generic;
using Newtonsoft.Json;
using StalkerModdingHelperLib.Enums;

namespace StalkerModdingHelperLib.Model
{
    public class InstanceDto
    {
        [JsonProperty("type")]
        public InstanceType? Type { get; set; }
        
        [JsonProperty("executable_path")]
        public string ExecutablePath { get; set; }
        
        [JsonProperty("instance_path")]
        public string InstancePath { get; set; }
        
        [JsonProperty("copy")]
        public bool? Copy { get; set; }
        
        [JsonProperty("copy_method")]
        public CopyMethod? CopyMethod { get; set; }
        
        [JsonProperty("blacklisted_extensions")]
        public IList<string> BlacklistedExtensions { get; set; }
        
        [JsonProperty("whitelisted_folders")]
        public IList<string> WhitelistedFolders { get; set; }
        
        [JsonProperty("save_name")]
        public string SaveName { get; set; }
        
        [JsonProperty("auto_load")]
        public bool? AutoLoad { get; set; }
    }
}