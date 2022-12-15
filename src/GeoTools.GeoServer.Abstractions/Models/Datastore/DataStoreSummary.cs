using GeoTools.GeoServer.Models.Catalog;
using GeoTools.GeoServer.Models.CatalogResponses;
using System;
using System.Text.Json.Serialization;

namespace GeoTools.GeoServer.Models.Datastore
{
    /// <summary>
    /// Datastore.
    /// </summary>
    public class DataStoreSummary
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

        [JsonPropertyName("workspace")]
        public NamedLink Workspace { get; }

        [JsonPropertyName("_default")]
        public bool Default { get; }

        [JsonPropertyName("disableOnConnFailure")]
        public bool DisableOnConnFailure { get; }

        [JsonPropertyName("featureTypes")]
        public Uri FeatureTypes { get; }

        [JsonConstructor]
        public DataStoreSummary(string name, string description, bool enabled, ConnectionParameters connectionParameters, NamedLink workspace, bool @default, bool disableOnConnFailure, Uri featureTypes)
        {
            Name = name;
            Description = description;
            Enabled = enabled;
            ConnectionParameters = connectionParameters;
            Workspace = workspace;
            Default = @default;
            DisableOnConnFailure = disableOnConnFailure;
            FeatureTypes = featureTypes;
        }
    }
}
