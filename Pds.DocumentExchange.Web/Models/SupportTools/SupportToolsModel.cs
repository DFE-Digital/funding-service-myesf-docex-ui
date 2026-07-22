namespace Pds.DocumentExchange.Web.Models.SupportTools
{
    /// <summary>
    /// View model for support tools landing page.
    /// </summary>
    public class SupportToolsModel : BaseDocumentExchangePageViewModel
    {
        /// <summary>
        /// Gets or sets a value indicating whether the user has advanced or admin roles.
        /// </summary>
        public bool IsUserAdvancedOrAdmin { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user has any document exchange internal roles.
        /// </summary>
        public bool IsAnyDocumentExchangeInternalUser { get; set; }

        #region Base view model overrides

        /// <inheritdoc/>
        protected override string Title
            => "Document exchange support tools";

        #endregion
    }
}
