using GeoTools.GeoServer.Helpers;
using GeoTools.GeoServer.Models;
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
    public class WorkspaceService : IWorkspaceService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<WorkspaceService> _logger;
        private readonly GeoServerOptions _options;
        private readonly JsonSerializerOptions _jsonOpt;

        public WorkspaceService(IHttpClientFactory httpClientFactory, IServiceProvider provider, IOptions<GeoServerOptions> options)
        {
            _httpClientFactory = httpClientFactory;
            _logger = provider.GetService<ILogger<WorkspaceService>>();
            _options = options.Value;
            _jsonOpt = new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement,
            };
        }

        public async Task<GeoServerResponse<Uri>> CreateWorkspaceAsync(WorkspaceInfo workspace, bool? @default, CancellationToken token)
        {
            using (HttpClient client = _httpClientFactory.CreateClient(GeoServerOptions.HttpClientName))
            {
                try
                {
                    var request = new HttpRequestMessage(
                        HttpMethod.Post,
                        "workspaces" + (@default.HasValue ? $"?default={@default.Value}" : ""))
                    {
                        Content = JsonContent.Create(new WorkspaceWrapper(workspace))
                    };

                    var response = await client.SendAsync(request, token);

                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.Created:
                            var result = await response.Content.ReadAsStringAsync();
                            if (result != workspace.Name)
                            {
                                throw new GeoServerClientException((int)response.StatusCode, null,
                                    new ArgumentException(string.Format(
                                        Messages.Value_ComparisonMismatch,
                                        workspace.Name,
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
                        case HttpStatusCode.Conflict:
                            _logger.LogInformation(string.Format(
                                Messages.Request_409Conflict,
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
                                        "{201,401,409}")));
                    }
                }
                catch (GeoServerClientException e)
                {
                    if (_options.IgnoreServerErrors)
                    {
                        _logger.LogWarning(e, nameof(CreateWorkspaceAsync));
                        return new GeoServerResponse<Uri>(e.StatusCode, null);
                    }
                    else
                    {
                        _logger.LogError(e, nameof(CreateWorkspaceAsync));
                        throw e.InnerException;
                    }
                }
            }
        }

        public async Task<GeoServerResponse<bool>> DeleteWorkspaceAsync(string name, bool? recurse, CancellationToken token)
        {
            using (HttpClient client = _httpClientFactory.CreateClient(GeoServerOptions.HttpClientName))
            {
                try
                {
                    var request = new HttpRequestMessage(
                        HttpMethod.Delete,
                        $"workspaces/{name}" + (recurse.HasValue ? $"?recurse={recurse}" : ""));

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
                                        "{200,401,403,404,405}")));
                    }
                }
                catch (GeoServerClientException e)
                {
                    if (_options.IgnoreServerErrors)
                    {
                        _logger.LogWarning(e, nameof(DeleteWorkspaceAsync));
                        return new GeoServerResponse<bool>(e.StatusCode, false);
                    }
                    else
                    {
                        _logger.LogError(e, nameof(DeleteWorkspaceAsync));
                        throw e.InnerException;
                    }
                }
            }
        }

        public async Task<GeoServerResponse<WorkspaceSummary>> GetWorkspaceAsync(string name, CancellationToken token)
        {
            using (HttpClient client = _httpClientFactory.CreateClient(GeoServerOptions.HttpClientName))
            {
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, $"workspaces/{name}?quietOnNotFound={_options.QuietIfNotFound}");

                    var response = await client.SendAsync(request, token);

                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            var result = await response.Content.ReadFromJsonAsync<GetWorkspaceResponse>(_jsonOpt, token);
                            _logger.LogInformation(string.Format(
                                Messages.Request_200OK,
                                request.RequestUri));
                            return new GeoServerResponse<WorkspaceSummary>((int)response.StatusCode, result.Workspace);
                        case HttpStatusCode.NotFound:
                            _logger.LogInformation(string.Format(
                                Messages.Request_404NotFound,
                                request.RequestUri));
                            return new GeoServerResponse<WorkspaceSummary>((int)response.StatusCode, null);
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
                        _logger.LogWarning(e, nameof(GetWorkspaceAsync));
                        return new GeoServerResponse<WorkspaceSummary>(e.StatusCode, null);
                    }
                    else
                    {
                        _logger.LogError(e, nameof(GetWorkspaceAsync));
                        throw e.InnerException;
                    }
                }
            }
        }

        public async Task<GeoServerResponse<IList<NamedLink>>> GetWorkspacesAsync(CancellationToken token)
        {
            using (HttpClient client = _httpClientFactory.CreateClient(GeoServerOptions.HttpClientName))
            {
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, $"workspaces");

                    var response = await client.SendAsync(request, token);

                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            var result = await response.Content.ReadFromJsonAsync<WorkspacesResponse>(
                                _jsonOpt, token);
                            _logger.LogInformation(string.Format(
                                Messages.Request_200OK,
                                request.RequestUri));
                            return new GeoServerResponse<IList<NamedLink>>((int)response.StatusCode, result.Workspaces.Workspace);
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
                                        "{200,401}")));
                    }
                }
                catch (GeoServerClientException e)
                {
                    if (_options.IgnoreServerErrors)
                    {
                        _logger.LogWarning(e, nameof(GetWorkspacesAsync));
                        return new GeoServerResponse<IList<NamedLink>>(e.StatusCode, null);
                    }
                    else
                    {
                        _logger.LogError(e, nameof(GetWorkspacesAsync));
                        throw e.InnerException;
                    }
                }
            }
        }

        public async Task<GeoServerResponse<bool>> UpdateWorkspaceAsync(string name, WorkspaceInfo workspace, CancellationToken token)
        {
            using (HttpClient client = _httpClientFactory.CreateClient(GeoServerOptions.HttpClientName))
            {
                try
                {
                    var request = new HttpRequestMessage(
                        HttpMethod.Put,
                        $"workspaces/{name}")
                    {
                        Content = JsonContent.Create(new WorkspaceWrapper(workspace)),
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
                        _logger.LogWarning(e, nameof(UpdateWorkspaceAsync));
                        return new GeoServerResponse<bool>(e.StatusCode, false);
                    }
                    else
                    {
                        _logger.LogError(e, nameof(UpdateWorkspaceAsync));
                        throw e.InnerException;
                    }
                }
            }
        }
    }
}
