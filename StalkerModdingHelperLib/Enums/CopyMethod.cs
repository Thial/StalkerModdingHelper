using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace StalkerModdingHelperLib.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CopyMethod
    {
        [EnumMember(Value = "unknown")]
        Unknown = 0,
        
        [EnumMember(Value = "folder")]
        Folder = 1,
        
        [EnumMember(Value = "contents")]
        FolderContents = 2,
    }
}