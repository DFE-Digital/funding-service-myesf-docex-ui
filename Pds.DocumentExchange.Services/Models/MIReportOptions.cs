using System;

namespace Pds.DocumentExchange.Services.Models
{
    /// <summary>
    /// Class containing options for generating the MI report.
    /// </summary>
    public class MIReportOptions
    {
        /// <summary>
        /// Gets or sets the start date of the report.
        /// </summary>
        public DateTime FromDate { get; set; }

        /// <summary>
        /// Gets or sets the end date of the report.
        /// </summary>
        public DateTime ToDate { get; set; }
    }
}