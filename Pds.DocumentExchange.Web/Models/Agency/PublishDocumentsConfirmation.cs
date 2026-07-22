namespace Pds.DocumentExchange.Web.Models.Agency
{
    /// <summary>
    /// View model for the confirmation page within the publish documents journey.
    /// </summary>
    public class PublishDocumentsConfirmation : PublishDocuments
    {
        /// <summary>
        /// Gets or sets a value indicating whether the user confirmed that the documents should be published.
        /// </summary>
        public bool? PublishConfirmed { get; set; }

        /// <inheritdoc/>
        // The title will be explicitly rendered in the view so we will not show the one in the global layout.
        public override bool ShowContentTitle => false;

        /// <inheritdoc/>
        public override bool IsTwoThirdsLayout => false;

        /// <inheritdoc/>
        protected override string Title
            => $"You've published {Count} {(Count == 1 ? Product?.Name : Product?.PluralName)}";
    }
}