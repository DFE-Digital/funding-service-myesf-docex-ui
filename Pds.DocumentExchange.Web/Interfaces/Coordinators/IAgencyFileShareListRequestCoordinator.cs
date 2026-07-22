using Pds.Core.Web.Components.Areas.Lists.DTOs;
using Pds.DocumentExchange.Web.DTOs;
using Pds.DocumentExchange.Web.Models.Agency;
using System.Threading.Tasks;

namespace Pds.DocumentExchange.Web.Interfaces.Coordinators
{
    /// <summary>
    /// Coordinates requests for lists of agency file share documents.
    /// </summary>
    public interface IAgencyFileShareListRequestCoordinator
    {
        /// <summary>
        /// Gets the invalid file share documents to review.
        /// </summary>
        /// <param name="request">An object containing the request parameters.</param>
        /// <returns>The documents to review.</returns>
        Task<DocumentsToReview> GetDocumentsToReview(ListRequest request);

        /// <summary>
        /// Gets the list update data for the invalid file share documents to review.
        /// </summary>
        /// <param name="request">An object containing the request parameters.</param>
        /// <returns>The list update data for the documents to review.</returns>
        Task<DocumentListUpdateData> GetDocumentsToReviewData(ListRequest request);

        /// <summary>
        /// Gets the valid file share documents to publish.
        /// </summary>
        /// <param name="request">An object containing the request parameters.</param>
        /// <returns>The documents to publish.</returns>
        Task<DocumentsToPublish> GetDocumentsToPublish(ListRequest request);

        /// <summary>
        /// Gets the list update data for the invalid file share documents to publish.
        /// </summary>
        /// <param name="request">An object containing the request parameters.</param>
        /// <returns>The list update data for the documents to publish.</returns>
        Task<DocumentListUpdateData> GetDocumentsToPublishData(ListRequest request);
    }
}