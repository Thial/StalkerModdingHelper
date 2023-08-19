using System.Collections.Generic;
using Newtonsoft.Json;
using StalkerModdingHelperLib.Enums;

namespace StalkerModdingHelperLib.Model
{
    public class ModDto
    {
        [JsonProperty("mod_path")]
        public string ModPath { get; set; }
        
        [JsonProperty("destination_path")]
        public string DestinationPath { get; set; }
        
        [JsonProperty("copy")]
        public bool? Copy { get; set; }
        
        [JsonProperty("copy_method")]
        public CopyMethod? CopyMethod { get; set; }
        
        [JsonProperty("blacklisted_extensions")]
        public IList<string> BlacklistedExtensions { get; set; }
        
        [JsonProperty("whitelisted_folders")]
        public IList<string> WhitelistedFolders { get; set; }
    }
}