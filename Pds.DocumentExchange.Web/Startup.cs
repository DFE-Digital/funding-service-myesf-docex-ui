using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Pds.Core.DfESignIn;
using Pds.Core.Identity.Claims.Registration;
using Pds.Core.Logging;
using Pds.Core.SecurityAssurances.Middlewares;
using Pds.Core.SecurityAssurances.Middlewares.Options;
using Pds.Core.Telemetry.ApplicationInsights;
using Pds.Core.Utils;
using Pds.Core.Web.Areas.ErrorPages.Builders;
using Pds.Core.Web.Areas.ErrorPages.Registration;
using Pds.Core.Web.Components.Areas.Lists.Models;
using Pds.Core.Web.Components.Areas.Lists.Registration;
using Pds.Core.Web.Models;
using Pds.DocumentExchange.Services.DependencyInjection;
using Pds.DocumentExchange.Services.Models.Filters;
using Pds.DocumentExchange.Web.Areas.Admin.Implementations;
using Pds.DocumentExchange.Web.Areas.Admin.Interfaces;
using Pds.DocumentExchange.Web.Areas.ErrorPages.Builders;
using Pds.DocumentExchange.Web.Authentication.Implementations;
using Pds.DocumentExchange.Web.Authentication.Interfaces;
using Pds.DocumentExchange.Web.Extensions;
using Pds.DocumentExchange.Web.Helpers;
using Pds.DocumentExchange.Web.Implementations.Converters;
using Pds.DocumentExchange.Web.Implementations.Coordinators;
using Pds.DocumentExchange.Web.Implementations.Helpers;
using Pds.DocumentExchange.Web.Implementations.Providers;
using Pds.DocumentExchange.Web.Implementations.Renderers;
using Pds.DocumentExchange.Web.Interfaces.Converters;
using Pds.DocumentExchange.Web.Interfaces.Coordinators;
using Pds.DocumentExchange.Web.Interfaces.Helpers;
using Pds.DocumentExchange.Web.Interfaces.Providers;
using Pds.DocumentExchange.Web.Interfaces.Renderer;
using Pds.DocumentExchange.Web.Models;
using Pds.DocumentExchange.Web.MvcConfiguration;
using System.Diagnostics.CodeAnalysis;

namespace Pds.DocumentExchange.Web
{
    /// <summary>
    /// The startup class.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        /// <summary>
        /// Gets the application configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Configures the services for the container.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                    ForwardedHeaders.XForwardedProto |
                    ForwardedHeaders.XForwardedHost;
            });

            AddAuthentication(services);
            AddCoordinators(services);
            AddConverters(services);
            AddMapper(services);
            AddHelpers(services);
            AddRenderers(services);

            services
                .AddClaimsBasedIdentityService(options => Configuration.Bind("DocumentExchangeServices:AdminApiClient", options))
                .AddScoped<IUserInformationProvider, UserInformationProvider>()
                .AddAuthorization(options => options.AddDocumentExchangePolicies())
                .AddPdsApplicationInsightsTelemetry(options => BuildAppInsightsConfiguration(options))
                .AddLoggerAdapter()
                .AddWebComponents()
                .AddDocumentExchangeServices(options => Configuration.Bind("DocumentExchangeServices", options))
                .AddPdsUtils()
                .Configure<DocumentExchangeConfiguration>(options => Configuration.Bind("DocumentExchangeWeb", options))
                .AddWebControllers()
                .AddPdsErrorPages(false)
                .AddSingleton<IErrorViewModelBuilder, ErrorViewModelBuilder>()
                .AddSingleton<IDateTimeDisplayHelper, DateTimeDisplayHelper>()
                .AddSingleton<IValidationService, ValidationService>()
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddSession(opt =>
                {
                    opt.Cookie.IsEssential = true;
                    opt.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                })
                .AddHealthChecks();
        }

        /// <summary>
        /// Configures the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="env">The web hosting environment.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSession();
            app.UsePathBase(ServiceConstants.PathBase);

            app.UseMiddleware<SecurityHeadersMiddleware>(Options.Create(
                new SecurityHeadersOptions
                {
                    ContentSecurityPolicyDirectives = "default-src 'self'; script-src 'self' 'unsafe-inline' www.googletagmanager.com www.google-analytics.com *.cloudflare.com *.fontawesome.com https://*.clarity.ms https://c.bing.com; script-src-elem 'self' 'unsafe-inline' www.googletagmanager.com www.google-analytics.com *.cloudflare.com *.fontawesome.com https://*.clarity.ms https://c.bing.com; style-src 'self' 'unsafe-inline' *.cloudflare.com *.fontawesome.com; font-src 'self' *.cloudflare.com *.fontawesome.com data:; connect-src 'self' www.google-analytics.com *.fontawesome.com https://*.clarity.ms https://c.bing.com; img-src 'self' www.google-analytics.com https://*.clarity.ms https://c.bing.com"
                }));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UsePdsErrorPages();
                app.UseHsts();
            }

            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHealthChecks("/ping");
            });
        }

        private void AddCoordinators(IServiceCollection services)
        {
            services
                .AddScoped<IAgencySummaryRequestCoordinator, AgencySummaryRequestCoordinator>()
                .AddScoped<IAgencyFileShareListRequestCoordinator, AgencyFileShareListRequestCoordinator>()
                .AddScoped<IAgencyExchangeListRequestCoordinator, AgencyExchangeListRequestCoordinator>()
                .AddScoped<IAgencyExchangeDeletionRequestCoordinator, AgencyExchangeDeletionRequestCoordinator>();
        }

        private void AddConverters(IServiceCollection services)
        {
            services
                .AddSingleton<IDocumentModelConverter, DocumentModelConverter>();
        }

        private void AddMapper(IServiceCollection services)
        {
            services
                .AddSingleton<IMapper, Mapper>();
        }

        private void AddHelpers(IServiceCollection services)
        {
            services
                .AddSingleton<IListHelper, ListHelper>();
        }

        private void AddRenderers(IServiceCollection services)
        {
            services
                .AddSingleton<IDocumentVersionRenderer, DocumentVersionRenderer>();
        }

        private void BuildAppInsightsConfiguration(PdsApplicationInsightsConfiguration options)
        {
            Configuration.Bind("PdsApplicationInsights", options);
            options.Component = this.GetType().Assembly.GetName().Name;
        }

        private void AddAuthentication(IServiceCollection services)
        {
            services.AddDfESignInAuthentication(options => Configuration.Bind("DfESignIn", options));
            services.AddSingleton<ISignOutMethod, DfESignInSignOutMethod>();
        }
    }
}