using GeoTools.GeoServer.Models.Catalog;
using System.Text.Json.Serialization;

namespace GeoTools.GeoServer.Models.Datastore
{
    /// <summary>
    /// Datastore.
    /// </summary>
    public class DataStoreInfo
    {
        /// <summary>
        /// Name of data store.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; }

        /// <summary>
        /// Description of data store.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; }

        /// <summary>
        /// Whether or not the data store is enabled.
        /// </summary>
        [JsonPropertyName("enabled")]
        public bool Enabled { get; } = true;

        [JsonPropertyName("connectionParameters")]
        public ConnectionParameters ConnectionParameters { get; }

        [JsonPropertyName("disableOnConnFailure")]
        public bool DisableOnConnFailure { get; }

        [JsonConstructor]
        public DataStoreInfo(string name, string description, bool enabled, ConnectionParameters connectionParameters, bool disableOnConnFailure)
        {
            Name = name;
            Description = description;
            Enabled = enabled;
            ConnectionParameters = connectionParameters;
            DisableOnConnFailure = disableOnConnFailure;
        }
    }
}
