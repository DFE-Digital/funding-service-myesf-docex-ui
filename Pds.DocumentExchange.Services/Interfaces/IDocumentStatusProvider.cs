using Pds.DocumentExchange.Services.Models;
using System.Collections.Generic;

namespace Pds.DocumentExchange.Services.Interfaces
{
    /// <summary>
    /// Interface exposing methods to get a Document's status.
    /// </summary>
    public interface IDocumentStatusProvider
    {
        /// <summary>
        /// Gets the document's download status.
        /// </summary>
        /// <param name="exchangeDocumentEvents">The document history.</param>
        /// <returns>The document status info.</returns>
        DocumentStatusInfo GetDownloadStatus(IEnumerable<ExchangeDocumentEvent> exchangeDocumentEvents);
    }
}
