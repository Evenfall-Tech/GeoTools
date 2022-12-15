using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GeoTools.GeoServer.Models.Catalog
{
    public class ConnectionParameters
    {
        [JsonPropertyName("entry")]
        public IList<ConnectionParameter> ConnectionParameterList { get; }

        [JsonConstructor]
        public ConnectionParameters(IList<ConnectionParameter> connectionParameters)
        {
            ConnectionParameterList = connectionParameters;
        }
    }
}
