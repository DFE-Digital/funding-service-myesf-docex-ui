namespace Pds.DocumentExchange.Web.Models.Organisation
{
    /// <summary>
    /// Shared view model for pages within the 'send your document' journey.
    /// </summary>
    public class SendYourDocument : BaseOrganisationPageViewModel
    {
        /// <summary>
        /// Gets or sets a value indicating whether to show the parent organisation view.
        /// </summary>
        public bool ShowParentView { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the step was required
        /// to select an organisation to upload a document for.
        /// </summary>
        public bool SelectAnOrganisationRequired { get; set; }

        /// <summary>
        /// Gets or sets the UKPRN of the organisation that the user has chosen to upload a document for.
        /// If not specified, the user is uploading a document for their own organisation.
        /// </summary>
        public string UploadingForUkprn { get; set; }
    }
}