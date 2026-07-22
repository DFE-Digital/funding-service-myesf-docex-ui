using Pds.Core.Utils;
using Pds.DocumentExchange.Services.Interfaces;
using Pds.DocumentExchange.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pds.DocumentExchange.Services.Implementations
{
    /// <summary>
    /// Service providing methods to download exchange documents.
    /// </summary>
    public class ExchangeDocumentDownloadService : IExchangeDocumentDownloadService
    {
        private readonly IDocumentReferenceService _documentReferenceService;
        private readonly IExchangeApiClient _exchangeApiClient;
        private readonly IMimeMappingService _mimeMappingService;
        private readonly ISystemProvider _systemProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExchangeDocumentDownloadService"/> class.
        /// </summary>
        /// <param name="documentReferenceService">The document reference service.</param>
        /// <param name="exchangeApiClient">The exchange API client.</param>
        /// <param name="mimeMappingService">The MIME mapping service.</param>
        /// <param name="systemProvider">The system provider.</param>
        public ExchangeDocumentDownloadService(
            IDocumentReferenceService documentReferenceService,
            IExchangeApiClient exchangeApiClient,
            IMimeMappingService mimeMappingService,
            ISystemProvider systemProvider)
        {
            _documentReferenceService = documentReferenceService;
            _exchangeApiClient = exchangeApiClient;
            _mimeMappingService = mimeMappingService;
            _systemProvider = systemProvider;
        }

        /// <inheritdoc/>
        public async Task<DownloadedFile> DownloadExchangeDocumentByReference(
            string documentReferenceString,
            UserInfo userInfo)
        {
            if (string.IsNullOrWhiteSpace(documentReferenceString))
            {
                throw new ArgumentNullException(
                    nameof(documentReferenceString),
                    "The document reference cannot be empty.");
            }

            if (userInfo == null)
            {
                throw new ArgumentNullException(nameof(userInfo), "The user info cannot be null.");
            }

            var documentReference =
                _documentReferenceService.CreateDocumentReferenceFromString(documentReferenceString);

            var downloadRequest = new ExchangeDocumentDownloadRequest
            {
                ListOptions = new ExchangeListDocumentOptions
                {
                    DocumentReferences = new[] { documentReference }
                },
                UserInfo = userInfo
            };

            var documentContent = await _exchangeApiClient.DownloadDocuments(downloadRequest);
            var contentType = _mimeMappingService.GetContentType(documentReference.FileName);

            return new DownloadedFile
            {
                Name = documentReference.FileName,
                Content = documentContent,
                ContentType = contentType
            };
        }

        /// <inheritdoc/>
        public async Task<DownloadedFile> DownloadExchangeDocumentsByReferences(
            IEnumerable<string> documentReferenceStringList,
            UserInfo userInfo)
        {
            var documentReferenceStrings = documentReferenceStringList?.ToList();

            if (documentReferenceStrings?.Any() != true)
            {
                throw new ArgumentException(
                    "The document reference list cannot be empty.",
                    nameof(documentReferenceStringList));
            }

            if (documentReferenceStrings.Count == 1)
            {
                return await DownloadExchangeDocumentByReference(documentReferenceStrings.Single(), userInfo);
            }

            var documentReferences =
                documentReferenceStrings.Select(_documentReferenceService.CreateDocumentReferenceFromString);

            var downloadRequest = new ExchangeDocumentDownloadRequest
            {
                ListOptions = new ExchangeListDocumentOptions
                {
                    DocumentReferences = documentReferences
                },
                UserInfo = userInfo
            };

            var documentContent = await _exchangeApiClient.DownloadDocuments(downloadRequest);

            return GetZipFileResult(documentContent);
        }

        /// <inheritdoc />
        public async Task<DownloadedFile> DownloadAgencyExchangeDocumentsByListOptions(
            ExchangeListDocumentOptions listOptions,
            UserInfo userInfo,
            string teams)
        {
            if (listOptions == null)
            {
                throw new ArgumentNullException(nameof(listOptions), "The list options parameter cannot be null.");
            }

            if (userInfo == null)
            {
                throw new ArgumentNullException(nameof(userInfo), "The user info parameter cannot be null.");
            }

            if (string.IsNullOrEmpty(teams))
            {
                throw new ArgumentNullException(nameof(teams), "The teams parameter cannot be null or empty.");
            }

            var downloadRequest = new ExchangeDocumentDownloadRequest
            {
                ListOptions = listOptions,
                UserInfo = userInfo
            };

            var documentContent = await _exchangeApiClient.DownloadAgencyTeamDocuments(teams, downloadRequest);

            return GetZipFileResult(documentContent);
        }

        private DownloadedFile GetZipFileResult(byte[] documentContent)
        {
            var zipFileName = $"DocumentDownload{_systemProvider.DateTime.Now()}.zip";
            var contentType = _mimeMappingService.GetContentType(zipFileName);

            return new DownloadedFile
            {
                Name = zipFileName,
                Content = documentContent,
                ContentType = contentType
            };
        }
    }
}