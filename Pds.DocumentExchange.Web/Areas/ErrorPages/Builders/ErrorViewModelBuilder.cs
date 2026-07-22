using Microsoft.AspNetCore.Http;
using Pds.Core.Web.Areas.ErrorPages.Builders;
using Pds.Core.Web.Areas.ErrorPages.Models;
using Pds.DocumentExchange.Web.Areas.ErrorPages.Models;

namespace Pds.DocumentExchange.Web.Areas.ErrorPages.Builders
{
    /// <inheritdoc cref="IErrorViewModelBuilder"/>
    public sealed class ErrorViewModelBuilder : IErrorViewModelBuilder
    {
        /// <inheritdoc/>
        public IErrorViewModel Build(int statusCode)
        {
            if (statusCode == StatusCodes.Status503ServiceUnavailable)
            {
                return new ServiceUnavailableErrorViewModel();
            }

            return new ErrorViewModel(statusCode);
        }
    }
}