using Microsoft.AspNetCore.Http;
using Pds.DocumentExchange.Services.Models;
using Pds.DocumentExchange.Web.Interfaces.Renderer;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pds.DocumentExchange.Web.Implementations.Renderers
{
    /// <summary>
    /// Renderer responsible for rendering a comma delimited string of hyperlinked document versions.
    /// </summary>
    public class DocumentVersionRenderer : IDocumentVersionRenderer
    {
        private readonly string _host;
        private readonly string _scheme;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentVersionRenderer"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">The current http context accessor.</param>
        public DocumentVersionRenderer(
            IHttpContextAccessor httpContextAccessor)
        {
            _scheme = httpContextAccessor.HttpContext.Request.Scheme;
            _host = httpContextAccessor.HttpContext.Request.Host.ToString();
        }

        /// <inheritdoc/>
        public string GetHyperLinkedDocumentVersionLabels(ExchangeDocument exchangeDocument)
        {
            var documentVersionLabelsList = new List<string>
            {
                CreateHyperLinkVersionLabel(exchangeDocument)
            };

            documentVersionLabelsList
                .AddRange((exchangeDocument.PreviousVersions?.OrderByDescending(v => v.Version))
                .Select(item => CreateHyperLinkVersionLabel(item)));

            return string.Join(", ", documentVersionLabelsList);
        }

        /// <inheritdoc/>
        public string GetHyperLinkedDocumentVersionLabelWithPrefix(ExchangeDocument exchangeDocument, string versionLabelPrefix)
        {
            return CreateHyperLinkVersionLabel(exchangeDocument, versionLabelPrefix);
        }

        /// <inheritdoc/>
        public string GetDocumentVersionsWithoutHyperlinks(ExchangeDocument exchangeDocument)
        {
            var documentVersionLabelsList = new List<string>
            {
                exchangeDocument.Version.ToString()
            };

            documentVersionLabelsList
                .AddRange((exchangeDocument.PreviousVersions?.OrderByDescending(v => v.Version))
                .Select(item => CreateVersionLabelWithoutHyperlinks(item)));

            return string.Join(", ", documentVersionLabelsList);
        }

        private string CreateHyperLinkVersionLabel(ExchangeDocument exchangeDocument, string versionLabelPrefix = "")
        {
            var url = $"{_scheme}://{_host}{ServiceConstants.PathBase}/download-exchange-document?documentReferenceString={CreateDocumentReferenceQueryStringValue(exchangeDocument.DocumentReference)}";
            return $"<a class=document-version-label href={url}> {versionLabelPrefix + exchangeDocument.Version}</a>";
        }

        private string CreateVersionLabelWithoutHyperlinks(ExchangeDocument exchangeDocument)
        {
            return exchangeDocument.Version.ToString();
        }

        private string CreateDocumentReferenceQueryStringValue(DocumentReference documentReference)
        {
            return HttpUtility.UrlEncode($"{documentReference.FileName}|{documentReference.BatchIdentifier}|{documentReference.ParentBatchIdentifier}");
        }
    }
}