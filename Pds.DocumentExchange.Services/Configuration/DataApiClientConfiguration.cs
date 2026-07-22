using Pds.Core.ApiClient;

namespace Pds.DocumentExchange.Services.Configuration
{
    /// <summary>
    /// Structure to hold configuration for a Document Exchange Data API client.
    /// </summary>
    public class DataApiClientConfiguration : BaseApiClientConfiguration
    {
        /// <summary>
        /// Gets or sets the timeout value of HttpClient.
        /// </summary>
        public int Timeout { get; set; }
    }
}