using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pds.Core.ApiClient.Interfaces;
using Pds.Core.ApiClient.Services;
using Pds.Core.Documents.Aspose;
using Pds.DocumentExchange.Services.Configuration;
using Pds.DocumentExchange.Services.Implementations;
using Pds.DocumentExchange.Services.Interfaces;
using System;
using System.Net.Http;
using System.Reflection;

namespace Pds.DocumentExchange.Services.DependencyInjection
{
    /// <summary>
    /// Extensions class for <see cref="IServiceCollection"/> for registering the feature's services.
    /// </summary>
    public static class FeatureServiceCollectionExtensions
    {
        /// <summary>
        /// Adds services for the current feature to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the feature's services to.</param>
        /// <param name="configureOptions">An action that will hydrate an instance
        /// of <see cref="DocumentExchangeServicesConfiguration"/>.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddDocumentExchangeServices(
            this IServiceCollection services,
            Action<DocumentExchangeServicesConfiguration> configureOptions)
        {
            AddConfiguration(services, configureOptions);
            AddApiClients(services);
            AddServices(services);

            return services;
        }

        private static void AddConfiguration(
            IServiceCollection services,
            Action<DocumentExchangeServicesConfiguration> configureOptions)
        {
            services.Configure(configureOptions);
            services.Configure<DataApiClientConfiguration>(options =>
            {
                var config = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
                config.Bind("DocumentExchangeServices:DataApiClient", options);
            });
        }

        private static void AddApiClients(IServiceCollection services)
        {
            const string DocExApiHttpClientName = "DocExApiHttpClient";

            services
                .AddHttpClient(DocExApiHttpClientName)
                .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
                {
                    MaxConnectionsPerServer = 64,
                    PooledConnectionLifetime = TimeSpan.FromMinutes(10),
                    PooledConnectionIdleTimeout = TimeSpan.FromMinutes(5)
                })
                .SetHandlerLifetime(TimeSpan.FromMinutes(10))
                .AddTypedClient<IAgencyApiClient, AgencyApiClient>()
                .AddTypedClient<IOrganisationApiClient, OrganisationApiClient>()
                .AddTypedClient<IExchangeApiClient, ExchangeApiClient>()
                .AddTypedClient<IUploadApiClient, UploadApiClient>()
                .AddTypedClient<ISettingsApiClient, SettingsApiClient>()
                .AddTypedClient<ISupportToolsApiClient, SupportToolsApiClient>();

            services.AddSingleton(typeof(IAuthenticationService<>), typeof(AuthenticationService<>));
        }

        private static void AddServices(IServiceCollection services)
        {
            services.AddSingleton<IMimeMappingService, MimeMappingService>();
            services.AddSingleton<IDocumentReferenceService, DocumentReferenceService>();
            services.AddSingleton<IExchangeDocumentDownloadService, ExchangeDocumentDownloadService>();
            services.AddSingleton<IDocumentStatusProvider, DocumentStatusProvider>();

            services.AddAsposeSpreadsheetBuilder(options =>
            {
                var licenseStream = Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("Pds.DocumentExchange.Services.Resources.Aspose.Total.lic");
                options.ApplyLicense = true;
                options.LicenseStream = licenseStream;

                services.PostConfigure<DocumentExchangeServicesConfiguration>(opt =>
                    opt.AsposeSpreadsheetBuilderConfiguration = options);
            });
            services.AddSingleton<IAgencyDocumentErrorReportBuilder, AgencyDocumentErrorReportBuilder>();
        }
    }
}