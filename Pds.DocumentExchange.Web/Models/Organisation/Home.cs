namespace Pds.DocumentExchange.Web.Models.Organisation
{
    /// <summary>
    /// View model for the organisation landing page (home).
    /// </summary>
    public class Home : BaseOrganisationPageViewModel
    {
        #region Landing page-specific properties

        /// <summary>
        /// Gets or sets the count of newly received documents.
        /// </summary>
        public int CountOfNewDocuments { get; set; }

        /// <summary>
        /// Gets or sets the name of the MVC action to which the user
        /// should be directed when they want to send a new document.
        /// </summary>
        public string SendNewDocumentActionName { get; set; }

        #endregion


        #region Base view model overrides

        /// <inheritdoc/>
        public override bool IsTwoThirdsLayout => true;

        #endregion
    }
}