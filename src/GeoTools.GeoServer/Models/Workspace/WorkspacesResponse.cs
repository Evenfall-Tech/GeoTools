using System.Text.Json.Serialization;

namespace GeoTools.GeoServer.Models
{
    internal class WorkspacesResponse
    {
        /// <summary>
        /// WorkspaceResponseWrapper.
        /// </summary>
        [JsonPropertyName("workspaces")]
        public WorkspaceResponseWrapper Workspaces { get; }

        [JsonConstructor]
        public WorkspacesResponse(WorkspaceResponseWrapper workspaces)
        {
            Workspaces = workspaces;
        }
    }
}
