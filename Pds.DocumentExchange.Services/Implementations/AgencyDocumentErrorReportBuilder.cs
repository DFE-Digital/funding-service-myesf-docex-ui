using Pds.Core.Documents.Aspose.Interfaces;
using Pds.Core.Documents.Aspose.Models;
using Pds.Core.Logging;
using Pds.DocumentExchange.Services.Interfaces;
using Pds.DocumentExchange.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pds.DocumentExchange.Services.Implementations
{
    /// <summary>
    /// A service that can be used to build the error report for invalid agency documents.
    /// </summary>
    public class AgencyDocumentErrorReportBuilder : IAgencyDocumentErrorReportBuilder
    {
        private readonly IAgencyApiClient _agencyClient;
        private readonly ISpreadsheetBuilder _spreadsheetBuilder;
        private readonly ILoggerAdapter<AgencyDocumentErrorReportBuilder> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AgencyDocumentErrorReportBuilder"/> class.
        /// </summary>
        /// <param name="agencyClient">The agency api client.</param>
        /// <param name="spreadsheetBuilder">The spreadsheet builder.</param>
        /// <param name="logger">The logger.</param>
        public AgencyDocumentErrorReportBuilder(
            IAgencyApiClient agencyClient,
            ISpreadsheetBuilder spreadsheetBuilder,
            ILoggerAdapter<AgencyDocumentErrorReportBuilder> logger)
        {
            _agencyClient = agencyClient;
            _spreadsheetBuilder = spreadsheetBuilder;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<byte[]> BuildErrorReportSpreadsheet(string team, AgencyListDocumentOptions listOptions)
        {
            if (string.IsNullOrEmpty(team))
            {
                throw new ArgumentException("Must specify a team.", nameof(team));
            }

            if (listOptions.Validity != Enums.AgencyDocumentValidity.Invalid)
            {
                throw new ArgumentException("Error report can only be generated for invalid documents.", nameof(listOptions));
            }

            _logger.LogInformation("Getting data for error report spreadsheet for team: " + team);
            var documents = await _agencyClient.ListTeamDocuments(team, listOptions);

            _logger.LogInformation("Composing model for error report spreadsheet for team: " + team);
            var spreadsheet = new Spreadsheet
            {
                Worksheets = new Dictionary<string, Worksheet>
                {
                    {
                        "DocumentErrors",
                        new Worksheet
                        {
                            Cells = GetErrorReportData(documents),
                            Columns = new List<Column>
                            {
                                new Column
                                {
                                    Position = 0,
                                    WidthInches = 4
                                },
                                new Column
                                {
                                    Position = 1,
                                    WidthInches = 4
                                }
                            }
                        }
                    }
                }
            };

            _logger.LogInformation("Building error report spreadsheet for team: " + team);
            var spreadsheetData = _spreadsheetBuilder.BuildSpreadsheetWithData(spreadsheet, true, false);
            _logger.LogInformation("Completed Building error report spreadsheet for team: " + team);

            return spreadsheetData;
        }

        private Dictionary<string, CellData> GetErrorReportData(ListResult<AgencyDocument> documents)
        {
            var agencyDocuments = documents.Items.ToList();
            var cells = new Dictionary<string, CellData>(2 * (1 + agencyDocuments.Count));

            cells.Add("A1", GetCellData("Document name"));
            cells.Add("B1", GetCellData("Document name error"));

            for (int documentPosition = 0; documentPosition < agencyDocuments.Count; documentPosition++)
            {
                var document = agencyDocuments[documentPosition];
                var row = documentPosition + 2;

                cells.Add($"A{row}", GetCellData(document.FileName));
                cells.Add($"B{row}", GetCellData(document.FileNameError));
            }

            return cells;
        }

        private CellData GetCellData(string value)
        {
            return new CellData(value)
            {
                FontSize = 11
            };
        }
    }
}