using Pds.DocumentExchange.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pds.DocumentExchange.Services.Interfaces
{
    /// <summary>
    /// Interface providing methods to download exchange documents.
    /// </summary>
    public interface IExchangeDocumentDownloadService
    {
        /// <summary>
        /// Downloads an exchange document.
        /// </summary>
        /// <param name="documentReferenceString">The document reference information.</param>
        /// <param name="userInfo">The user info.</param>
        /// <returns>A <see cref="DownloadedFile"/> containing the requested file.</returns>
        Task<DownloadedFile> DownloadExchangeDocumentByReference(string documentReferenceString, UserInfo userInfo);

        /// <summary>
        /// Downloads a list of exchange documents.
        /// </summary>
        /// <param name="documentReferenceStringList">The list of document references to download.</param>
        /// <param name="userInfo">The user info.</param>
        /// <returns>A <see cref="DownloadedFile"/> containing the requested file.</returns>
        Task<DownloadedFile> DownloadExchangeDocumentsByReferences(IEnumerable<string> documentReferenceStringList, UserInfo userInfo);

        /// <summary>
        /// Downloads the list of exchange documents identified by the given <see cref="ExchangeListDocumentOptions"/>.
        /// </summary>
        /// <param name="listOptions">The options for filters and pagination.</param>
        /// <param name="userInfo">The user info.</param>
        /// <param name="teams">The CSV list of teams.</param>
        /// <returns>A <see cref="DownloadedFile"/> containing the requested file.</returns>
        Task<DownloadedFile> DownloadAgencyExchangeDocumentsByListOptions(ExchangeListDocumentOptions listOptions, UserInfo userInfo, string teams);
    }
}