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

            var response = await service.GetWorkspaceAsync("ne", token);

            Assert.NotNull(response);
            Assert.False(string.IsNullOrWhiteSpace(response.Name));
            Assert.False(string.IsNullOrWhiteSpace(response.CoverageStores));
            Assert.False(string.IsNullOrWhiteSpace(response.DataStores));
            Assert.False(string.IsNullOrWhiteSpace(response.WmsStores));

            Assert.Null(await service.GetWorkspaceAsync("nen", token));
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