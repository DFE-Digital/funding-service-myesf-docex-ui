using Pds.Core.Common.Organisation.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pds.DocumentExchange.Services.Interfaces
{
    /// <summary>
    /// Interface for exposing methods to interact with the Document Exchange API Organisation controller.
    /// </summary>
    public interface IOrganisationApiClient
    {
        /// <summary>
        /// Gets the organisation with the given identifier.
        /// </summary>
        /// <param name="identifier">The organisation identifier to lookup.</param>
        /// <returns>The organisation with the given identifier.</returns>
        Task<Organisation> GetOrganisation(OrganisationIdentifier identifier);

        /// <summary>
        /// Searches for organisations by name or ukprn.
        /// </summary>
        /// <param name="searchTerm">The organisation name or ukprn search term.</param>
        /// <param name="maxResults">The maximum number of matching organisations to return.</param>
        /// <returns>The collection of matching organisations.</returns>
        Task<IReadOnlyCollection<Organisation>> Search(string searchTerm, int maxResults);
    }
}
