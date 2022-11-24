namespace GeoTools.GeoServer.Models
{
    /// <summary>
    /// Workspace.
    /// </summary>
    public class WorkspaceInfo
    {
        /// <summary>
        /// Name of the workspace.
        /// </summary>
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
        public bool Isolated { get; }

        public WorkspaceInfo(string name, bool isolated = false)
        {
            Name = name;
            Isolated = isolated;
        }
    }
}
