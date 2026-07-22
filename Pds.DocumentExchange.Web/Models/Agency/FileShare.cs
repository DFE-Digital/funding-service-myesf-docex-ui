using Pds.DocumentExchange.Web.Helpers;

namespace Pds.DocumentExchange.Web.Models.Agency
{
    /// <summary>
    /// View model for the file share page.
    /// </summary>
    public class FileShare : BaseAgencyPageViewModel
    {
        #region File share page-specific properties

        /// <summary>
        /// Gets or sets the total count of documents in the file share.
        /// </summary>
        public int TotalCountOfDocuments { get; set; }

        /// <summary>
        /// Gets or sets the count of valid documents that can be published.
        /// </summary>
        public int CountOfValidDocuments { get; set; }

        /// <summary>
        /// Gets or sets the count of invalid documents that need reviewing.
        /// </summary>
        public int CountOfInvalidDocuments { get; set; }

        #endregion


        #region Base view model overrides

        /// <inheritdoc/>
        public override bool IsTwoThirdsLayout => true;

        /// <inheritdoc/>
        protected override string Title
            => ContentHelper.GetDocumentCountMessage(
                TotalCountOfDocuments,
                "There {is/are} {count} {document(s)} in your file share");

        #endregion
    }
}