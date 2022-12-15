using System.Text.Json.Serialization;

namespace GeoTools.GeoServer.Models.Datastore
{
    /// <summary>
    /// Wrapper object for DataStoreSummary, in order to comply with current API encoding.
    /// </summary>
    internal class DataStoreSummaryWrapper
    {
        [JsonPropertyName("dataStore")]
        public DataStoreSummary DataStore { get; }

        [JsonConstructor]
        public DataStoreSummaryWrapper(DataStoreSummary dataStore)
        {
            DataStore = dataStore;
        }
    }
}
