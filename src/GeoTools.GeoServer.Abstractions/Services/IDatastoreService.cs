using GeoTools.GeoServer.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace GeoTools.GeoServer.Services
{
    public interface IDatastoreService
    {

        /// <summary>
        /// Get a list of data stores.
        /// </summary>
        /// <param name="workspaceName">The name of the worskpace containing the data stores.</param>
        /// <param name="token"></param>
        /// <returns>List all data stores in workspace ws.</returns>
        /// <remarks>
        /// <para>200: OK. Returns the list of models.</para>
        /// <para>401: Missing auth configuration. Returns null / <seealso cref="UnauthorizedAccessException"/>.</para>
        /// <para>404: Workspace not found.</para>
        /// <para>Def: Returns null / <seealso cref="ArgumentOutOfRangeException"/>.</para>
        /// </remarks>
        Task<GeoServerResponse<IList<NamedLink>>> GetDatastoresAsync(string workspaceName, CancellationToken token);
    }
}
