using Pds.Core.Common.Identity.Models;
using Pds.Core.Web.Components.Areas.Lists.Models;
using Pds.Core.Web.Models;
using Pds.DocumentExchange.Services.Models.Filters;
using Pds.DocumentExchange.Web.Extensions;
using Pds.DocumentExchange.Web.Interfaces.Helpers;

namespace Pds.DocumentExchange.Web.Implementations.Helpers
{
    /// <summary>
    /// Class to implement IMapper for testing of mapping extension methods.
    /// </summary>
    public class Mapper : IMapper
    {
        /// <inheritdoc/>
        public Services.Models.UserInfo ToUserInfo(User source)
        {
            return UserMappings.ToUserInfo(source);
        }

        /// <inheritdoc/>
        public CurrentUserViewModel ToCurrentUserViewModel(User source)
        {
            return UserMappings.ToCurrentUserViewModel(source);
        }

        /// <inheritdoc/>
        public Web.Models.Shared.Product ToWebProduct(Services.Models.Product source)
        {
            return ProductMappings.ToWebProduct(source);
        }

        /// <inheritdoc/>
        public IFilterCategoryViewModel ToFilterCategoryView(IFilter source)
        {
            return IFilterMappings.ToFilterCategoryView(source);
        }
    }
}