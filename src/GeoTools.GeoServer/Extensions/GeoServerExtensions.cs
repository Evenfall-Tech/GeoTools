using GeoTools.GeoServer.Helpers.Options;
using GeoTools.GeoServer.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace GeoTools.GeoServer.Extensions
{
    public static class GeoServerExtensions
    {
        public static IServiceCollection AddGeoServer(this IServiceCollection services)
        {
            return services.AddGeoServer(options => { });
        }

        public static IServiceCollection AddGeoServer(this IServiceCollection services, Action<GeoServerOptions> configureOptions)
        {
            return AddGeoServer(services, (options, _) => configureOptions(options));
        }

        public static IServiceCollection AddGeoServer(this IServiceCollection services, Action<GeoServerOptions, IServiceProvider> configureOptions)
        {
            services
                .AddSingleton<IValidateOptions<GeoServerOptions>, ValidateGeoServerOptions>()
                .AddOptions<GeoServerOptions>()
                .Configure(configureOptions);

            services
                .AddHttpClient(GeoServerOptions.HttpClientName, ConfigureHttpClient);

            return services
                .AddTransient<IWorkspaceService, WorkspaceService>()
                .AddTransient<IDatastoreService, DatastoreService>();
        }

        private static void ConfigureHttpClient(IServiceProvider provider, HttpClient client)
        {
            var optionsWrapper = provider.GetService<IOptions<GeoServerOptions>>();

            if (optionsWrapper != null)
            {
                var options = optionsWrapper.Value;

                if (options.ConfigureHttpClient != null)
                {
                    options.ConfigureHttpClient(provider, client);
                }
                else
                {
                    client.BaseAddress = options.BaseAddress;

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.ParseAdd("application/json");

                    if (options.AuthorizationHeaderValue != null)
                    {
                        var authTokens = options.AuthorizationHeaderValue.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        var authSchema = authTokens[0];
                        var authParameter = string.Join(" ", authTokens.Skip(1));

                        client.DefaultRequestHeaders.Authorization = string.IsNullOrEmpty(authParameter)
                            ? new AuthenticationHeaderValue(authSchema)
                            : new AuthenticationHeaderValue(authSchema, authParameter);
                    }
                }
            }
        }
    }
}
