using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using GeoTools.GeoServer.Models;
using System.Threading;
using GeoTools.GeoServer.Helpers;
using GeoTools.GeoServer.Resources;
using System.Net.Http.Json;
using System.Net;
using System.Xml.Linq;

namespace GeoTools.GeoServer.Services
{
    public class DatastoreService : IDatastoreService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<DatastoreService> _logger;
        private readonly GeoServerOptions _options;
        private readonly JsonSerializerOptions _jsonOpt;

        public DatastoreService(IHttpClientFactory httpClientFactory, IServiceProvider provider, IOptions<GeoServerOptions> options)
        {
            _httpClientFactory = httpClientFactory;
            _logger = provider.GetService<ILogger<DatastoreService>>();
            _options = options.Value;
            _jsonOpt = new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement,
            };
        }

        public async Task<GeoServerResponse<IList<NamedLink>>> GetDatastoresAsync(string workspaceName, CancellationToken token)
        {
            using (HttpClient client = _httpClientFactory.CreateClient(GeoServerOptions.HttpClientName))
            {
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, $"workspaces/{workspaceName}/datastores");

                    var response = await client.SendAsync(request, token);

                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            var result = await response.Content.ReadFromJsonAsync<DataStoresListResponse>(_jsonOpt, token);
                            _logger.LogInformation(string.Format(
                                Messages.Request_200OK,
                                request.RequestUri));
                            return new GeoServerResponse<IList<NamedLink>>((int)response.StatusCode, result.DataStores.DataStore);
                        case HttpStatusCode.NotFound:
                            _logger.LogInformation(string.Format(
                                Messages.Request_404NotFound,
                                request.RequestUri));
                            return new GeoServerResponse<IList<NamedLink>>((int)response.StatusCode, null);
                        case HttpStatusCode.Unauthorized:
                            throw new GeoServerClientException((int)response.StatusCode, null,
                                new UnauthorizedAccessException(string.Format(
                                    Messages.Request_401Unauthorized,
                                    request.RequestUri)));
                        default:
                            throw new GeoServerClientException((int)response.StatusCode, null,
                                new ArgumentOutOfRangeException(
                                    nameof(response.StatusCode),
                                    response.StatusCode,
                                    string.Format(
                                        Messages.Value_OutOfRange,
                                        nameof(response.StatusCode),
                                        "{200,401,404}")));
                    }
                }
                catch (GeoServerClientException e)
                {
                    if (_options.IgnoreServerErrors)
                    {
                        _logger.LogWarning(e, nameof(GetDatastoresAsync));
                        return new GeoServerResponse<IList<NamedLink>>(e.StatusCode, null);
                    }
                    else
                    {
                        _logger.LogError(e, nameof(GetDatastoresAsync));
                        throw e.InnerException;
                    }
                }
            }
        }
    }
}
