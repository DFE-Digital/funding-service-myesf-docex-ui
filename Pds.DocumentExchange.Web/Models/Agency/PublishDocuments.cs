using Pds.DocumentExchange.Web.Models.Shared;

namespace Pds.DocumentExchange.Web.Models.Agency
{
    /// <summary>
    /// Shared view model for pages within the publish documents journey.
    /// </summary>
    public class PublishDocuments : BaseAgencyPageViewModel
    {
        /// <summary>
        /// Gets or sets the product ID selected for publish.
        /// </summary>
        public int SelectedProductId { get; set; }

        /// <summary>
        /// Gets or sets the selected team ID.
        /// </summary>
        public string SelectedTeam { get; set; }

        /// <summary>
        /// Gets or sets the product being published.
        /// </summary>
        public Product Product { get; set; }

        /// <summary>
        /// Gets or sets the count of documents being published.
        /// </summary>
        public int Count { get; set; }
    }
}