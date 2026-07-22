namespace Pds.DocumentExchange.Web.Models
{
    /// <summary>
    /// Class to store configuration for the Document Exchange website.
    /// </summary>
    public class DocumentExchangeConfiguration
    {
        /// <summary>
        /// Gets or sets a value indicating whether or not to show the service start page.
        /// </summary>
        /// <remarks>Set to true if Document Exchange is a standalone service with its own start page.</remarks>
        public bool ShowServiceStartPage { get; set; } = false;

        /// <summary>
        /// Gets or sets the MSClarityId.
        /// </summary>
        public string MSClarityId { get; set; } = "MSClarityTestId";

        /// <summary>
        /// Gets or sets the number of items to show on a single list page.
        /// </summary>
        public int ListPageSize { get; set; } = 25;

        /// <summary>
        /// Gets or sets the default survey link on the beta banner.
        /// </summary>
        public string FeedbackLinkUrl { get; set; } = "https://dferesearch.fra1.qualtrics.com/jfe/form/SV_1TumnFi6WPxJpnn";
    }
}