using GeoTools.GeoServer.Models;
using GeoTools.GeoServer.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace GeoTools.GeoServer.Services
{
    public class WorkspaceService : IWorkspaceService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<WorkspaceService> _logger;
        private readonly GeoServerOptions _options;

        public WorkspaceService(IHttpClientFactory httpClientFactory, IServiceProvider provider, IOptions<GeoServerOptions> options)
        {
            _httpClientFactory = httpClientFactory;
            _logger = provider.GetService<ILogger<WorkspaceService>>();
            _options = options.Value;
        }

        public async Task<WorkspaceSummary> GetWorkspaceAsync(string name, CancellationToken token)
        {
            using (HttpClient client = _httpClientFactory.CreateClient(GeoServerOptions.HttpClientName))
            {
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, $"workspaces/{name}?quietOnNotFound={_options.QuietIfNotFound}");
                    request.Headers.Accept.Clear();
                    request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));

                    var response = await client.SendAsync(request, token);

                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            var result = await response.Content.ReadFromJsonAsync<GetWorkspaceResponse>(cancellationToken: token);
                            _logger.LogInformation(string.Format(
                                ExceptionMessages.Request_OK,
                                request.RequestUri));
                            return result.Workspace;
                        case HttpStatusCode.NotFound:
                            _logger.LogInformation(string.Format(
                                ExceptionMessages.Request_NotFound,
                                request.RequestUri));
                            return null;
                        case HttpStatusCode.Unauthorized:
                            throw new UnauthorizedAccessException(string.Format(
                                ExceptionMessages.Request_Unauthorized,
                                request.RequestUri));
                        default:
                            throw new ArgumentOutOfRangeException(
                                nameof(response.StatusCode),
                                response.StatusCode,
                                string.Format(
                                    ExceptionMessages.Value_OutOfRange,
                                    nameof(response.StatusCode),
                                    "{200,401,404}"));
                    }
                }
                catch (UnauthorizedAccessException e)
                {
                    _logger.LogError(e, nameof(GetWorkspaceAsync));
                    throw;
                }
                catch (Exception e)
                {
                    if (_options.IgnoreServerErrors)
                    {
                        _logger.LogWarning(e, nameof(GetWorkspaceAsync));
                        return null;
                    }
                    else
                    {
                        _logger.LogError(e, nameof(GetWorkspaceAsync));
                        throw;
                    }
                }
            }
        }
    }
}
