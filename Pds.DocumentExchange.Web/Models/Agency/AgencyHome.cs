using System.Collections.Generic;

namespace Pds.DocumentExchange.Web.Models.Agency
{
    /// <summary>
    /// View model for the agency landing page (agency home).
    /// </summary>
    public class AgencyHome : BaseAgencyPageViewModel
    {
        /// <summary>
        /// Gets or sets the collection of tiles to display.
        /// </summary>
        public IEnumerable<DashboardTile> Tiles { get; set; }
    }
}