using Pds.Core.Common.Organisation.Models;
using Pds.DocumentExchange.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pds.DocumentExchange.Services.Interfaces
{
    /// <summary>
    /// An interface exposing methods for interacting with the Document Exchange "Exchange" API.
    /// </summary>
    public interface IExchangeApiClient
    {
        /// <summary>
        /// Gets the summary for an organisation user.
        /// </summary>
        /// <param name="userInfo">The user information.</param>
        /// <returns>A task representing the result of the asynchronous operation.</returns>
        Task<Summary> GetOrganisationUserSummary(UserInfo userInfo);

        /// <summary>
        /// Gets the summary for a list of agency teams.
        /// </summary>
        /// <param name="teams">The team names.</param>
        /// <returns>A task representing the result of the asynchronous operation.</returns>
        Task<Summary> GetAgencyTeamSummary(string teams);

        /// <summary>
        /// Downloads the exchanged documents for the given team, filtered and paginated
        /// by the given options.
        /// </summary>
        /// <param name="team">The agency team.</param>
        /// <param name="downloadRequest">The download request parameters.</param>
        /// <returns>A task representing the result of the asynchronous operation.</returns>
        Task<byte[]> DownloadAgencyTeamDocuments(string team, ExchangeDocumentDownloadRequest downloadRequest);

        /// <summary>
        /// Gets the exchanged documents for the given team, filtered and paginated
        /// by the given options.
        /// </summary>
        /// <param name="team">The agency team.</param>
        /// <param name="options">The options for filters and pagination.</param>
        /// <returns>A task representing the result of the asynchronous operation.</returns>
        Task<ListResult<ExchangeDocument>> GetAgencyTeamDocuments(string team, ExchangeListDocumentOptions options);

        /// <summary>
        /// Gets the organisation received and sent files.
        /// </summary>
        /// <param name="options">The options for user, filters and pagination.</param>
        /// <returns>A task representing the result of the asynchronous operation.</returns>
        Task<ListResult<ExchangeDocument>> GetOrganisationDocuments(ExchangeListOrganisationDocumentOptions options);

        /// <summary>
        /// Downloads the specified documents. Marks the documents as viewed by the specified user.
        /// </summary>
        /// <param name="downloadRequest">Request object containing
        /// the user information and the documents to download.</param>
        /// <returns>A task representing the result of the asynchronous operation.</returns>
        Task<byte[]> DownloadDocuments(ExchangeDocumentDownloadRequest downloadRequest);

        /// <summary>Gets the current product version for an organisation.</summary>
        /// <param name="organisationIdentifier">The organisation identifier.</param>
        /// <param name="productIdentifier">The product identifier.</param>
        /// <returns>A task representing the result of the asynchronous operation.</returns>
        Task<int> GetCurrentProductVersionForOrganisation(OrganisationIdentifier organisationIdentifier, string productIdentifier);

        /// <summary>
        /// Deletes the specified documents. Marks the documents metadata as deleted.
        /// </summary>
        /// <param name="deleteRequest">Request object containing
        /// the user information and the documents to delete.</param>
        /// <returns>A task representing the result of the asynchronous operation.</returns>
        Task<IEnumerable<ExchangeDocument>> DeleteDocuments(ExchangeDocumentDeleteRequest deleteRequest);

        /// <summary>
        /// Deletes the specified document. Marks the document metadata as deleted.
        /// </summary>
        /// <param name="deleteRequest">Request object containing
        /// the user information and the document to delete.</param>
        /// <returns>A task representing the result of the asynchronous operation.</returns>
        Task<ExchangeDocument> DeleteDocument(ExchangeDocumentDeleteRequest deleteRequest);

        /// <summary>
        /// Gets the MI report.
        /// </summary>
        /// <param name="options">Object containing the parameters of the report.</param>
        /// <returns>A task representing the result of the asynchronous operation.</returns>
        Task<MIReportData> MIReport(MIReportOptions options);
    }
}