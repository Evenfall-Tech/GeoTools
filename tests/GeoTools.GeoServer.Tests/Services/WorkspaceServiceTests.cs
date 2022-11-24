using GeoTools.GeoServer.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text;

namespace GeoTools.GeoServer.Tests.Services
{
    public class WorkspaceServiceTests
    {
        private readonly ITestOutputHelper _output;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        [Fact]
        public async Task GivenDefaultInstallation_GetWorkspace()
        {
            CancellationToken token = default;

            var service = _serviceProvider.GetRequiredService<IWorkspaceService>();

            var responseWrapper = await service.GetWorkspaceAsync("ne", token);
            Assert.NotNull(responseWrapper);
            Assert.Equal(200, responseWrapper.StatusCode);

            var response = responseWrapper.Response;
            Assert.NotNull(response);
            Assert.False(string.IsNullOrWhiteSpace(response.Name));
            Assert.False(string.IsNullOrWhiteSpace(response.CoverageStores));
            Assert.False(string.IsNullOrWhiteSpace(response.DataStores));
            Assert.False(string.IsNullOrWhiteSpace(response.WmsStores));

            responseWrapper = await service.GetWorkspaceAsync("ne1", token);
            Assert.NotNull(responseWrapper);
            Assert.Equal(404, responseWrapper.StatusCode);
            Assert.Null(responseWrapper.Response);
        }

        [Fact]
        public async Task GivenDefaultInstallation_GetWorkspaces()
        {
            CancellationToken token = default;

            var service = _serviceProvider.GetRequiredService<IWorkspaceService>();

            var responseWrapper = await service.GetWorkspacesAsync(token);

            Assert.NotNull(responseWrapper);
            Assert.Equal(200, responseWrapper.StatusCode);

            var response = responseWrapper.Response;
            Assert.NotEmpty(response);
            Assert.All(response, x =>
            {
                Assert.False(string.IsNullOrWhiteSpace(x.Name));
                Assert.False(string.IsNullOrWhiteSpace(x.Href));
            });
        }

        [Fact]
        public async Task GivenDefaultInstallation_CreateDeleteWorkspace()
        {
            CancellationToken token = default;

            var service = _serviceProvider.GetRequiredService<IWorkspaceService>();

            var responseWrapper = await service.CreateWorkspaceAsync(new WorkspaceInfo(
                "necd"), null, token);

            Assert.NotNull(responseWrapper);
            Assert.Equal(201, responseWrapper.StatusCode);

            var response = responseWrapper.Response;
            Assert.NotNull(response);

            responseWrapper = await service.CreateWorkspaceAsync(new WorkspaceInfo(
                "necd"), null, token);

            Assert.NotNull(responseWrapper);
            Assert.Equal(409, responseWrapper.StatusCode);
            Assert.Null(responseWrapper.Response);

            var responseWrapperD = await service.DeleteWorkspaceAsync("necd", true, token);

            Assert.NotNull(responseWrapperD);
            Assert.Equal(200, responseWrapperD.StatusCode);

            var responseD = responseWrapperD.Response;
            Assert.True(responseD);

            responseWrapperD = await service.DeleteWorkspaceAsync("necd", true, token);

            Assert.NotNull(responseWrapperD);
            Assert.Equal(404, responseWrapperD.StatusCode);

            responseD = responseWrapperD.Response;
            Assert.False(responseD);

            responseWrapperD = await service.DeleteWorkspaceAsync("ne", false, token);

            Assert.NotNull(responseWrapperD);
            Assert.Equal(403, responseWrapperD.StatusCode);

            responseD = responseWrapperD.Response;
            Assert.False(responseD);
        }

        [Fact]
        public async Task GivenDefaultInstallation_CreateUpdateDeleteWorkspace()
        {
            CancellationToken token = default;

            var service = _serviceProvider.GetRequiredService<IWorkspaceService>();

            var responseWrapper = await service.CreateWorkspaceAsync(new WorkspaceInfo(
                "necud"), null, token);

            Assert.NotNull(responseWrapper);
            Assert.Equal(201, responseWrapper.StatusCode);

            var response = responseWrapper.Response;
            Assert.NotNull(response);

            var responseWrapperU = await service.UpdateWorkspaceAsync("necud", new WorkspaceInfo("necud1", false), token);

            Assert.NotNull(responseWrapperU);
            Assert.Equal(200, responseWrapperU.StatusCode);

            var responseU = responseWrapperU.Response;
            Assert.True(responseU);

            responseWrapperU = await service.UpdateWorkspaceAsync("necud", new WorkspaceInfo("necud1", false), token);

            Assert.NotNull(responseWrapperU);
            Assert.Equal(404, responseWrapperU.StatusCode);

            responseU = responseWrapperU.Response;
            Assert.False(responseU);

            var responseWrapperD = await service.DeleteWorkspaceAsync("necud1", true, token);

            Assert.NotNull(responseWrapperD);
            Assert.Equal(200, responseWrapperD.StatusCode);

            var responseD = responseWrapperD.Response;
            Assert.True(responseD);
        }

        public WorkspaceServiceTests(ITestOutputHelper output)
        {
            _output = output;

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();

            _serviceProvider = new ServiceCollection()
                .AddGeoServer(options =>
                {
                    options.BaseAddress = new Uri("http://localhost:8080/geoserver/rest/");
                    options.AuthorizationHeaderValue = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes("admin:geoserver"));
                })
                .AddLogging((builder) => builder.AddXUnit(_output, options =>
                {
                    options.IncludeScopes = true;
                }))
                .BuildServiceProvider();
        }
    }
}