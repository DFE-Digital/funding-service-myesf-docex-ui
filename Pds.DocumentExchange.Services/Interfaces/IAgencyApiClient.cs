using Pds.DocumentExchange.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pds.DocumentExchange.Services.Interfaces
{
    /// <summary>
    /// An interface exposing methods for interacting with the Document Exchange Agency API.
    /// </summary>
    public interface IAgencyApiClient
    {
        /// <summary>
        /// Gets a summary of the agency files contained within the file share for the given teams.
        /// </summary>
        /// <param name="teams">The csv list of agency teams.</param>
        /// <returns>A task representing the result of the asynchronous operation.</returns>
        Task<FileShareSummary> GetTeamSummary(string teams);

        /// <summary>
        /// Gets the file share documents for the given team, filtered and paginated by the given options.
        /// </summary>
        /// <param name="team">The agency team.</param>
        /// <param name="options">The options for filters and pagination.</param>
        /// <returns>A task representing the result of the asynchronous operation.</returns>
        Task<ListResult<AgencyDocument>> ListTeamDocuments(string team, AgencyListDocumentOptions options);

        /// <summary>
        /// Downloads the specified document for the given team.
        /// </summary>
        /// <param name="team">The agency team.</param>
        /// <param name="fileName">The file name.</param>
        /// <returns>A task representing the result of the asynchronous operation.</returns>
        Task<byte[]> DownloadTeamDocument(string team, string fileName);

        /// <summary>
        /// Deletes the specified documents from the file share of the given team.
        /// </summary>
        /// <param name="team">The agency team.</param>
        /// <param name="fileNames">The list of file names to delete.</param>
        /// <returns>A <see cref="Task"/> representing the result
        /// of the asynchronous operation.</returns>
        Task RemoveTeamDocuments(string team, IEnumerable<string> fileNames);

        /// <summary>
        /// Publishes the specified documents from the file share of the given team.
        /// </summary>
        /// <param name="team">The agency team.</param>
        /// <param name="agencyPublishRequest">Request object containing the product ID to publish
        /// and the user information.</param>
        /// <returns>A task representing the result of the asynchronous operation.</returns>
        Task<KeyValuePair<Product, int>> PublishTeamDocuments(
            string team,
            AgencyPublishRequest agencyPublishRequest);

        /// <summary>
        /// Gets the previous document version references.
        /// </summary>
        /// <param name="team">The team.</param>
        /// <param name="documentReferences">The document references.</param>
        /// <returns>List of previous document references.</returns>
        Task<IEnumerable<DocumentReference>> GetPreviousDocumentVersionReferences(
            string team,
            IEnumerable<DocumentReference> documentReferences);
    }
}