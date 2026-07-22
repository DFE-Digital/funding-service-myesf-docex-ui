using Pds.DocumentExchange.Services.Models;
using System.Threading.Tasks;

namespace Pds.DocumentExchange.Services.Interfaces
{
    /// <summary>
    /// An interface exposing methods for interacting with the Document Exchange "Upload" API.
    /// </summary>
    public interface IUploadApiClient
    {
        /// <summary>Uploads the document.</summary>
        /// <param name="request">Request object containing information about the document being uploaded.</param>
        /// <returns>Task.</returns>
        Task UploadDocument(UploadDocumentRequest request);
    }
}