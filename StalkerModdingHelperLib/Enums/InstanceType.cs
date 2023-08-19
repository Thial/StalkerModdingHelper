using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace StalkerModdingHelperLib.Enums
{
    /// <summary>
    /// Defines the instance we are working on. Either Stalker or Mod Organizer.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum InstanceType
    {
        [EnumMember(Value = "unknown")]
        Unknown = 0,
        
        /// <summary>
        /// This type is used when we are directly working on Anomaly.
        /// </summary>
        [EnumMember(Value = "stalker")]
        Stalker = 1,
        
        /// <summary>
        /// This type is used when we are working through Mod Organizer.
        /// </summary>
        [EnumMember(Value = "mod_organizer")]
        ModOrganizer = 2
    }
}