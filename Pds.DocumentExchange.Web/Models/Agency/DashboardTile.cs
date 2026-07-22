namespace Pds.DocumentExchange.Web.Models.Agency
{
    /// <summary>
    /// Structure for holding the data necessary for rendering a dashboard tile.
    /// </summary>
    public class DashboardTile
    {
        /// <summary>
        /// Gets or sets the ID of the link.
        /// </summary>
        public string LinkId { get; set; }

        /// <summary>
        /// Gets or sets the URL of the link.
        /// </summary>
        public string LinkUrl { get; set; }

        /// <summary>
        /// Gets or sets the ID of the title.
        /// </summary>
        public string TitleId { get; set; }

        /// <summary>
        /// Gets or sets the tile title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the CSS class of the alert.
        /// </summary>
        public string AlertClass { get; set; }

        /// <summary>
        /// Gets or sets the text of the alert.
        /// </summary>
        public string AlertText { get; set; }
    }
}