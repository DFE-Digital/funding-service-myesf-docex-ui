using Pds.DocumentExchange.Services.Models;
using System.Threading.Tasks;

namespace Pds.DocumentExchange.Services.Interfaces
{
    /// <summary>
    /// An interface for a service that can be used to build the error report for invalid agency documents.
    /// </summary>
    public interface IAgencyDocumentErrorReportBuilder
    {
        /// <summary>
        /// Builds the error report as a spreadsheet.
        /// </summary>
        /// <param name="team">The agency team name.</param>
        /// <param name="listOptions">The options for selecting agency documents to include in the report.</param>
        /// <returns>A byte array representing the spreadsheet.</returns>
        Task<byte[]> BuildErrorReportSpreadsheet(string team, AgencyListDocumentOptions listOptions);
    }
}