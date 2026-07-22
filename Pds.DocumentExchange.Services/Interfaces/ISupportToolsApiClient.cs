using Pds.DocumentExchange.Services.Enums;
using Pds.DocumentExchange.Services.Models;
using System;
using System.Threading.Tasks;

namespace Pds.DocumentExchange.Services.Interfaces
{
    /// <summary>
    /// The support tools API client.
    /// </summary>
    public interface ISupportToolsApiClient
    {
        /// <summary>
        /// Gets the documents published by DfE.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>A <see cref="Task"/> returning the documents published by DfE.</returns>
        Task<ListResult<PublishedBatch>> GetDocumentsPublishedByDfE(int pageNumber, int pageSize);

        /// <summary>
        /// Downloads the CSV file containing the documents published by the given parent batch identifier.
        /// </summary>
        /// <param name="parentBatchIdentifier">The parent batch identifier.</param>
        /// <returns>A <see cref="Task"/> returning a CSV file containing the documents published by parent batch identifier.</returns>
        Task<byte[]> DownloadDocumentsPublishedCsv(Guid parentBatchIdentifier);

        /// <summary>
        /// Downloads the CSV file containing the notification recipients by the given parent batch identifier.
        /// </summary>
        /// <param name="parentBatchIdentifier">The parent batch identifier.</param>
        /// <returns>A <see cref="Task"/> returning a CSV file containing the documents published by parent batch identifier.</returns>
        Task<byte[]> DownloadNotificationRecipientsCsv(Guid parentBatchIdentifier);

        /// <summary>
        /// Gets the documents to be deleted.
        /// </summary>
        /// <param name="exchangeDocumentDirection">The exchange document direction.</param>
        /// <param name="ukprn">The UKPRN.</param>
        /// <param name="fileType">The file type.</param>
        /// <param name="year">The academic year.</param>
        /// <returns>A <see cref="Task"/> returning the documents to delete info.</returns>
        Task<ListResult<ExchangeDocument>> DeleteDocumentsSearch(ExchangeDocumentDirection exchangeDocumentDirection, int ukprn, string fileType, string year);

        /// <summary>
        /// Downloads the MI Report.
        /// </summary>
        /// <param name="from">from date.</param>
        /// <param name="to">to date.</param>
        /// <returns>A <see cref="Task"/> Downloads the MI report by given parameters.</returns>
        Task<byte[]> DownloadMIReport(DateTime from, DateTime to);
    }
}