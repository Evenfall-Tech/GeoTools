using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GeoTools.GeoServer.Models
{
    /// <summary>
    /// Wrapper object in order to comply with current API encoding.
    /// </summary>
    public class DataStoreListWrapper
    {
        [JsonPropertyName("dataStore")]
        public IList<NamedLink> DataStore { get; }

        [JsonConstructor]
        public DataStoreListWrapper(IList<NamedLink> dataStore)
        {
            DataStore = dataStore;
        }
    }
}
