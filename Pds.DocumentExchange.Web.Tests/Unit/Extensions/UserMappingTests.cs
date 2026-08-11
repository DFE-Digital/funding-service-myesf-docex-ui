using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pds.Core.Common.Identity.Models;
using Pds.Core.Common.Organisation.Enums;
using Pds.Core.Common.Organisation.Models;
using Pds.Core.Web.Models;
using Pds.DocumentExchange.Services.Models;
using Pds.DocumentExchange.Web.Extensions;
using System.Collections.Generic;

namespace Pds.DocumentExchange.Web.Tests.Unit.Extensions
{
    [TestClass]
    [TestCategory("Unit")]
    public class UserMappingTests
    {
        #region ToUserInfo

        [TestMethod]
        public void ToUserInfo_WhenUserIsExternal_ReturnsMappedUserInfo()
        {
            // Arrange
            var sourceUser = new User
            {
                IsAuthenticated = true,
                Ukprn = 12345678,
                IsExternalUser = true,
                ProviderName = "the-provider-name",
                Email = "the-provider-email@education.gov.uk",
                Roles = new List<string>() { "role-1", "role-2" },
                FirstName = "first-name",
                LastName = "last-name",
                FullName = "full-name",
                Principal = "principal"
            };

            var expectedDestinationUserInfo = new UserInfo
            {
                Principal = sourceUser.Principal,
                FullName = sourceUser.FullName,
                EmailAddress = sourceUser.Email,
                OrganisationInfo = new OrganisationInfo
                {
                    OrganisationIdentifier = new OrganisationIdentifier
                    {
                        Type = OrganisationIdentifierType.Ukprn,
                        Value = sourceUser.Ukprn.Value.ToString()
                    }
                },
                IsViewAsOrganisation = false
            };

            // Act
            var result = sourceUser.ToUserInfo();

            // Assert
            result.Should().BeEquivalentTo(expectedDestinationUserInfo);
        }

        [TestMethod]
        public void ToUserInfo_WhenUserIsInternal_ReturnsMappedUserInfo()
        {
            // Arrange
            var sourceUser = new User
            {
                IsAuthenticated = true,
                Ukprn = null,
                IsExternalUser = false,
                ProviderName = "the-provider-name",
                Email = "the-provider-email@education.gov.uk",
                Roles = new List<string>() { "role-1", "role-2" },
                FirstName = "first-name",
                LastName = "last-name",
                FullName = "full-name",
                Principal = "principal"
            };

            var expectedDestinationUserInfo = new UserInfo
            {
                Principal = sourceUser.Principal,
                FullName = sourceUser.FullName,
                EmailAddress = sourceUser.Email,
                OrganisationInfo = new OrganisationInfo
                {
                    OrganisationIdentifier = new OrganisationIdentifier
                    {
                        Type = OrganisationIdentifierType.Ukprn,
                        Value = null
                    }
                },
                IsViewAsOrganisation = false
            };

            // Act
            var result = sourceUser.ToUserInfo();

            // Assert
            result.Should().BeEquivalentTo(expectedDestinationUserInfo);
        }

        [TestMethod]
        public void ToUserInfo_WhenUserIsInternalButIsImpersonating_ReturnsMappedUserInfoViewAsOrganisation()
        {
            // Arrange
            var sourceUser = new User
            {
                IsAuthenticated = true,
                Ukprn = 12345678,
                IsExternalUser = false,
                ProviderName = "the-provider-name",
                Email = "the-provider-email@education.gov.uk",
                Roles = new List<string>() { "role-1", "role-2" },
                FirstName = "first-name",
                LastName = "last-name",
                FullName = "full-name",
                Principal = "principal"
            };

            var expectedDestinationUserInfo = new UserInfo
            {
                Principal = sourceUser.Principal,
                FullName = sourceUser.FullName,
                EmailAddress = sourceUser.Email,
                OrganisationInfo = new OrganisationInfo
                {
                    OrganisationIdentifier = new OrganisationIdentifier
                    {
                        Type = OrganisationIdentifierType.Ukprn,
                        Value = sourceUser.Ukprn.Value.ToString()
                    }
                },
                IsViewAsOrganisation = true
            };

            // Act
            var result = sourceUser.ToUserInfo();

            // Assert
            result.Should().BeEquivalentTo(expectedDestinationUserInfo);
        }

        #endregion


        #region ToCurrentUserViewModel

        [TestMethod]
        public void ToCurrentUserViewModel_WhenUserIsExternal_ReturnsMappedCurrentUserViewModel()
        {
            // Arrange
            var sourceUser = new User
            {
                IsAuthenticated = true,
                Ukprn = 12345678,
                IsExternalUser = true,
                ProviderName = "the-provider-name",
                Email = "the-provider-email@education.gov.uk",
                Roles = new List<string>() { "role-1", "role-2" },
                FirstName = "first-name",
                LastName = "last-name",
                FullName = "full-name",
                Principal = "principal"
            };

            var expectedDestinationCurrentUserViewModel = new CurrentUserViewModel
            {
                IsLoggedIn = sourceUser.IsAuthenticated,
                IsExternalUser = sourceUser.IsExternalUser,
                Ukprn = sourceUser.Ukprn,
                ProviderName = sourceUser.ProviderName,
                CanViewAsOrganisation = sourceUser.CanViewAsOrganisation,
                FirstName = sourceUser.FirstName,
                LastName = sourceUser.LastName,
                FullName = sourceUser.FullName
            };

            // Act
            var result = sourceUser.ToCurrentUserViewModel();

            // Assert
            result.Should().BeEquivalentTo(expectedDestinationCurrentUserViewModel);
        }

        [TestMethod]
        public void ToCurrentUserViewModel_WhenUserIsInternal_ReturnsMappedCurrentUserViewModel()
        {
            // Arrange
            var sourceUser = new User
            {
                IsAuthenticated = true,
                Ukprn = null,
                IsExternalUser = false,
                ProviderName = "the-provider-name",
                Email = "the-provider-email@education.gov.uk",
                Roles = new List<string>() { "role-1", "role-2" },
                FirstName = "first-name",
                LastName = "last-name",
                FullName = "full-name",
                Principal = "principal"
            };

            var expectedDestinationCurrentUserViewModel = new CurrentUserViewModel
            {
                IsLoggedIn = sourceUser.IsAuthenticated,
                IsExternalUser = sourceUser.IsExternalUser,
                Ukprn = sourceUser.Ukprn,
                ProviderName = sourceUser.ProviderName,
                CanViewAsOrganisation = sourceUser.CanViewAsOrganisation,
                FirstName = sourceUser.FirstName,
                LastName = sourceUser.LastName,
                FullName = sourceUser.FullName
            };

            // Act
            var result = sourceUser.ToCurrentUserViewModel();

            // Assert
            result.Should().BeEquivalentTo(expectedDestinationCurrentUserViewModel);
        }

        [TestMethod]
        public void ToCurrentUserViewModel_WhenUserIsInternalButIsImpersonating_ReturnsMappedCurrentUserViewModel()
        {
            // Arrange
            var sourceUser = new User
            {
                IsAuthenticated = true,
                Ukprn = 12345678,
                IsExternalUser = false,
                ProviderName = "the-provider-name",
                Email = "the-provider-email@education.gov.uk",
                Roles = new List<string>() { "role-1", "role-2" },
                FirstName = "first-name",
                LastName = "last-name",
                FullName = "full-name",
                Principal = "principal"
            };

            var expectedDestinationCurrentUserViewModel = new CurrentUserViewModel
            {
                IsLoggedIn = sourceUser.IsAuthenticated,
                IsExternalUser = sourceUser.IsExternalUser,
                Ukprn = sourceUser.Ukprn,
                ProviderName = sourceUser.ProviderName,
                CanViewAsOrganisation = sourceUser.CanViewAsOrganisation,
                FirstName = sourceUser.FirstName,
                LastName = sourceUser.LastName,
                FullName = sourceUser.FullName
            };

            // Act
            var result = sourceUser.ToCurrentUserViewModel();

            // Assert
            result.Should().BeEquivalentTo(expectedDestinationCurrentUserViewModel);
        }

        #endregion
    }
}