using GeoTools.GeoServer.Models;
using GeoTools.GeoServer.Models.CatalogResponses;
using GeoTools.GeoServer.Models.Datastore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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

        /// <summary>
        /// Retrieve a particular data store from a workspace.
        /// </summary>
        /// <param name="workspaceName">The name of the worskpace containing the data store.</param>
        /// <param name="datastoreName">The name of the data store to retrieve.</param>
        /// <param name="token"></param>
        /// <returns>Controls a particular data store in a given workspace.</returns>
        /// <remarks>
        /// <para>200: OK. Returns the model.</para>
        /// <para>401: Missing auth configuration. Returns null / <seealso cref="UnauthorizedAccessException"/>.</para>
        /// <para>404: Workspace or datastore does not exist. Returns null.</para>
        /// <para>Def: Returns null / <seealso cref="ArgumentOutOfRangeException"/>.</para>
        /// </remarks>
        Task<GeoServerResponse<DataStoreSummary>> GetDatastoreAsync(string workspaceName, string datastoreName, CancellationToken token);

        /// <summary>
        /// Create a new data store.
        /// </summary>
        /// <param name="workspaceName">The name of the worskpace containing the data stores.</param>
        /// <param name="datastoreInfo">
        /// The data store body information to upload.
        /// The contents of the connection parameters will differ depending on the type of data store being added.
        /// </param>
        /// <param name="token"></param>
        /// <returns>Adds a new data store to the workspace.</returns>
        /// <remarks>
        /// <para>201: Created. Returns true if created successfully, false if already exists.</para>
        /// <para>401: Missing auth configuration. Returns null / <seealso cref="UnauthorizedAccessException"/>.</para>
        /// <para>500: Workspace not found. Returns null.</para>
        /// <para>Def: Returns null / <seealso cref="ArgumentOutOfRangeException"/>.</para>
        /// See examples at https://github.com/geoserver/geoserver/blob/main/src/community/rest-openapi/openapi/src/main/resources/org/geoserver/rest/openapi/1.0.0/datastores.yaml#L61.
        /// </remarks>
        Task<GeoServerResponse<Uri>> CreateDatastoreAsync(string workspaceName, DataStoreInfo datastoreInfo, CancellationToken token);

        /// <summary>
        /// Delete data store.
        /// </summary>
        /// <param name="workspaceName">The name of the worskpace containing the data store.</param>
        /// <param name="datastoreName">The name of the data store to delete.</param>
        /// <param name="recurse">
        /// The recurse controls recursive deletion. When set to true all
        /// resources contained in the store are also removed.The default value
        /// is "false".
        /// </param>
        /// <param name="token"></param>
        /// <returns>Deletes a data store from the server.</returns>
        /// <remarks>
        /// <para>200: Success datastore deleted. Returns true.</para>
        /// <para>401: Missing auth configuration. Returns false / <seealso cref="UnauthorizedAccessException"/>.</para>
        /// <para>403: Datastore is not empty (and recurse not true). Returns false.</para>
        /// <para>404: Workspace or datastore doesn't exist. Returns false.</para>
        /// <para>Def: Returns false / <seealso cref="ArgumentOutOfRangeException"/>.</para>
        /// </remarks>
        Task<GeoServerResponse<bool>> DeleteDatastoreAsync(string workspaceName, string datastoreName, bool? recurse, CancellationToken token);

        /// <summary>
        /// Modify a data store.
        /// </summary>
        /// <param name="workspaceName">The name of the worskpace containing the data store.</param>
        /// <param name="datastore">
        /// The updated data store definition.
        /// For a PUT, only values which should be changed need to be included.
        /// The connectionParameters map counts as a single value,
        /// so if you change it all preexisting connection parameters will be
        /// overwritten.
        /// The contents of the connection parameters will differ depending on the
        /// type of data store being added.
        /// </param>
        /// <param name="token"></param>
        /// <returns>Modify data store ds.</returns>
        /// <remarks>
        /// <para>200: Modified. Returns true.</para>
        /// <para>401: Missing auth configuration. Returns false / <seealso cref="UnauthorizedAccessException"/>.</para>
        /// <para>404: Workspace not found. Returns false.</para>
        /// <para>405: Forbidden to change the name of the datastore. Returns false.</para>
        /// <para>Def: Returns false / <seealso cref="ArgumentOutOfRangeException"/>.</para>
        /// </remarks>
        Task<GeoServerResponse<bool>> UpdateDatastoreAsync(string workspaceName, DataStoreInfo datastore, CancellationToken token);
    }
}
