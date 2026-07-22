namespace Pds.DocumentExchange.Web.Models.Organisation
{
    /// <summary>
    /// Model representing information about a child organisation.
    /// </summary>
    public class ChildOrganisationInfo
    {
        /// <summary>
        /// Gets or sets the organisation UKPRN.
        /// </summary>
        public string Ukprn { get; set; }

        /// <summary>
        /// Gets or sets the organisation name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the string representation of this organisation for display and search purposes.
        /// </summary>
        /// <returns>The string representation of this organisation for display and search purposes.</returns>
        public override string ToString()
        {
            return $"{Name} ({Ukprn})";
        }
    }
}