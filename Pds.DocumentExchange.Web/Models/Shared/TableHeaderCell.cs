namespace Pds.DocumentExchange.Web.Models.Shared
{
    /// <summary>
    /// View model representing a table header cell.
    /// </summary>
    public class TableHeaderCell
    {
        /// <summary>
        /// Gets or sets the cell's value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the CSS classes to apply to the rendered table header cell.
        /// </summary>
        public string CssClasses { get; set; }
    }
}