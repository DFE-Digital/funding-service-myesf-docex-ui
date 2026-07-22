using Pds.Core.Web.Components.Areas.Lists.DTOs;
using Pds.DocumentExchange.Web.Models.Agency;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pds.DocumentExchange.Web.Interfaces.Coordinators
{
    /// <summary>
    /// Coordinates requests for deletion of documents by agency users.
    /// </summary>
    public interface IAgencyExchangeDeletionRequestCoordinator
    {
        /// <summary>
        /// Gets information about the documents referenced by the given list that have been selected for deletion.
        /// </summary>
        /// <param name="documentReferences">The list of document references.</param>
        /// <param name="filters">The list of filters.</param>
        /// <returns>The collection of <see cref="AgencyExchangeDocument"/>s that were selected.</returns>
        Task<IEnumerable<AgencyExchangeDocument>> GetDocumentsToDelete(IEnumerable<string> documentReferences, IEnumerable<IFilterCategory> filters);

        /// <summary>
        /// Gets the documents to be deleted.
        /// </summary>
        /// <param name="exchangeDocumentDirection">The exchange document direction.</param>
        /// <param name="ukprn">The UKPRN.</param>
        /// <param name="fileType">The file type.</param>
        /// <param name="year">The academic year.</param>
        /// <returns>The collection of <see cref="AgencyExchangeDocument"/>s that were retrieved.</returns>
        Task<IEnumerable<AgencyExchangeDocument>> GetDocumentsToDelete(Services.Enums.ExchangeDocumentDirection exchangeDocumentDirection, int ukprn, string fileType, string year);

        /// <summary>
        /// Deletes the documents referenced by the given list that have been confirmed for deletion.
        /// </summary>
        /// <param name="documentReferences">The list of document references.</param>
        /// <param name="publishers">A list of strings showing the document publishers.</param>
        /// <returns>The collection of <see cref="AgencyExchangeDocument"/>s that were deleted.</returns>
        Task<IEnumerable<AgencyExchangeDocument>> DeleteDocuments(IEnumerable<string> documentReferences, IEnumerable<string> publishers = null);

        /// <summary>
        /// Deletes the document referenced by the given reference that has been confirmed for deletion.
        /// </summary>
        /// <param name="documentReference">The document reference.</param>
        /// <returns>The <see cref="AgencyExchangeDocument"/> that was deleted.</returns>
        Task<AgencyExchangeDocument> DeleteDocumentVersion(string documentReference);
    }
}