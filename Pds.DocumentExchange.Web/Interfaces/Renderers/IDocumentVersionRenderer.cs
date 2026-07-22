using Pds.DocumentExchange.Services.Models;

namespace Pds.DocumentExchange.Web.Interfaces.Renderer
{
    /// <summary>
    /// Interface for rendering document version information.
    /// </summary>
    public interface IDocumentVersionRenderer
    {
        /// <summary>
        /// Renders the version labels with hyper links.
        /// </summary>
        /// <param name="exchangeDocument">Exchange document for which versions need to be generated.</param>
        /// <returns>Comma delimited versions with each version hyperlinked so that the version can be individually downloaded.</returns>
        string GetHyperLinkedDocumentVersionLabels(ExchangeDocument exchangeDocument);

        /// <summary>
        /// Renders the version label with hyper link.
        /// </summary>
        /// <param name="exchangeDocument">Exchange document for which versions need to be generated.</param>
        /// <param name="versionLabelPrefix">Prefix to be added to hyperlink text just before the version number.</param>
        /// <returns>Comma delimited versions with each version hyperlinked so that the version can be individually downloaded.</returns>
        string GetHyperLinkedDocumentVersionLabelWithPrefix(ExchangeDocument exchangeDocument, string versionLabelPrefix);

        /// <summary>
        /// Renders the versions without hyper links.
        /// </summary>
        /// <param name="exchangeDocument">Exchange document for which versions need to be generated.</param>
        /// <returns>Comma delimited versions without hyperlinks for display.</returns>
        string GetDocumentVersionsWithoutHyperlinks(ExchangeDocument exchangeDocument);
    }
}