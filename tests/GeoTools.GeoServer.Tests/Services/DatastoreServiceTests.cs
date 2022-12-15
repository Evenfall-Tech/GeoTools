using GeoTools.GeoServer.Extensions;
using GeoTools.GeoServer.Models.Datastore;
using GeoTools.GeoServer.Models.Datastore.Sources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text;

namespace GeoTools.GeoServer.Tests.Services
{
    public class DatastoreServiceTests
    {
        private readonly ITestOutputHelper _output;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        [Fact]
        public async Task GivenDefaultInstallation_GetDatastores()
        {
            CancellationToken token = default;

            var service = _serviceProvider.GetRequiredService<IDatastoreService>();

            var responseWrapper = await service.GetDatastoresAsync("topp", token);

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
        public async Task GivenDefaultInstallation_CreateDeleteDatastore()
        {
            CancellationToken token = default;
            const string CreateDatastoreName = "states1";

            var workspaceService = _serviceProvider.GetRequiredService<IWorkspaceService>();
            var datastoreService = _serviceProvider.GetRequiredService<IDatastoreService>();

            var workspaceWrapper = await workspaceService.GetWorkspaceAsync("topp", token);

            Assert.NotNull(workspaceWrapper);
            Assert.Equal(200, workspaceWrapper.StatusCode);

            var createResponseWrapper = await datastoreService.CreateDatastoreAsync(
                workspaceWrapper.Response.Name,
                new DataStoreInfo(
                    CreateDatastoreName, "MyDesc", true,
                    new ShapefileConnectionParameters(new Uri("file:///data/shapefiles/states.shp"), new Uri("http://www.openplans.org/topp")),
                    false),
                token);

            Assert.NotNull(createResponseWrapper);
            Assert.Equal(201, createResponseWrapper.StatusCode);

            var createResponse = createResponseWrapper.Response;
            Assert.NotNull(createResponse);

            var getResponseWrapper = await datastoreService.GetDatastoreAsync(workspaceWrapper.Response.Name, CreateDatastoreName, token);

            Assert.NotNull(getResponseWrapper);
            Assert.Equal(200, getResponseWrapper.StatusCode);

            var getResponse = getResponseWrapper.Response;
            Assert.NotNull(getResponse);
            Assert.Equal(CreateDatastoreName, getResponse.Name);
            Assert.Equal("MyDesc", getResponse.Description);
            // TODO: Should other fields be checked?
            Assert.True(getResponse.Enabled);

            var deleteResponseWrapper = await datastoreService.DeleteDatastoreAsync(workspaceWrapper.Response.Name, CreateDatastoreName, false, token);

            Assert.NotNull(deleteResponseWrapper);
            Assert.Equal(200, deleteResponseWrapper.StatusCode);

            var deleteResponse = deleteResponseWrapper.Response;
            Assert.True(deleteResponse);
        }

        [Fact]
        public async Task GivenDefaultInstallation_CreateUpdateDeleteDatastore()
        {
            CancellationToken token = default;
            const string CreateDatastoreName = "states1";

            var workspaceService = _serviceProvider.GetRequiredService<IWorkspaceService>();
            var datastoreService = _serviceProvider.GetRequiredService<IDatastoreService>();

            var workspaceWrapper = await workspaceService.GetWorkspaceAsync("topp", token);

            Assert.NotNull(workspaceWrapper);
            Assert.Equal(200, workspaceWrapper.StatusCode);

            var datastoreInfoCreate = new DataStoreInfo(
                    CreateDatastoreName, "MyDesc", true,
                    new ShapefileConnectionParameters(new Uri("file:///data/shapefiles/states.shp"), new Uri("http://www.openplans.org/topp")),
                    false);

            var createResponseWrapper = await datastoreService.CreateDatastoreAsync(
                workspaceWrapper.Response.Name,
                datastoreInfoCreate,
                token);

            Assert.NotNull(createResponseWrapper);
            Assert.Equal(201, createResponseWrapper.StatusCode);

            var createResponse = createResponseWrapper.Response;
            Assert.NotNull(createResponse);

            var datastoreInfoUpdate = new DataStoreInfo(
                    CreateDatastoreName, "MyDesc1", true,
                    new ShapefileConnectionParameters(new Uri("file:///data/shapefiles/states.shp"), new Uri("http://www.openplans.org/topp")),
                    false);

            var updateResponseWrapper = await datastoreService.UpdateDatastoreAsync(
                workspaceWrapper.Response.Name,
                datastoreInfoUpdate,
                token);

            Assert.NotNull(updateResponseWrapper);
            Assert.Equal(200, updateResponseWrapper.StatusCode);

            var updateResponse = updateResponseWrapper.Response;
            Assert.True(updateResponse);

            var getResponseWrapper = await datastoreService.GetDatastoreAsync(workspaceWrapper.Response.Name, CreateDatastoreName, token);

            Assert.NotNull(getResponseWrapper);
            Assert.Equal(200, getResponseWrapper.StatusCode);

            var getResponse = getResponseWrapper.Response;
            Assert.NotNull(getResponse);
            Assert.Equal(CreateDatastoreName, getResponse.Name);
            Assert.Equal("MyDesc1", getResponse.Description);
            // TODO: Should other fields be checked?
            Assert.True(getResponse.Enabled);

            var deleteResponseWrapper = await datastoreService.DeleteDatastoreAsync(workspaceWrapper.Response.Name, CreateDatastoreName, false, token);

            Assert.NotNull(deleteResponseWrapper);
            Assert.Equal(200, deleteResponseWrapper.StatusCode);

            var deleteResponse = deleteResponseWrapper.Response;
            Assert.True(deleteResponse);
        }

        public DatastoreServiceTests(ITestOutputHelper output)
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
