namespace Pds.DocumentExchange.Web.Models.DocumentExchange
{
    /// <summary>
    /// View model for the terms and conditions page.
    /// </summary>
    public class UserGuide : BaseDocumentExchangePageViewModel
    {
        /// <inheritdoc />
        public override bool IsTwoThirdsLayout => true;

        /// <inheritdoc/>
        protected override string Title => "Document exchange external user guide";
    }
}