using Microsoft.AspNetCore.Mvc.Rendering;
using Pds.DocumentExchange.Services.Enums;
using System.Collections.Generic;
using System.Linq;

namespace Pds.DocumentExchange.Web.Models.SupportTools
{
    /// <summary>
    /// The delete documents view model.
    /// </summary>
    public class DeleteDocumentsViewModel : BaseDocumentExchangePageViewModel
    {
        /// <summary>
        /// Gets or sets the exchange document direction.
        /// </summary>
        public ExchangeDocumentDirection ExchangeDocumentDirection { get; set; } = ExchangeDocumentDirection.PublishedByAgency;

        /// <summary>
        /// Gets or sets the UKPRN.
        /// </summary>
        public string Ukprn { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the document type.
        /// </summary>
        public string DocumentType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the period.
        /// </summary>
        public string Period { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the collection or error messages.
        /// </summary>
        public IEnumerable<string> ErrorMessages { get; set; } = Enumerable.Empty<string>();

        /// <summary>
        /// Gets or sets the document types.
        /// </summary>
        public IEnumerable<SelectListItem> DocumentTypes { get; set; }

        /// <inheritdoc/>
        protected override string Title
            => "Delete documents";
    }
}