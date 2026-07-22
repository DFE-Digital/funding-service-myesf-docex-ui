using Microsoft.AspNetCore.Http;
using Pds.Core.Web.Areas.ErrorPages.Models;
using System;

namespace Pds.DocumentExchange.Web.Areas.ErrorPages.Models
{
    /// <summary>
    /// View model for the 503 service unavailable error page.
    /// </summary>
    public sealed class ServiceUnavailableErrorViewModel : ErrorViewModel, IServiceUnavailableErrorViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceUnavailableErrorViewModel"/> class.
        /// </summary>
        public ServiceUnavailableErrorViewModel()
            : base(StatusCodes.Status503ServiceUnavailable)
        {
        }

        /// <inheritdoc/>
        public DateTime? ServiceAvailableFrom => null;
    }
}