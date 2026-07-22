using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Pds.DocumentExchange.Web.Interfaces.Providers;
using Pds.DocumentExchange.Web.Models;

namespace Pds.DocumentExchange.Web.Controllers
{
    /// <summary>
    /// Base class for an MVC controller in Document Exchange.
    /// </summary>
    [Route("/[Action]")]
    public abstract class BaseDocumentExchangeController : Controller
    {
        private readonly IUserInformationProvider _userInformationProvider;
        private readonly DocumentExchangeConfiguration _configuration;

        /// <summary>
        /// Gets the user information provider.
        /// </summary>
        protected IUserInformationProvider UserInformationProvider
            => _userInformationProvider;

        /// <summary>
        /// Gets the configuration for Document Exchange.
        /// </summary>
        protected DocumentExchangeConfiguration Configuration
            => _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseDocumentExchangeController"/> class.
        /// </summary>
        /// <param name="userInformationProvider"><see cref="IUserInformationProvider"/>.</param>
        /// <param name="configurationOptions">The configuration options.</param>
        protected BaseDocumentExchangeController(
            IUserInformationProvider userInformationProvider,
            IOptions<DocumentExchangeConfiguration> configurationOptions)
        {
            _userInformationProvider = userInformationProvider;
            _configuration = configurationOptions.Value;
        }
    }
}