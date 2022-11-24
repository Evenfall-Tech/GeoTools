using System.Text.Json.Serialization;

namespace GeoTools.GeoServer.Models
{
    public class NamedLink
    {
        [JsonPropertyName("class")]
        public string Class { get; }

        [JsonPropertyName("name")]
        public string Name { get; }

        [JsonPropertyName("href")]
        public string Href { get; }

        [JsonPropertyName("link")]
        public string Link { get; }

        [JsonConstructor]
        public NamedLink(
            string @class = null,
            string name = null,
            string href = null,
            string link = null
            )
        {
            Class = @class;
            Name = name;
            Href = href;
            Link = link;
        }
    }
}
