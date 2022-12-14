using System.Text.Json.Serialization;

namespace GeoTools.GeoServer.Models
{
    /// <summary>
    /// Wrapper object around Workspace, in order to conform to how XStream serializes to JSON in GeoServer.
    /// </summary>
    internal class WorkspaceWrapper
    {
        [JsonPropertyName("workspace")]
        public WorkspaceInfo Workspace { get; }

        [JsonConstructor]
        public WorkspaceWrapper(WorkspaceInfo workspace)
        {
            Workspace = workspace;
        }
    }
}
