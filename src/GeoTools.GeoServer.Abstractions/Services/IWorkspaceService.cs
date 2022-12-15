using GeoTools.GeoServer.Models;
using GeoTools.GeoServer.Models.CatalogResponses;
using GeoTools.GeoServer.Models.Workspace;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GeoTools.GeoServer.Services
{
    public interface IWorkspaceService
    {
        /// <summary>
        /// Add a new workspace to GeoServer.
        /// </summary>
        /// <param name="workspace">The Workspace body information to upload.</param>
        /// <param name="default">
        /// New workspace will be the used as the default.
        /// Allowed values are true or false. The default value is false.
        /// </param>
        /// <param name="token"></param>
        /// <returns>Adds a new workspace to the server. URL where the newly created workspace can be found, null otherwise.</returns>
        /// <remarks>
        /// <para>201: Created. Reurns true.</para>
        /// <para>401: Missing auth configuration. Returns false / <seealso cref="UnauthorizedAccessException"/>.</para>
        /// <para>409: Unable to add workspace as it already exists. Returns false.</para>
        /// <para>Def: Returns false / <seealso cref="ArgumentOutOfRangeException"/>.</para>
        /// </remarks>
        /// <exception cref="UnauthorizedAccessException"/>
        Task<GeoServerResponse<Uri>> CreateWorkspaceAsync(WorkspaceInfo workspace, bool? @default, CancellationToken token);

        /// <summary>
        /// Get a list of workspaces.
        /// </summary>
        /// <param name="token"></param>
        /// <returns>Displays a list of all workspaces on the server.</returns>
        /// <remarks>
        /// <para>200: OK. Returns the list of models.</para>
        /// <para>401: Missing auth configuration. Returns null / <seealso cref="UnauthorizedAccessException"/>.</para>
        /// <para>Def: Returns null / <seealso cref="ArgumentOutOfRangeException"/>.</para>
        /// </remarks>
        Task<GeoServerResponse<IList<NamedLink>>> GetWorkspacesAsync(CancellationToken token);

        /// <summary>
        /// Retrieve a Workspace.
        /// </summary>
        /// <param name="name">The name of the workspace to fetch.</param>
        /// <param name="token"></param>
        /// <returns>Retrieves a single workspace definition.</returns>
        /// <remarks>
        /// <para>200: OK. Returns the model.</para>
        /// <para>401: Missing auth configuration. Returns null / <seealso cref="UnauthorizedAccessException"/>.</para>
        /// <para>404: Workspace does not exist. Returns null.</para>
        /// <para>Def: Returns null / <seealso cref="ArgumentOutOfRangeException"/>.</para>
        /// </remarks>
        Task<GeoServerResponse<WorkspaceSummary>> GetWorkspaceAsync(string name, CancellationToken token);

        /// <summary>
        /// Delete a Workspace.
        /// </summary>
        /// <param name="name">The name of the workspace to delete.</param>
        /// <param name="recurse">Delete workspace contents (default false).</param>
        /// <param name="token"></param>
        /// <returns>Deletes a single workspace definition.</returns>
        /// <remarks>
        /// <para>200: Success workspace deleted. Returns true.</para>
        /// <para>401: Missing auth configuration. Returns false / <seealso cref="UnauthorizedAccessException"/>.</para>
        /// <para>403: Workspace or related Namespace is not empty (and recurse not true). Returns false.</para>
        /// <para>404: Workspace doesn't exist. Returns false.</para>
        /// <para>405: Can't delete default workspace. Returns false.</para>
        /// <para>Def: Returns false / <seealso cref="ArgumentOutOfRangeException"/>.</para>
        /// </remarks>
        Task<GeoServerResponse<bool>> DeleteWorkspaceAsync(string name, bool? recurse, CancellationToken token);

        /// <summary>
        /// Update a workspace.
        /// </summary>
        /// <param name="name">The old name of the workspace to update.</param>
        /// <param name="workspace">Takes the body of the post and modifies the workspace from it.</param>
        /// <param name="token"></param>
        /// <returns>Updates a single workspace definition.</returns>
        /// <remarks>
        /// <para>200: Modified. Returns true.</para>
        /// <para>401: Missing auth configuration. Returns false / <seealso cref="UnauthorizedAccessException"/>.</para>
        /// <para>404: Workspace not found. Returns false.</para>
        /// <para>405: Forbidden to change the name of the workspace. Returns false.</para>
        /// <para>Def: Returns false / <seealso cref="ArgumentOutOfRangeException"/>.</para>
        /// </remarks>
        Task<GeoServerResponse<bool>> UpdateWorkspaceAsync(string name, WorkspaceInfo workspace, CancellationToken token);
    }
}
