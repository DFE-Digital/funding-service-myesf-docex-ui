using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.DependencyInjection;
using Pds.Core.Web.Filters;
using Pds.DocumentExchange.Web.Filters;
using System.Diagnostics.CodeAnalysis;

namespace Pds.DocumentExchange.Web.MvcConfiguration
{
    /// <summary>
    /// Extension methods for setting up Web MVC services in an <see cref="IServiceCollection"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class WebMvcServiceCollectionExtensions
    {
        /// <summary>
        /// Adds services for API controllers to the specified <see cref="IServiceCollection"/>,
        /// using custom conventions for routing.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
        /// <returns>The <see cref="IServiceCollection"/> instance for chaining.</returns>
        public static IServiceCollection AddWebControllers(this IServiceCollection services)
        {
            services.AddControllersWithViews(options =>
            {
                options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
                options.Filters.Add<CommonPagePropertiesActionFilterAttribute>();
                options.Filters.Add<UserContextActionFilterAttribute>();
                options.Filters.Add<CookiePreferencesActionFilterAttribute>();
            });

            return services;
        }
    }
}