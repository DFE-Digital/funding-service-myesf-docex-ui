namespace Pds.DocumentExchange.Web.Models.Shared
{
    /// <summary>
    /// A page containing a list of documents.
    /// </summary>
    public interface IDocumentListPage
    {
        /// <summary>
        /// Gets or sets a value indicating whether there are any documents available.
        /// </summary>
        public bool AnyDocumentsAvailable { get; set; }

        /// <summary>
        /// Gets the name of the partial view to render when there are no documents to view.
        /// </summary>
        public string NoDocumentsPartialName { get; }
    }
}