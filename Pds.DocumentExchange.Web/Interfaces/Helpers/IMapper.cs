using Pds.Core.Common.Identity.Models;
using Pds.Core.Web.Components.Areas.Lists.Models;
using Pds.Core.Web.Models;
using Pds.DocumentExchange.Services.Models;
using Pds.DocumentExchange.Services.Models.Filters;

namespace Pds.DocumentExchange.Web.Interfaces.Helpers
{
    /// <summary>
    /// Interface to allow testing of mapping extension methods.
    /// </summary>
    public interface IMapper
    {
        /// <summary>
        /// Interface to allow testing of extension method ToCurrentUserViewModel.
        /// </summary>
        /// <param name="source">User source object.</param>
        /// <returns>CurrentUserViewModel destination object.</returns>
        CurrentUserViewModel ToCurrentUserViewModel(User source);

        /// <summary>
        /// Interface to allow testing of extension method ToUserInfo.
        /// </summary>
        /// <param name="source">User source object.</param>
        /// <returns>UserInfo destination object.</returns>
        UserInfo ToUserInfo(User source);

        /// <summary>
        /// Interface to allow testing of extension method ToWebProduct.
        /// </summary>
        /// <param name="source">Services.Models.Product source object.</param>
        /// <returns>Models.Shared.Product destination object.</returns>
        Models.Shared.Product ToWebProduct(Services.Models.Product source);

        /// <summary>
        /// Interface to allow testing of extension method ToFilterCategoryView.
        /// </summary>
        /// <param name="source">Source object that implements IFilter.</param>
        /// <returns>Destination object that implements IFilterCategoryViewModel.</returns>
        IFilterCategoryViewModel ToFilterCategoryView(IFilter source);
    }
}