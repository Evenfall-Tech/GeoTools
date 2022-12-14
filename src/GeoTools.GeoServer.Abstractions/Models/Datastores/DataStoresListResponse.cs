using System.Text.Json.Serialization;

namespace GeoTools.GeoServer.Models
{
    /// <summary>
    /// Datastores.
    /// </summary>
    public class DataStoresListResponse
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
