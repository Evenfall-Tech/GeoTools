using System.Text.Json.Serialization;

namespace GeoTools.GeoServer.Models.Datastore
{
    /// <summary>
    /// Datastores.
    /// </summary>
    internal class DataStoresListResponse
    {
        [JsonPropertyName("dataStores")]
        public DataStoreListWrapper DataStores { get; }

        [JsonConstructor]
        public DataStoresListResponse(DataStoreListWrapper dataStores)
        {
            DataStores = dataStores;
        }
    }
}
