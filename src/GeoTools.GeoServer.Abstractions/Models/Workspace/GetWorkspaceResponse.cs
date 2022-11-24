namespace GeoTools.GeoServer.Models
{
    public class GetWorkspaceResponse
    {
        public WorkspaceSummary Workspace { get; }

        public GetWorkspaceResponse(WorkspaceSummary workspace)
        {
            Workspace = workspace;
        }
    }
}
