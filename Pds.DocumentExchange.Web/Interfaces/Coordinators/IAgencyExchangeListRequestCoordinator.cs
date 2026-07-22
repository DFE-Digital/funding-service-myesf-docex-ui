using Pds.Core.Web.Components.Areas.Lists.DTOs;
using Pds.DocumentExchange.Services.Models;
using Pds.DocumentExchange.Web.DTOs;
using Pds.DocumentExchange.Web.Models.Agency;
using System.Threading.Tasks;

namespace Pds.DocumentExchange.Web.Interfaces.Coordinators
{
    /// <summary>
    /// Coordinates requests for lists of exchanged documents for agency users.
    /// </summary>
    public interface IAgencyExchangeListRequestCoordinator
    {
        /// <summary>
        /// Gets the documents available to download.
        /// </summary>
        /// <param name="request">An object containing the request parameters.</param>
        /// <returns>The documents available to download.</returns>
        Task<DownloadDocuments> GetDocumentsToDownload(ListRequest request);

        /// <summary>
        /// Gets the list update data for the documents available to manage.
        /// </summary>
        /// <param name="request">An object containing the request parameters.</param>
        /// <returns>The list update data for the documents available to manage.</returns>
        Task<DocumentListUpdateData> GetDocumentsToDownloadData(ListRequest request);

        /// <summary>
        /// Downloads the documents matching the given request parameters.
        /// </summary>
        /// <param name="request">An object containing the request parameters.</param>
        /// <returns>A zip compression of the matching documents.</returns>
        Task<DownloadedFile> DownloadDocuments(ListRequest request);
    }
}