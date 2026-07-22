using Pds.DocumentExchange.Services.Enums;
using Pds.DocumentExchange.Services.Interfaces;
using Pds.DocumentExchange.Services.Models;
using System.Collections.Generic;
using System.Linq;

namespace Pds.DocumentExchange.Services.Implementations
{
    /// <inheritdoc/>
    public class DocumentStatusProvider : IDocumentStatusProvider
    {
        /// <inheritdoc/>
        public DocumentStatusInfo GetDownloadStatus(IEnumerable<ExchangeDocumentEvent> exchangeDocumentEvents)
        {
            var statusInfo = new DocumentStatusInfo
            {
                Status = DownloadDocumentStatus.New
            };

            var downloadedEvent =
                exchangeDocumentEvents?
                    .Where(x => x.EventType == ExchangeDocumentEventType.DownloadedByReceiver)
                    .OrderByDescending(e => e.EventDateTime)
                    .FirstOrDefault();

            if (downloadedEvent != null)
            {
                statusInfo.Status = DownloadDocumentStatus.Downloaded;
                statusInfo.DownloadedBy = downloadedEvent.UserInfo?.FullName ?? downloadedEvent.UserInfo?.Principal;
                statusInfo.DownloadedTime = downloadedEvent.EventDateTime;
            }

            return statusInfo;
        }
    }
}
