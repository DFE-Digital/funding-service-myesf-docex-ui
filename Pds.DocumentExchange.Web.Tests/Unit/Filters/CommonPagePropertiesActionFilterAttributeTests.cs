using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.Core.Web.Models;
using Pds.Core.Web.Models.Hyperlinks;
using Pds.DocumentExchange.Web.Attributes.BreadCrumbs;
using Pds.DocumentExchange.Web.Controllers;
using Pds.DocumentExchange.Web.DTOs;
using Pds.DocumentExchange.Web.Filters;
using Pds.DocumentExchange.Web.Interfaces.Providers;
using Pds.DocumentExchange.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pds.DocumentExchange.Web.Tests.Unit.Filters
{
    [TestClass]
    [TestCategory("Unit")]
    public sealed class CommonPagePropertiesActionFilterAttributeTests
    {
        private readonly IUserInformationProvider _userInfoProvider
            = Mock.Of<IUserInformationProvider>(MockBehavior.Strict);

        private readonly IUrlHelperFactory _urlHelperFactory
            = Mock.Of<IUrlHelperFactory>(MockBehavior.Strict);

        [TestMethod]
        public async Task OnActionExecutionAsync_WhenNoBreadCrumbAttributeSet_ShouldNotSetBreadCrumbs()
        {
            //Arrange
            var config = new DocumentExchangeConfiguration();

            var model = new TestViewModel();

            var (context, next) = GetOnActionExecutionParameters(config, model, breadCrumbAttribute: null);

            var filter = GetTestFilter(config);

            //Act
            await filter.OnActionExecutionAsync(context, next);

            //Assert
            model.TestBreadCrumbs.Should().BeNull();
        }

        [TestMethod]
        public async Task OnActionExecutionAsync_WhenBreadCrumbAttributeSet_ShouldSetBreadCrumbs()
        {
            //Arrange
            var config = new DocumentExchangeConfiguration();

            var model = new TestViewModel();

            var breadCrumb = new TestBreadCrumbAttribute();

            var (context, next) = GetOnActionExecutionParameters(config, model, breadCrumb);

            var filter = GetTestFilter(config);

            //Act
            await filter.OnActionExecutionAsync(context, next);

            //Assert
            model.TestBreadCrumbs.Should().NotBeNull();
        }

        [TestMethod]
        public async Task OnActionExecutionAsync_ShouldSetFeedbackLinks()
        {
            //Arrange
            var config = new DocumentExchangeConfiguration
            {
                FeedbackLinkUrl = "/feedback/link"
            };

            var model = new TestViewModel();

            var (context, next) = GetOnActionExecutionParameters(config, model);

            var filter = GetTestFilter(config);

            //Act
            await filter.OnActionExecutionAsync(context, next);

            //Assert
            model.FeedbackLink.Should().BeEquivalentTo(config.FeedbackLinkUrl);
        }

        [TestMethod]
        public async Task OnActionExecutionAsync_WhenNotShowingServiceStartPage_ShouldNotSetItems()
        {
            //Arrange
            var config = new DocumentExchangeConfiguration
            {
                ShowServiceStartPage = false
            };

            var model = new TestViewModel();

            var (context, next) = GetOnActionExecutionParameters(config, model);

            var filter = GetTestFilter(config);

            //Act
            await filter.OnActionExecutionAsync(context, next);

            //Assert
            model.TestExitViewAsOrganisationLink.Should().BeNull();
            model.TestHeaderTitle.Should().BeNull();
            model.TestHeaderLink.Should().BeNull();
            model.TestLogoutLink.Should().BeNull();
            model.TestTermsAndConditionsLink.Should().BeNull();
            model.TestViewYourSubservicesLink.Should().BeNull();
            model.TestViewYourSubservicesText.Should().BeNull();
        }

        [TestMethod]
        public async Task OnActionExecutionAsync_WhenShowingServiceStartPage_ShouldSetItems()
        {
            //Arrange
            var config = new DocumentExchangeConfiguration
            {
                ShowServiceStartPage = true
            };

            var model = new TestViewModel();

            var (context, next) = GetOnActionExecutionParameters(config, model);

            var urlHelper = Mock.Of<IUrlHelper>(MockBehavior.Strict);

            Mock.Get(_urlHelperFactory)
                .Setup(factory => factory.GetUrlHelper(It.IsAny<ActionContext>()))
                .Returns(urlHelper);

            var expectedExitViewAsOrganisationLink = "/exit/view/as/org";
            var expectedTermsAndConditionsLink = "/terms/and/conditions";
            var expectedViewYourSubservicesLink = "/account/settings";

            var mockUrlHelper = Mock.Get(urlHelper);

            mockUrlHelper
                .SetupGet(url => url.ActionContext)
                .Returns(context);

            mockUrlHelper
                .SetupSequence(url => url.Action(It.IsAny<UrlActionContext>()))
                .Returns(expectedExitViewAsOrganisationLink)
                .Returns(expectedTermsAndConditionsLink)
                .Returns(expectedViewYourSubservicesLink);

            var filter = GetTestFilter(config);

            //Act
            await filter.OnActionExecutionAsync(context, next);

            //Assert
            model.TestHeaderTitle.Should().Be(ServiceConstants.ServiceName);
            model.TestHeaderLink.Should().Be(ServiceConstants.PathBase);
            model.TestExitViewAsOrganisationLink.Should().Be(expectedExitViewAsOrganisationLink);
            model.TestLogoutLink.Should().Be($"{ServiceConstants.PathBase}/account/logout");
            model.TestTermsAndConditionsLink.Should().Be(expectedTermsAndConditionsLink);
            model.TestViewYourSubservicesLink.Should().Be(expectedViewYourSubservicesLink);
            model.TestViewYourSubservicesText.Should().Be("Settings");

            Mock.VerifyAll(
                mockUrlHelper,
                Mock.Get(_urlHelperFactory));
        }

        private CommonPagePropertiesActionFilterAttribute GetTestFilter(DocumentExchangeConfiguration config)
        {
            return new CommonPagePropertiesActionFilterAttribute(
                _urlHelperFactory,
                Options.Create(config));
        }

        private (ActionExecutingContext context, ActionExecutionDelegate next) GetOnActionExecutionParameters(
            DocumentExchangeConfiguration config,
            TestViewModel model,
            BreadCrumbAttribute breadCrumbAttribute = null)
        {
            var controller = new DocumentExchangeController(
                _userInfoProvider,
                Options.Create(config));

            var httpContext = new DefaultHttpContext();

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

                if (breadCrumbAttribute != null)
                {
                    actionExecutedContext.Filters.Add(breadCrumbAttribute);
                }

                return Task.FromResult(actionExecutedContext);
            };

            return (context, next);
        }

        private class TestViewModel : BasePageViewModel, ISetBaseViewModelProperties
        {
            public IList<BreadCrumbViewModel> TestBreadCrumbs { get; set; }

            public string TestExitViewAsOrganisationLink { get; set; }

            public string TestHeaderTitle { get; set; }

            public string TestHeaderLink { get; set; }

            public string TestLogoutLink { get; set; }

            public string TestTermsAndConditionsLink { get; set; }

            public string TestViewYourSubservicesLink { get; set; }

            public string TestViewYourSubservicesText { get; set; }

            public void SetBreadCrumbs(IList<BreadCrumbViewModel> breadCrumbs)
            {
                TestBreadCrumbs = breadCrumbs;
            }

            public void SetExitViewAsOrganisationLink(string link)
            {
                TestExitViewAsOrganisationLink = link;
            }

            public void SetHeader(string title, string link)
            {
                TestHeaderTitle = title;
                TestHeaderLink = link;
            }

            public void SetLogoutLink(string link)
            {
                TestLogoutLink = link;
            }

            public void SetTermsAndConditionsLink(string link)
            {
                TestTermsAndConditionsLink = link;
            }

            public void SetViewYourSubServicesLink(string link)
            {
                TestViewYourSubservicesLink = link;
            }

            public void SetViewYourSubServicesText(string text)
            {
                TestViewYourSubservicesText = text;
            }
        }

        private class TestBreadCrumbAttribute : BreadCrumbAttribute
        {
            public override BreadCrumbViewModel ViewModel
                => new BreadCrumbViewModel();

            public override BreadCrumbAttribute GetParent(BreadCrumbData breadCrumbData)
                => null;
        }
    }
}