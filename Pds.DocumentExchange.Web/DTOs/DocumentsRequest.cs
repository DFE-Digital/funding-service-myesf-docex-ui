using Pds.Core.Web.Components.Areas.Lists.DTOs;

namespace Pds.DocumentExchange.Web.DTOs
{
    /// <summary>
    /// Class containing the request parameters for documents pages.
    /// </summary>
    public class DocumentsRequest : ListRequest
    {
        /// <summary>
        /// Gets or sets a value indicating whether the page has a validation error.
        /// </summary>
        public bool Error { get; set; }

        /// <summary>
        /// Gets or sets the action that caused the validation error.
        /// </summary>
        public string ErrorAction { get; set; }
    }
}