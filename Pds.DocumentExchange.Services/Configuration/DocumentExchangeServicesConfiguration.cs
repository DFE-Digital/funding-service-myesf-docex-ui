using Pds.Core.Documents.Aspose.Models;

namespace Pds.DocumentExchange.Services.Configuration
{
    /// <summary>
    /// Structure to hold configuration for the service layer.
    /// </summary>
    public class DocumentExchangeServicesConfiguration
    {
        /// <summary>
        /// Gets or sets the data API client configuration.
        /// </summary>
        public DataApiClientConfiguration DataApiClient { get; set; }

        /// <summary>
        /// Gets or sets the Aspose spreadsheet builder configuration.
        /// </summary>
        public AsposeDocumentManagementConfiguration AsposeSpreadsheetBuilderConfiguration { get; set; }
    }
}