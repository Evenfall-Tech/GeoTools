namespace GeoTools.GeoServer.Models
{
    /// <summary>
    /// Workspace Response.
    /// </summary>
    public class WorkspaceSummary
    {
        /// <summary>
        /// Name of workspace.
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

        /// <summary>
        /// URL to Datas tores in this workspace.
        /// </summary>
        public string DataStores { get; }

        /// <summary>
        /// URL to Coverage stores in this workspace.
        /// </summary>
        public string CoverageStores { get; }

        /// <summary>
        /// URL to WMS stores in this workspace.
        /// </summary>
        public string WmsStores { get; }

        public WorkspaceSummary(string name, string dataStores, string coverageStores, string wmsStores, bool isolated = false)
        {
            Name = name;
            Isolated = isolated;
            DataStores = dataStores;
            CoverageStores = coverageStores;
            WmsStores = wmsStores;
        }
    }
}
