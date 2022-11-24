namespace GeoTools.GeoServer.Models
{
    /// <summary>
    /// Wrapper object around Workspace, in order to conform to how XStream serializes to JSON in GeoServer.
    /// </summary>
    public class WorkspaceWrapper
    {
        public WorkspaceInfo Workspace { get; }

        public WorkspaceWrapper(WorkspaceInfo workspace)
        {
            Workspace = workspace;
        }
    }
}
