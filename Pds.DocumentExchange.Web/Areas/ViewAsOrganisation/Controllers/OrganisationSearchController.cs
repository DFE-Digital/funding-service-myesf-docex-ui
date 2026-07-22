using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Pds.Core.Common.Organisation.Enums;
using Pds.Core.Common.Organisation.Models;
using Pds.DocumentExchange.Services.Interfaces;
using Pds.DocumentExchange.Web.Areas.ViewAsOrganisation.Models;
using Pds.DocumentExchange.Web.Controllers;
using Pds.DocumentExchange.Web.Interfaces.Providers;
using Pds.DocumentExchange.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Pds.DocumentExchange.Web.Helpers.AuthorizationHelper;

namespace Pds.DocumentExchange.Web.Areas.ViewAsOrganisation.Controllers
{
    /// <summary>
    /// Controller providing actions for searching for an organisation to view as.
    /// </summary>
    [Authorize(Policy = Policies.RequireViewAsOrganisationRole)]
    [Area(Constants.AreaName)]
    [Route("/[Area]/[Action]")]
    public class OrganisationSearchController : BaseDocumentExchangeController
    {
        private readonly IOrganisationApiClient _organisationApiClient;

        private readonly Regex _ukprnRegex = new Regex("^[0-9]{8}$", RegexOptions.Compiled);

        /// <summary>
        /// Initializes a new instance of the <see cref="OrganisationSearchController"/> class.
        /// </summary>
        /// <param name="userInformationProvider"><see cref="IUserInformationProvider"/>.</param>
        /// <param name="configurationOptions">The configuration options.</param>
        /// <param name="organisationApiClient"><see cref="IOrganisationApiClient"/>.</param>
        public OrganisationSearchController(
            IUserInformationProvider userInformationProvider,
            IOptions<DocumentExchangeConfiguration> configurationOptions,
            IOrganisationApiClient organisationApiClient)
            : base(userInformationProvider, configurationOptions)
        {
            _organisationApiClient = organisationApiClient;
        }

        /// <summary>
        /// Action for the organisation search form.
        /// </summary>
        /// <param name="error">A value indicating whether there is a validation error.</param>
        /// <returns>The organisation search form view.</returns>
        public async Task<IActionResult> SearchForAnOrganisation(bool error = false)
            => await Task.FromResult(View(new SearchForAnOrganisation { Error = error }));

        /// <summary>
        /// Action for the organisation search results.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>The organisation search results view.</returns>
        public async Task<IActionResult> OrganisationSearchResults(OrganisationSearchResults model)
        {
            var searchTerm = model.SearchTerm;
            IReadOnlyCollection<Organisation> organisations;

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return RedirectToAction(nameof(SearchForAnOrganisation), new { error = true });
            }

            if (_ukprnRegex.IsMatch(searchTerm))
            {
                var organisation = await _organisationApiClient.GetOrganisation(
                    new OrganisationIdentifier
                    {
                        Type = OrganisationIdentifierType.Ukprn,
                        Value = searchTerm
                    });

                organisations = new[] { organisation };
            }
            else
            {
                organisations = await _organisationApiClient.Search(searchTerm, Constants.MaxSearchResults);
            }

            model.HasMoreResults = organisations.Count >= Constants.MaxSearchResults;

            model.Organisations = organisations
                .Where(org => org?.Identifiers?.Any(IsValidUkprnIdentifier) == true)
                .Select(org => (org.Identifiers.First(IsValidUkprnIdentifier).Value, org.Name))
                .ToList();

            return View(model);
        }

        private static Func<OrganisationIdentifier, bool> IsValidUkprnIdentifier =>
            id => id.Type == OrganisationIdentifierType.Ukprn && !string.IsNullOrEmpty(id.Value);
    }
}