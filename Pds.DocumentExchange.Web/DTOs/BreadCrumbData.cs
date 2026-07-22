using Pds.DocumentExchange.Web.Models;

namespace Pds.DocumentExchange.Web.DTOs
{
    /// <summary>
    /// Bread crumb parameters.
    /// </summary>
    public class BreadCrumbData
    {
        /// <summary>
        /// Gets or sets the page view model.
        /// </summary>
        public ISetBaseViewModelProperties PageViewModel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the service is using its own service start page.
        /// </summary>
        public bool UsingServiceStartPage { get; set; }
    }
}