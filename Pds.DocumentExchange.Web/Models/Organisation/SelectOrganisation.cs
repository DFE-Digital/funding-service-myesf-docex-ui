namespace Pds.DocumentExchange.Web.Models.Organisation
{
    /// <summary>
    /// View model for the 'select an organisation' page.
    /// </summary>
    public class SelectOrganisation : BaseOrganisationPageViewModel
    {
        /// <inheritdoc/>
        public override bool IsTwoThirdsLayout => true;

        /// <inheritdoc/>
        // Hide the content title in the layout as it is explicitly included in the view.
        public override bool ShowContentTitle => false;

        /// <inheritdoc/>
        protected override string Title
            => "Which organisation do you want to send a document for?";
    }
}