using Pds.DocumentExchange.Services.Enums;
using Pds.DocumentExchange.Services.Interfaces;
using Pds.DocumentExchange.Services.Models;
using Pds.DocumentExchange.Web.Interfaces.Converters;
using Pds.DocumentExchange.Web.Interfaces.Helpers;
using Pds.DocumentExchange.Web.Interfaces.Renderer;
using Pds.DocumentExchange.Web.Models.Agency;
using System;
using System.Linq;

namespace Pds.DocumentExchange.Web.Implementations.Converters
{
    /// <inheritdoc cref="IDocumentModelConverter"/>
    public class DocumentModelConverter : IDocumentModelConverter
    {
        private readonly IDateTimeDisplayHelper _dateTimeDisplayHelper;
        private readonly IDocumentStatusProvider _documentStatusProvider;
        private readonly IDocumentVersionRenderer _documentVersionRenderer;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentModelConverter"/> class.
        /// </summary>
        /// <param name="dateTimeDisplayHelper"><see cref="IDateTimeDisplayHelper"/>.</param>
        /// <param name="documentStatusProvider"><see cref="IDocumentStatusProvider"/>.</param>
        /// <param name="documentVersionRenderer"><see cref="IDocumentVersionRenderer"/>.</param>
        public DocumentModelConverter(
            IDateTimeDisplayHelper dateTimeDisplayHelper,
            IDocumentStatusProvider documentStatusProvider,
            IDocumentVersionRenderer documentVersionRenderer)
        {
            _dateTimeDisplayHelper = dateTimeDisplayHelper;
            _documentStatusProvider = documentStatusProvider;
            _documentVersionRenderer = documentVersionRenderer;
        }

        /// <inheritdoc/>
        public AgencyExchangeDocument CreateAgencyExchangeDocumentFromExchangeDocument(ExchangeDocument exchangeDocument)
        {
            var documentDownloadStatus = _documentStatusProvider.GetDownloadStatus(exchangeDocument.EventHistory);
            var docRef = exchangeDocument.DocumentReference;
            var downloadedDateTime = documentDownloadStatus.DownloadedTime;

            var uploadedDateTime = exchangeDocument
                                    .EventHistory
                                    ?.FirstOrDefault(e => e.EventType == ExchangeDocumentEventType.SentByOrganisation)
                                    ?.EventDateTime
                                ?? DateTime.MinValue;

            var publishedEvent = exchangeDocument
                                   .EventHistory
                                   ?.FirstOrDefault(e => e.EventType == ExchangeDocumentEventType.PublishedByAgency);

            var versionsString = _documentVersionRenderer?.GetHyperLinkedDocumentVersionLabels(exchangeDocument);
            var versionString = _documentVersionRenderer?.GetHyperLinkedDocumentVersionLabelWithPrefix(exchangeDocument, "View version ");

            var versionStringWithoutHyperlinks = _documentVersionRenderer?.GetDocumentVersionsWithoutHyperlinks(exchangeDocument);

            return new AgencyExchangeDocument
            {
                FileName = docRef.FileName,
                BatchIdentifier = docRef.BatchIdentifier,
                ParentBatchIdentifier = docRef.ParentBatchIdentifier,
                ProductName = exchangeDocument.Product.Name,
                Status = documentDownloadStatus.Status,
                StatusDateTime = downloadedDateTime,
                DownloadedBy = documentDownloadStatus.DownloadedBy,
                DisplayStatusDateTime = _dateTimeDisplayHelper.ToTimeAndDateDisplayString(downloadedDateTime),
                PreviousVersions = exchangeDocument.PreviousVersions?.Select(CreateAgencyExchangeDocumentFromExchangeDocument),
                ProviderName = exchangeDocument.OrganisationInfo.Name,
                ProviderUkprn = exchangeDocument.OrganisationInfo.OrganisationIdentifier.Value,
                Versions = versionsString ?? string.Empty,
                VersionsWithoutHyperlinks = versionStringWithoutHyperlinks ?? string.Empty,
                Version = versionString ?? string.Empty,
                VersionWithHyperlink = versionString?.Replace("View version ", string.Empty) ?? string.Empty,
                VersionNumber = exchangeDocument.Version,
                UploadedDateTime = uploadedDateTime,
                DisplayUploadedDateTime = _dateTimeDisplayHelper.ToTimeAndDateDisplayString(uploadedDateTime),
                PublishedBy = publishedEvent?.UserInfo?.FullName ?? publishedEvent?.UserInfo?.Principal ?? string.Empty,
                PublishedDateTime = publishedEvent?.EventDateTime ?? DateTime.MinValue,
                DisplayPublishedDateTime = _dateTimeDisplayHelper.ToTimeAndDateDisplayString(publishedEvent?.EventDateTime ?? DateTime.MinValue),
            };
        }
    }
}