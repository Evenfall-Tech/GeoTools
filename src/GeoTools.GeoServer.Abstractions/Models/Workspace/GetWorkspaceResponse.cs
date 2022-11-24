using System.Text.Json.Serialization;

namespace GeoTools.GeoServer.Models
{
    public class GetWorkspaceResponse
    {
        [JsonPropertyName("workspace")]
        public WorkspaceSummary Workspace { get; }

        [JsonConstructor]
        public GetWorkspaceResponse(WorkspaceSummary workspace)
        {
            Workspace = workspace;
        }
    }
}
