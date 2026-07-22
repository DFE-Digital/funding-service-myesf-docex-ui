using System;

namespace Pds.DocumentExchange.Services.Models.Filters
{
    /// <summary>
    /// Class representing a date range filter that can be applied against some items.
    /// </summary>
    public class DateRangeFilter : IFilter
    {
        /// <inheritdoc/>
        public string Title { get; set; }

        /// <inheritdoc/>
        public string Key { get; set; }

        /// <inheritdoc/>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the 'from' date filter title.
        /// </summary>
        public string FromTitle { get; set; }

        /// <summary>
        /// Gets or sets the 'to' date filter title.
        /// </summary>
        public string ToTitle { get; set; }

        /// <summary>
        /// Gets or sets the 'from' date filter value.
        /// </summary>
        public DateTime? From { get; set; }

        /// <summary>
        /// Gets or sets the 'to' date filter value.
        /// </summary>
        public DateTime? To { get; set; }
    }
}