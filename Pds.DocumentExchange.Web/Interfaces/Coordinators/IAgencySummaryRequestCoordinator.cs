using Pds.DocumentExchange.Web.DTOs;
using Pds.DocumentExchange.Web.Models.Agency;
using System.Threading.Tasks;

namespace Pds.DocumentExchange.Web.Interfaces.Coordinators
{
    /// <summary>
    /// Coordinates requests for agency summary data.
    /// </summary>
    public interface IAgencySummaryRequestCoordinator
    {
        /// <summary>
        /// Gets the home page data.
        /// </summary>
        /// <returns>The home page data.</returns>
        Task<AgencyHomePageData> GetHomePageData();

        /// <summary>
        /// Gets the file share page data.
        /// </summary>
        /// <returns>The file share page data.</returns>
        Task<FileShare> GetFileSharePage();
    }
}