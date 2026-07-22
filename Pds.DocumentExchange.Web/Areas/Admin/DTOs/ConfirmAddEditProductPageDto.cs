using Pds.Core.Web.Components.Areas.Lists.DTOs;

namespace Pds.DocumentExchange.Web.Areas.Admin.DTOs
{
    /// <summary>
    /// Add edit product DTO.
    /// </summary>
    public class ConfirmAddEditProductPageDto : BaseListItem
    {
        /// <summary>Gets or sets the identifier.</summary>
        public int Identifier { get; set; }

        /// <summary>Gets or sets the product name.</summary>
        public string Name { get; set; }

        /// <summary>Gets or sets the plural form of product name.</summary>
        public string PluralName { get; set; }

        /// <summary>Gets or sets the agency teams.</summary>
        public string AgencyTeam { get; set; }

        /// <summary>Gets or sets a value indicating whether organisations can upload.</summary>
        public string CanOrganisationsUpload { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the page is add or edit.
        /// </summary>
        public bool IsAddPage { get; set; }

        /// <summary>
        /// Gets or sets the old identifier value.
        /// </summary>
        public int OldIdentifier { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether page has errors.
        /// </summary>
        public bool Error { get; set; }
    }
}