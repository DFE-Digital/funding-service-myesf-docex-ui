using AutoMapper;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pds.Core.Common.Identity.Models;
using Pds.Core.Common.Organisation.Enums;
using Pds.Core.Common.Organisation.Models;
using Pds.DocumentExchange.Services.Models;
using Pds.DocumentExchange.Web.Automapper;
using System.Collections.Generic;

namespace Pds.DocumentExchange.Web.Tests.Unit.AutoMapperProfiles
{
    [TestClass]
    [TestCategory("Unit")]
    public class UserMappingTests
    {
        private readonly MapperConfiguration _mapperConfiguration;
        private readonly Mapper _mapper;

        public UserMappingTests()
        {
            var profile = new UserMapping();
            _mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile(profile));

            _mapper = new Mapper(_mapperConfiguration);
        }

        [TestMethod]
        public void AutoMapperProfileMeetsExpectation()
        {
            // Act / Assert
            _mapperConfiguration.AssertConfigurationIsValid();
        }

        [TestMethod]
        public void MapUser_WhenUserIsExternal_ReturnsMappedUserInfo()
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
            var result = _mapper.Map<User, UserInfo>(sourceUser);

            // Assert
            result.Should().BeEquivalentTo(expectedDestinationUserInfo);
        }

        [TestMethod]
        public void MapUser_WhenUserIsInternal_ReturnsMappedUserInfo()
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
            var result = _mapper.Map<User, UserInfo>(sourceUser);

            // Assert
            result.Should().BeEquivalentTo(expectedDestinationUserInfo);
        }

        [TestMethod]
        public void MapUser_WhenUserIsInternalButIsImpersonating_ReturnsMappedUserInfoViewAsOrganisation()
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
            var result = _mapper.Map<User, UserInfo>(sourceUser);

            // Assert
            result.Should().BeEquivalentTo(expectedDestinationUserInfo);
        }
    }
}