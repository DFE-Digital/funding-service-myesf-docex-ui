using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.Core.Web.Models;
using Pds.DocumentExchange.Web.Controllers;
using Pds.DocumentExchange.Web.Filters;
using Pds.DocumentExchange.Web.Interfaces.Providers;
using Pds.DocumentExchange.Web.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Pds.DocumentExchange.Web.Tests.Unit.Filters
{
    [TestClass]
    [TestCategory("Unit")]
    public sealed class UserContextActionFilterAttributeTests
    {
        private readonly IUserInformationProvider _userInfoProvider
            = Mock.Of<IUserInformationProvider>(MockBehavior.Strict);

        [TestMethod]
        public async Task OnActionExecutionAsync_InitialisesTheUser()
        {
            //Arrange
            var config = new DocumentExchangeConfiguration();

            var model = new TestViewModel();

            var user = new ClaimsPrincipal();

            Mock.Get(_userInfoProvider)
                .Setup(uip => uip.Initialise(user))
                .Returns(Task.CompletedTask);

            Mock.Get(_userInfoProvider)
                .Setup(uip => uip.GetCurrentUserViewModel())
                .ReturnsAsync(new CurrentUserViewModel());

            var (context, next) = GetOnActionExecutionParameters(config, model, user);

            var filter = GetTestFilter();

            //Act
            await filter.OnActionExecutionAsync(context, next);

            //Assert
            Mock.Get(_userInfoProvider).VerifyAll();
        }

        [TestMethod]
        public async Task OnActionExecutionAsync_SetsTheCurrentUserViewModel()
        {
            //Arrange
            var config = new DocumentExchangeConfiguration();

            var model = new TestViewModel();
            var currentUserViewModel = new CurrentUserViewModel();

            Mock.Get(_userInfoProvider)
                .Setup(uip => uip.Initialise(It.IsAny<ClaimsPrincipal>()))
                .Returns(Task.CompletedTask);

            Mock.Get(_userInfoProvider)
                .Setup(uip => uip.GetCurrentUserViewModel())
                .ReturnsAsync(currentUserViewModel);

            var (context, next) = GetOnActionExecutionParameters(config, model, new ClaimsPrincipal());

            var filter = GetTestFilter();

            //Act
            await filter.OnActionExecutionAsync(context, next);

            //Assert
            Mock.Get(_userInfoProvider).VerifyAll();
            model.CurrentUser.Should().Be(currentUserViewModel);
        }

        private UserContextActionFilterAttribute GetTestFilter()
            => new UserContextActionFilterAttribute(_userInfoProvider);

        private (ActionExecutingContext context, ActionExecutionDelegate next) GetOnActionExecutionParameters(
            DocumentExchangeConfiguration config,
            TestViewModel model,
            ClaimsPrincipal user)
        {
            var controller = new DocumentExchangeController(
                _userInfoProvider,
                Options.Create(config));

            var httpContext = new DefaultHttpContext
            {
                User = user
            };

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var actionContext = new ActionContext
            {
                HttpContext = httpContext,
                RouteData = new RouteData(),
                ActionDescriptor = new ActionDescriptor(),
            };
            var metadata = new List<IFilterMetadata>();

            var context = new ActionExecutingContext(
                actionContext,
                metadata,
                new Dictionary<string, object>(),
                controller);

            ActionExecutionDelegate next = () =>
            {
                var actionExecutedContext = new ActionExecutedContext(actionContext, metadata, controller)
                {
                    Result = new ViewResult
                    {
                        ViewData = new ViewDataDictionary(controller.ViewData)
                        {
                            Model = model
                        }
                    }
                };

                return Task.FromResult(actionExecutedContext);
            };

            return (context, next);
        }

        private class TestViewModel : BasePageViewModel
        {
        }
    }
}