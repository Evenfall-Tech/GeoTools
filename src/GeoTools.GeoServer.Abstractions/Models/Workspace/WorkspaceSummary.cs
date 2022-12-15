using System;
using System.Text.Json.Serialization;

namespace GeoTools.GeoServer.Models.Workspace
{
    /// <summary>
    /// Workspace Response.
    /// </summary>
    public class WorkspaceSummary
    {
        /// <summary>
        /// Name of workspace.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; }

        /// <summary>
        /// Is the workspace content isolated so it is not included as part of
        /// global web services.
        /// </summary>
        /// <remarks>
        /// Isolated workspaces content is only visible and queryable in the
        /// context of a virtual service bound to the isolated workspace.
        /// </remarks>
        /// <value>False by default.</value>
        [JsonPropertyName("isolated")]
        public bool Isolated { get; }

        /// <summary>
        /// URL to Datas tores in this workspace.
        /// </summary>
        [JsonPropertyName("dataStores")]
        public Uri DataStores { get; }

        /// <summary>
        /// URL to Coverage stores in this workspace.
        /// </summary>
        [JsonPropertyName("coverageStores")]
        public Uri CoverageStores { get; }

        /// <summary>
        /// URL to WMS stores in this workspace.
        /// </summary>
        [JsonPropertyName("wmsStores")]
        public Uri WmsStores { get; }

        /// <summary>
        /// URL to WMTS stores in this workspace.
        /// </summary>
        [JsonPropertyName("wmtsStores")]
        public Uri WmtsStores { get; }

        [JsonConstructor]
        public WorkspaceSummary(string name, Uri dataStores, Uri coverageStores, Uri wmsStores, Uri wmtsStores, bool isolated = false)
        {
            Name = name;
            Isolated = isolated;
            DataStores = dataStores;
            CoverageStores = coverageStores;
            WmsStores = wmsStores;
            WmtsStores = wmtsStores;
        }
    }
}
