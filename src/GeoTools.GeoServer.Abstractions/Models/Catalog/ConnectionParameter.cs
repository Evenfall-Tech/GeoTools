using System.Text.Json.Serialization;

namespace GeoTools.GeoServer.Models.Catalog
{
    public class ConnectionParameter
    {
        [JsonPropertyName("@key")]
        public string Key { get; }

        [JsonPropertyName("$")]
        public string Value { get; }

        [JsonConstructor]
        public ConnectionParameter(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
