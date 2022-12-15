using System.Text.Json.Serialization;

namespace GeoTools.GeoServer.Models.Datastore
{
    /// <summary>
    /// Wrapper object for DataStoreInfo, in order to comply with current API encoding.
    /// </summary>
    internal class DataStoreInfoWrapper
    {
        [JsonPropertyName("dataStore")]
        public DataStoreInfo DataStore { get; }

        [JsonConstructor]
        public DataStoreInfoWrapper(DataStoreInfo dataStore)
        {
            DataStore = dataStore;
        }
    }
}
