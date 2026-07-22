using Pds.DocumentExchange.Web.Models;

namespace Pds.DocumentExchange.Web.Areas.Admin.Models
{
    /// <summary>
    /// View model for the settings landing page.
    /// </summary>
    public class IndexViewModel : BaseDocumentExchangePageViewModel
    {
        #region Base view model overrides

        /// <inheritdoc/>
        protected override string Title
            => "Document exchange settings";

        #endregion
    }
}