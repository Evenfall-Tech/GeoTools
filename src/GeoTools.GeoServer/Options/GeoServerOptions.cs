using System;
using System.Net.Http;

namespace GeoTools.GeoServer
{
    public class GeoServerOptions
    {
        /// <summary>
        /// If a GeoServer error, like 400/401/500 occurs, return null instead
        /// of throwing an exception.
        /// </summary>
        /// <value>True by default.</value>
        public bool IgnoreServerErrors { get; set; } = true;

        /// <summary>
        /// Avoids logging an exception when the workspace is not present. Note
        /// that 404 status code will still be returned.
        /// </summary>
        /// <value>True by default.</value>
        public bool QuietIfNotFound { get; set; } = true;

        #region HttpClient

        /// <summary>
        /// Base uri address of the server,
        /// e.g. http://localhost:8080/geoserver/rest/.
        /// </summary>
        public Uri BaseAddress { get; set; }

        /// <summary>
        /// Full value of the Authorization header, e.g. Basic AABBCC.
        /// </summary>
        public string AuthorizationHeaderValue { get; set; }

        /// <summary>
        /// If not null, overrides other http-related options.
        /// </summary>
        public Action<IServiceProvider, HttpClient> ConfigureHttpClient { get; set; }

        /// <summary>
        /// Name of the client to get from the factory.
        /// </summary>
        internal const string HttpClientName = nameof(GeoServer);

        #endregion
    }
}
