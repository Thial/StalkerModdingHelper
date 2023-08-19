using System;
using Newtonsoft.Json;

namespace StalkerModdingHelperLib.Static
{
    public static class Json
    {
        /// <summary>
        /// Serializes an object into a string.
        /// </summary>
        /// <param name="data">The object to serialize.</param>
        /// <returns>A serialized json string.</returns>
        public static string Serialize(object data)
        {
            if (data is null)
                throw new Exception("Failed to serialize json. The data is null");

            return JsonConvert.SerializeObject(data);
        }
        
        /// <summary>
        /// Deserializes a json string into an object.
        /// </summary>
        /// <param name="data">The json string to deserialize.</param>
        /// <typeparam name="T">The type of the object to deserialize into.</typeparam>
        /// <returns>A deserialized object.</returns>
        public static T Deserialize<T>(string data)
        {
            if (string.IsNullOrEmpty(data))
                throw new Exception("Failed to deserialize json. The data is null or empty");

            return JsonConvert.DeserializeObject<T>(data);
        }
    }
}