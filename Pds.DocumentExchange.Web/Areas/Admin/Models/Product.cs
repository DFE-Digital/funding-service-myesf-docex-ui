using Pds.Core.Web.Components.Areas.Lists.DTOs;
using System;

namespace Pds.DocumentExchange.Web.Areas.Admin.Models
{
    /// <summary>
    /// View model representing a product that has been received.
    /// </summary>
    public class Product : BaseListItem
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
        public bool CanOrganisationsUpload { get; set; }

        /// <summary>
        /// Gets or sets the date and time the product was last updated.
        /// </summary>
        /// <remarks>Null if the date and time are unknown.</remarks>
        public DateTime? LastUpdated { get; set; }
    }
}