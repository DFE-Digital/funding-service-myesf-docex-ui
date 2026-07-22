using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Pds.Core.Web.Administration.Models;
using Pds.DocumentExchange.Services.Interfaces;
using Pds.DocumentExchange.Web.Areas.Admin.DTOs;
using Pds.DocumentExchange.Web.Areas.Admin.Interfaces;
using Pds.DocumentExchange.Web.Areas.Admin.Models;
using Pds.DocumentExchange.Web.Controllers;
using Pds.DocumentExchange.Web.Interfaces.Providers;
using Pds.DocumentExchange.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Pds.DocumentExchange.Web.Helpers.AuthorizationHelper;
using Product = Pds.DocumentExchange.Web.Areas.Admin.Models.Product;

namespace Pds.DocumentExchange.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// The controller containing actions that can be accessed by agency users.
    /// </summary>
    [Route("/[Controller]/[Action]")]
    [Authorize(Policy = Policies.RequireDocumentExchangeAdminRole)]
    [Area("Admin")]
    public class SettingsController : BaseDocumentExchangeController
    {
        private const int MinIdentifier = 10000;
        private const int MaxIdentifier = 99999;
        private const int MaxTextInputLength = 40;

        private readonly ISettingsApiClient _settingsApiClient;
        private readonly IValidationService _validationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsController"/> class.
        /// </summary
        /// <param name="userInformationProvider"><see cref="IUserInformationProvider"/>.</param>
        /// <param name="configurationOptions">The configuration options.</param>
        /// <param name="settingsApiClient">Settings Api Client.</param>
        /// <param name="validationService">Validation Service.</param>
        public SettingsController(
            IUserInformationProvider userInformationProvider,
            IOptions<DocumentExchangeConfiguration> configurationOptions,
            ISettingsApiClient settingsApiClient,
            IValidationService validationService)
            : base(userInformationProvider, configurationOptions)
        {
            _settingsApiClient = settingsApiClient;
            _validationService = validationService;
        }

        /// <summary>
        /// The index action.
        /// </summary>
        /// <returns>The index page view.</returns>
        [HttpGet("/[Controller]")]
        public IActionResult Index()
        {
            var viewModel = new IndexViewModel { };

            return View(viewModel);
        }

        /// <summary>
        /// The products action.
        /// </summary>
        /// <returns>The products page view.</returns>
        [HttpGet]
        public async Task<IActionResult> Products()
        {
            var productsList = await _settingsApiClient.GetAllProducts();
            var teams = await _settingsApiClient.GetTeams();

            var viewModel = new Products
            {
                ListItems = productsList
                    .Select(p => new Product
                    {
                        Identifier = p.Identifier,
                        Name = p.Name,
                        AgencyTeam = teams.First(t => p.AgencyTeams.Contains(t.Identifier)).Name,
                        CanOrganisationsUpload = p.CanOrganisationsUpload,
                        PluralName = p.PluralName,
                        ShouldBeDisplayed = true,
                        LastUpdated = p.LastUpdated
                    })
                    .OrderByDescending(p => p.Identifier)
            };

            return View(viewModel);
        }

        /// <summary>
        /// The add product action.
        /// </summary>
        /// <param name="pageData">The product dto.</param>
        /// <returns>The add product page view.</returns>
        [HttpGet]
        public async Task<IActionResult> AddProduct(ConfirmAddEditProductPageDto pageData)
        {
            var teamsList = await _settingsApiClient.GetTeams();
            var setting = new SettingViewModel();
            var addEditProduct = new AddEditProduct(setting);
            var addEditViewModel = new AddEditPageViewModel(addEditProduct, teamsList, isAddPage: true);

            if (pageData.Error)
            {
                addEditViewModel.Identifier = pageData.Identifier == 0 ? null : pageData.Identifier.ToString();
                addEditViewModel.Name = pageData.Name;
                addEditViewModel.PluralName = pageData.PluralName;
                addEditViewModel.AgencyTeam = pageData.AgencyTeam;
                addEditViewModel.CanOrganisationsUpload = pageData.CanOrganisationsUpload;

                await ValidateProductFieldsAndBuildViewModel(addEditViewModel, pageData);
            }

            return View("AddEditProduct", addEditViewModel);
        }

        /// <summary>
        /// The edit product action.
        /// </summary>
        /// <param name="pageData">The product dto.</param>
        /// <returns>The edit product page view.</returns>
        [HttpGet]
        public async Task<IActionResult> EditProduct(ConfirmAddEditProductPageDto pageData)
        {
            var teamsList = await _settingsApiClient.GetTeams();
            var setting = new SettingViewModel();
            var addEditProduct = new AddEditProduct(setting);
            var identifier = pageData.Identifier;

            var addEditViewModel = new AddEditPageViewModel(addEditProduct, teamsList, isAddPage: false, identifier.ToString());

            var product = await _settingsApiClient.GetProduct(identifier);
            addEditViewModel.OldIdentifier = product.Identifier == -1 ? pageData.OldIdentifier.ToString() : product.Identifier.ToString();

            if (pageData.Error)
            {
                addEditViewModel.Identifier = pageData.Identifier == 0 ? null : pageData.Identifier.ToString();
                addEditViewModel.Name = pageData.Name;
                addEditViewModel.PluralName = pageData.PluralName;
                addEditViewModel.AgencyTeam = pageData.AgencyTeam;
                addEditViewModel.CanOrganisationsUpload = pageData.CanOrganisationsUpload;

                await ValidateProductFieldsAndBuildViewModel(addEditViewModel, pageData);
            }
            else
            {
                addEditViewModel.Identifier = product.Identifier.ToString();
                addEditViewModel.Name = product.Name;
                addEditViewModel.PluralName = product.PluralName;
                addEditViewModel.AgencyTeam = product.AgencyTeams.FirstOrDefault();
                addEditViewModel.CanOrganisationsUpload = product.CanOrganisationsUpload.ToString();
            }

            return View("AddEditProduct", addEditViewModel);
        }

        /// <summary>
        /// The confirm page.
        /// </summary>
        /// <param name="pageData">The product dto.</param>
        /// <returns>The confirm product page view.</returns>
        [HttpPost]
        public async Task<IActionResult> ConfirmAddEditProduct(ConfirmAddEditProductPageDto pageData)
        {
            var productIdentifierExists = (pageData.IsAddPage || pageData.Identifier != pageData.OldIdentifier) ? await ProductIdentifierExists(pageData.Identifier) : false;

            if (!ValidateProductFields(pageData.Identifier, pageData.Name, pageData.PluralName) || productIdentifierExists)
            {
                var returnDto = new ConfirmAddEditProductPageDto
                {
                    Identifier = pageData.Identifier,
                    Name = pageData.Name,
                    PluralName = pageData.PluralName,
                    AgencyTeam = pageData.AgencyTeam,
                    CanOrganisationsUpload = pageData.CanOrganisationsUpload,
                    OldIdentifier = pageData.OldIdentifier,
                    IsAddPage = pageData.IsAddPage,
                    Error = true
                };

                return pageData.IsAddPage ? RedirectToAction(nameof(AddProduct), "Settings", returnDto) :
                    RedirectToAction(nameof(EditProduct), "Settings", returnDto);
            }

            var teams = await _settingsApiClient.GetTeams();

            var confirmEditViewModel = new ConfirmAddEditPageViewModel()
            {
                Product = new Product
                {
                    Identifier = pageData.Identifier,
                    Name = pageData.Name,
                    PluralName = pageData.PluralName,
                    AgencyTeam = pageData.AgencyTeam,
                    CanOrganisationsUpload = bool.Parse(pageData.CanOrganisationsUpload)
                },
                AgencyTeamFriendlyName = teams.First(t => pageData.AgencyTeam.Contains(t.Identifier)).Name,
                OldIdentifier = pageData.OldIdentifier == 0 ? pageData.Identifier : pageData.OldIdentifier,
                IsAddPage = pageData.IsAddPage
            };
            return View("ConfirmAddEditProduct", confirmEditViewModel);
        }

        /// <summary>
        /// The confirm page.
        /// </summary>
        /// <param name="pageData">The product dto.</param>
        /// <returns>The confirm product page view.</returns>
        [HttpPost]
        public async Task<IActionResult> AddOrUpdateProduct(ConfirmAddEditProductPageDto pageData)
        {
            var serviceProduct = new Services.Models.Product
            {
                Identifier = pageData.Identifier,
                Name = pageData.Name,
                PluralName = pageData.PluralName,
                AgencyTeams = new List<string> { pageData.AgencyTeam },
                CanOrganisationsUpload = bool.Parse(pageData.CanOrganisationsUpload)
            };

            var persistedProduct = await _settingsApiClient.AddOrUpdateProduct(pageData.OldIdentifier, serviceProduct);

            return RedirectToAction(nameof(Products), "Settings", new { });
        }


        #region Private Helpers

        private bool ValidateProductFields(int identifier, string name, string pluralName)
        {
            return _validationService.IsInRange(identifier, MinIdentifier, MaxIdentifier) &&
                (_validationService.NotEmptyAndInRange(name, MaxTextInputLength) && _validationService.HasNoSpecialCharacters(name)) &&
                (_validationService.NotEmptyAndInRange(pluralName, MaxTextInputLength) && _validationService.HasNoSpecialCharacters(pluralName));
        }

        private async Task<bool> ProductIdentifierExists(int identifier)
        {
            // If product does not exist it should return uknown product with Identifier equal to -1.
            var product = await _settingsApiClient.GetProduct(identifier);
            return product.Identifier != -1;
        }

        private async Task ValidateProductFieldsAndBuildViewModel(AddEditPageViewModel addEditViewModel, ConfirmAddEditProductPageDto pageData)
        {
            var identifierExists = (pageData.IsAddPage || pageData.Identifier != pageData.OldIdentifier) ? await ProductIdentifierExists(pageData.Identifier) : false;
            var identifierIsInRange = _validationService.IsInRange(pageData.Identifier, MinIdentifier, MaxIdentifier);
            var identifierErroMessage = identifierExists ? "Product Identifier Number already in use<br>" : string.Empty;
            identifierErroMessage += !identifierIsInRange ? "Product Identifier Must be a 5 digit number in range" : string.Empty;
            var identifierError = identifierExists || !identifierIsInRange;
            addEditViewModel.IdentifierError = identifierError;
            addEditViewModel.IdentifierErrorMessage = identifierErroMessage;

            var nameHasNoSpecialCharacters = _validationService.HasNoSpecialCharacters(pageData.Name);
            var nameIsNotEmptyAndInRange = _validationService.NotEmptyAndInRange(pageData.Name, MaxTextInputLength);
            var nameErrorMessage = !nameHasNoSpecialCharacters ? "Product name cannot contain special characters<br>" : string.Empty;
            nameErrorMessage += !nameIsNotEmptyAndInRange ? $"Product name should not be empty or should not exceed {MaxTextInputLength} characters" : string.Empty;
            var nameError = !(nameIsNotEmptyAndInRange && nameHasNoSpecialCharacters);
            addEditViewModel.NameError = nameError;
            addEditViewModel.NameErrorMessage = nameErrorMessage;

            var pluralNameHasNoSpecialCharacters = _validationService.HasNoSpecialCharacters(pageData.PluralName);
            var pluralNameIsNotEmptyAndInRange = _validationService.NotEmptyAndInRange(pageData.PluralName, MaxTextInputLength);
            var pluralNameErrorMessage = !pluralNameHasNoSpecialCharacters ? "Product plural name cannot contain special characters<br>" : string.Empty;
            pluralNameErrorMessage += !pluralNameIsNotEmptyAndInRange ? $"Product plural name should not be empty or should not exceed {MaxTextInputLength} characters" : string.Empty;
            var pluralNameError = !(pluralNameIsNotEmptyAndInRange && pluralNameHasNoSpecialCharacters);
            addEditViewModel.PluralNameError = pluralNameError;
            addEditViewModel.PluralNameErrorMessage = pluralNameErrorMessage;

            addEditViewModel.Error = identifierError || nameError || pluralNameError;
        }

        #endregion
    }
}