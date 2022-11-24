using GeoTools.GeoServer.Models;
using System.Threading;
using System.Threading.Tasks;

namespace GeoTools.GeoServer.Services
{
    public interface IWorkspaceService
    {
        /// <summary>
        /// Retrieve a Workspace.
        /// </summary>
        /// <param name="name">The name of the workspace to fetch.</param>
        /// <param name="token"></param>
        /// <returns>Retrieves a single workspace definition.</returns>
        Task<WorkspaceSummary> GetWorkspaceAsync(string name, CancellationToken token);
    }
}
