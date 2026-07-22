using Newtonsoft.Json;
using Pds.DocumentExchange.Services.Models.Filters.JsonConverters;

namespace Pds.DocumentExchange.Services.Models.Filters
{
    /// <summary>
    /// Interface representing the data for a filter that can be applied against some items.
    /// </summary>
    [JsonConverter(typeof(FilterJsonConverter))]
    public interface IFilter
    {
        /// <summary>
        /// Gets or sets the filter title, for display usage.
        /// E.g. "Document Type".
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Gets or sets the filter key, which identifies the item property that the filter represents.
        /// E.g. "ProductIdentifier".
        /// </summary>
        string Key { get; set; }

        /// <summary>
        /// Gets or sets the filter type.
        /// </summary>
        string Type { get; set; }
    }
}