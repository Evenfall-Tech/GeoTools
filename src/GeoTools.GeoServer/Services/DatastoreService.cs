using GeoTools.GeoServer.Helpers;
using GeoTools.GeoServer.Helpers.Converters;
using GeoTools.GeoServer.Models;
using GeoTools.GeoServer.Models.CatalogResponses;
using GeoTools.GeoServer.Models.Datastore;
using GeoTools.GeoServer.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

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
                Converters =
                {
                    new ConnectionParametersConverter(),
                }
            };
        }

        public async Task<GeoServerResponse<Uri>> CreateDatastoreAsync(string workspaceName, DataStoreInfo datastoreInfo, CancellationToken token)
        {
            using (HttpClient client = _httpClientFactory.CreateClient(GeoServerOptions.HttpClientName))
            {
                try
                {
                    var request = new HttpRequestMessage(
                        HttpMethod.Post,
                        $"workspaces/{workspaceName}/datastores")
                    {
                        Content = JsonContent.Create(new DataStoreInfoWrapper(datastoreInfo))
                    };

                    var response = await client.SendAsync(request, token);

                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.Created:
                            var result = await response.Content.ReadAsStringAsync();
                            if (result != datastoreInfo.Name)
                            {
                                throw new GeoServerClientException((int)response.StatusCode, null,
                                    new ArgumentException(string.Format(
                                        Messages.Value_ComparisonMismatch,
                                        datastoreInfo.Name,
                                        result)));
                            }
                            else if (response.Headers.Location == null)
                            {
                                throw new GeoServerClientException((int)response.StatusCode, null,
                                    new ArgumentException(string.Format(
                                        Messages.Value_Null,
                                        nameof(response.Headers.Location))));
                            }
                            else
                            {
                                _logger.LogInformation(string.Format(
                                    Messages.Request_201Created,
                                    request.RequestUri));
                                return new GeoServerResponse<Uri>((int)response.StatusCode, response.Headers.Location);
                            }
                        case HttpStatusCode.InternalServerError:
                            _logger.LogInformation(string.Format(
                                Messages.Request_500InternalServerError,
                                request.RequestUri));
                            return new GeoServerResponse<Uri>((int)response.StatusCode, null);
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
                                        "{201,401,500}")));
                    }
                }
                catch (GeoServerClientException e)
                {
                    if (_options.IgnoreServerErrors)
                    {
                        _logger.LogWarning(e, nameof(CreateDatastoreAsync));
                        return new GeoServerResponse<Uri>(e.StatusCode, null);
                    }
                    else
                    {
                        _logger.LogError(e, nameof(CreateDatastoreAsync));
                        throw e.InnerException;
                    }
                }
            }
        }

        public async Task<GeoServerResponse<bool>> DeleteDatastoreAsync(string workspaceName, string datastoreName, bool? recurse, CancellationToken token)
        {
            using (HttpClient client = _httpClientFactory.CreateClient(GeoServerOptions.HttpClientName))
            {
                try
                {
                    var request = new HttpRequestMessage(
                        HttpMethod.Delete,
                        $"workspaces/{workspaceName}/datastores/{datastoreName}" + (recurse.HasValue ? $"?recurse={recurse}" : ""));

                    var response = await client.SendAsync(request, token);

                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            _logger.LogInformation(string.Format(
                                Messages.Request_200OK,
                                request.RequestUri));
                            return new GeoServerResponse<bool>((int)response.StatusCode, true);
                        case HttpStatusCode.NotFound:
                            _logger.LogInformation(string.Format(
                                Messages.Request_404NotFound,
                                request.RequestUri));
                            return new GeoServerResponse<bool>((int)response.StatusCode, false);
                        case HttpStatusCode.Forbidden:
                            _logger.LogInformation(string.Format(
                                Messages.Request_403Forbidden,
                                request.RequestUri));
                            return new GeoServerResponse<bool>((int)response.StatusCode, false);
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
                                        "{200,401,403,404}")));
                    }
                }
                catch (GeoServerClientException e)
                {
                    if (_options.IgnoreServerErrors)
                    {
                        _logger.LogWarning(e, nameof(DeleteDatastoreAsync));
                        return new GeoServerResponse<bool>(e.StatusCode, false);
                    }
                    else
                    {
                        _logger.LogError(e, nameof(DeleteDatastoreAsync));
                        throw e.InnerException;
                    }
                }
            }
        }

        public async Task<GeoServerResponse<DataStoreSummary>> GetDatastoreAsync(string workspaceName, string datastoreName, CancellationToken token)
        {
            using (HttpClient client = _httpClientFactory.CreateClient(GeoServerOptions.HttpClientName))
            {
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, $"workspaces/{workspaceName}/datastores/{datastoreName}?quietOnNotFound={_options.QuietIfNotFound}");

                    var response = await client.SendAsync(request, token);

                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            var result = await response.Content.ReadFromJsonAsync<DataStoreSummaryWrapper>(_jsonOpt, token);
                            _logger.LogInformation(string.Format(
                                Messages.Request_200OK,
                                request.RequestUri));
                            return new GeoServerResponse<DataStoreSummary>((int)response.StatusCode, result.DataStore);
                        case HttpStatusCode.NotFound:
                            _logger.LogInformation(string.Format(
                                Messages.Request_404NotFound,
                                request.RequestUri));
                            return new GeoServerResponse<DataStoreSummary>((int)response.StatusCode, null);
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
                        _logger.LogWarning(e, nameof(GetDatastoreAsync));
                        return new GeoServerResponse<DataStoreSummary>(e.StatusCode, null);
                    }
                    else
                    {
                        _logger.LogError(e, nameof(GetDatastoreAsync));
                        throw e.InnerException;
                    }
                }
            }
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

        public async Task<GeoServerResponse<bool>> UpdateDatastoreAsync(string workspaceName, DataStoreInfo datastore, CancellationToken token)
        {
            using (HttpClient client = _httpClientFactory.CreateClient(GeoServerOptions.HttpClientName))
            {
                try
                {
                    var request = new HttpRequestMessage(
                        HttpMethod.Put,
                        $"workspaces/{workspaceName}/datastores/{datastore.Name}")
                    {
                        Content = JsonContent.Create(new DataStoreInfoWrapper(datastore)),
                    };

                    var response = await client.SendAsync(request, token);

                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            _logger.LogInformation(string.Format(
                                Messages.Request_200OK,
                                request.RequestUri));
                            return new GeoServerResponse<bool>((int)response.StatusCode, true);
                        case HttpStatusCode.NotFound:
                            _logger.LogInformation(string.Format(
                                Messages.Request_404NotFound,
                                request.RequestUri));
                            return new GeoServerResponse<bool>((int)response.StatusCode, false);
                        case HttpStatusCode.MethodNotAllowed:
                            _logger.LogInformation(string.Format(
                                Messages.Request_405MethodNotAllowed,
                                request.RequestUri));
                            return new GeoServerResponse<bool>((int)response.StatusCode, false);
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
                                        "{200,401,404,405}")));
                    }
                }
                catch (GeoServerClientException e)
                {
                    if (_options.IgnoreServerErrors)
                    {
                        _logger.LogWarning(e, nameof(UpdateDatastoreAsync));
                        return new GeoServerResponse<bool>(e.StatusCode, false);
                    }
                    else
                    {
                        _logger.LogError(e, nameof(UpdateDatastoreAsync));
                        throw e.InnerException;
                    }
                }
            }
        }
    }
}
