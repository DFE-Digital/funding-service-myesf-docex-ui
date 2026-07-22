using Pds.DocumentExchange.Services.Models;
using Pds.DocumentExchange.Web.Models.Agency;

namespace Pds.DocumentExchange.Web.Interfaces.Converters
{
    /// <summary>
    /// Converts document models.
    /// </summary>
    public interface IDocumentModelConverter
    {
        /// <summary>
        /// Converts an exchange document to an agency exchange document.
        /// </summary>
        /// <param name="exchangeDocument">The source document.</param>
        /// <returns>The converted document.</returns>
        AgencyExchangeDocument CreateAgencyExchangeDocumentFromExchangeDocument(ExchangeDocument exchangeDocument);
    }
}