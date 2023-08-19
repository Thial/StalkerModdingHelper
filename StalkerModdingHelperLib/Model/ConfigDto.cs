using System.Collections.Generic;
using Newtonsoft.Json;

namespace StalkerModdingHelperLib.Model
{
    public class ConfigDto
    {
        [JsonProperty("instance")]
        public InstanceDto Instance { get; set; }
        
        [JsonProperty("mods")]
        public IList<ModDto> Mods { get; set; }
    }
}