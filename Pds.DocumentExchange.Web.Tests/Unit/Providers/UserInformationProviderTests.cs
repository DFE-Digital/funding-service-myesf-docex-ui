using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.Core.Common.Identity.Enums;
using Pds.Core.Common.Identity.Models;
using Pds.Core.Common.Organisation.Enums;
using Pds.Core.Common.Organisation.Models;
using Pds.Core.Identity.Claims.Interfaces;
using Pds.Core.Web.Models;
using Pds.DocumentExchange.Services.Interfaces;
using Pds.DocumentExchange.Services.Models;
using Pds.DocumentExchange.Web.Implementations.Providers;
using Pds.DocumentExchange.Web.Interfaces.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Pds.DocumentExchange.Web.Tests.Unit.Providers
{
    [TestClass, TestCategory("Unit")]
    public sealed class UserInformationProviderTests
    {
        private readonly IClaimsBasedIdentityService _identityService
            = Mock.Of<IClaimsBasedIdentityService>(MockBehavior.Strict);

        private readonly IMapper _mapper
            = Mock.Of<IMapper>(MockBehavior.Strict);

        private readonly IOrganisationApiClient _organisationApiClient
            = Mock.Of<IOrganisationApiClient>(MockBehavior.Strict);

        [TestMethod]
        public async Task GetCurrentUserInfo_ReturnsMappedValue()
        {
            // Arrange
            var testUser = new User();
            var testUserInfo = new UserInfo();

            Mock.Get(_mapper)
                .Setup(m => m.ToUserInfo(testUser))
                .Returns(testUserInfo);

            var testProvider = await GetTestProvider(testUser);

            // Act
            var actual = await testProvider.GetCurrentUserInfo();

            // Assert
            actual.Should().Be(testUserInfo);

            Mock.VerifyAll(
                Mock.Get(_identityService),
                Mock.Get(_mapper));
        }

        [TestMethod]
        public async Task GetCurrentUserViewModel_ReturnsMappedValue()
        {
            // Arrange
            var testUser = new User();
            var testUserViewModel = new CurrentUserViewModel();

            Mock.Get(_mapper)
                .Setup(m => m.ToCurrentUserViewModel(testUser))
                .Returns(testUserViewModel);

            var testProvider = await GetTestProvider(testUser);

            // Act
            var actual = await testProvider.GetCurrentUserViewModel();

            // Assert
            actual.Should().Be(testUserViewModel);

            Mock.VerifyAll(
                Mock.Get(_identityService),
                Mock.Get(_mapper));
        }

        [TestMethod, DynamicData(nameof(CurrentUserCanAccessDocumentExchange_ReturnsExpected_TestData))]
        public async Task CurrentUserCanAccessDocumentExchange_ReturnsExpected(User user, bool expectedResult)
        {
            // Arrange
            var testProvider = await GetTestProvider(user);

            // Act
            var actual = await testProvider.CurrentUserCanAccessDocumentExchange();

            // Assert
            actual.Should().Be(expectedResult);

            Mock.VerifyAll(
                Mock.Get(_identityService));
        }

        [TestMethod, DynamicData(nameof(CurrentUserCanAccessSupportTools_ReturnsExpected_TestData))]
        public async Task CurrentUserCanAccessSupportTools_ReturnsExpected(User user, bool expectedResult)
        {
            // Arrange
            var testProvider = await GetTestProvider(user);

            // Act
            var actual = await testProvider.CurrentUserCanAccessToSupportTools();

            // Assert
            actual.Should().Be(expectedResult);

            Mock.VerifyAll(
                Mock.Get(_identityService));
        }

        [TestMethod, DynamicData(nameof(GetCurrentUserPermissions_ForInternal_ReturnsExpected_TestData))]
        public async Task GetCurrentUserPermissions_ForInternal_ReturnsExpected(User user, IEnumerable<string> expectedResult)
        {
            // Arrange
            var testProvider = await GetTestProvider(user);

            // Act
            var actual = await testProvider.GetCurrentUserPermissions();

            // Assert
            actual.Should().BeEquivalentTo(expectedResult);

            Mock.VerifyAll(
                Mock.Get(_identityService));
        }

        [TestMethod]
        public async Task GetCurrentUserPermissions_ForExternalChild_ReturnsExpected()
        {
            // Arrange
            var testUser = new User
            {
                IsExternalUser = true,
                IsAuthenticated = true
            };

            var testOrgIdentifier = new OrganisationIdentifier
            {
                Type = OrganisationIdentifierType.Ukprn,
                Value = "12345678"
            };

            var testUserInfo = new UserInfo
            {
                OrganisationInfo = new OrganisationInfo
                {
                    OrganisationIdentifier = testOrgIdentifier
                }
            };

            var testOrganisation = GetTestChildOrganisation();

            Mock.Get(_mapper)
                .Setup(m => m.ToUserInfo(testUser))
                .Returns(testUserInfo);

            Mock.Get(_organisationApiClient)
                .Setup(o => o.GetOrganisation(testOrgIdentifier))
                .ReturnsAsync(testOrganisation);

            var testProvider = await GetTestProvider(testUser);

            // Act
            var actual = await testProvider.GetCurrentUserPermissions();

            // Assert
            actual.Should().BeEquivalentTo(new[]
            {
                "view and download documents that were sent to your organisation",
                "send documents on behalf of your organisation"
            });

            Mock.VerifyAll(
                Mock.Get(_identityService));
        }

        [TestMethod]
        public async Task GetCurrentUserPermissions_ForExternalParent_ReturnsExpected()
        {
            // Arrange
            var testUser = new User
            {
                IsExternalUser = true,
                IsAuthenticated = true
            };

            var testOrgIdentifier = new OrganisationIdentifier
            {
                Type = OrganisationIdentifierType.Ukprn,
                Value = "12345678"
            };

            var testUserInfo = new UserInfo
            {
                OrganisationInfo = new OrganisationInfo
                {
                    OrganisationIdentifier = testOrgIdentifier
                }
            };

            var testOrganisation = GetTestParentOrganisation(1);

            Mock.Get(_mapper)
                .Setup(m => m.ToUserInfo(testUser))
                .Returns(testUserInfo);

            Mock.Get(_organisationApiClient)
                .Setup(o => o.GetOrganisation(testOrgIdentifier))
                .ReturnsAsync(testOrganisation);

            var testProvider = await GetTestProvider(testUser);

            // Act
            var actual = await testProvider.GetCurrentUserPermissions();

            // Assert
            actual.Should().BeEquivalentTo(new[]
            {
                "view and download documents that were sent to your group of academies or your MAT",
                "send documents on behalf of your group of academies or your MAT"
            });

            Mock.VerifyAll(
                Mock.Get(_identityService));
        }

        [TestMethod]
        public async Task CurrentUserIsOrganisationUserOrImpersonating_ForNullUser_ReturnsFalse()
        {
            // Arrange
            var testProvider = await GetTestProvider(null);

            // Act
            var actual = await testProvider.CurrentUserIsOrganisationUserOrImpersonating();

            // Assert
            actual.Should().BeFalse();

            Mock.VerifyAll(
                Mock.Get(_identityService));
        }

        [TestMethod]
        public async Task CurrentUserIsOrganisationUserOrImpersonating_ForUnAuthenticatedUser_ReturnsFalse()
        {
            // Arrange
            var testProvider = await GetTestProvider(new User { IsAuthenticated = false });

            // Act
            var actual = await testProvider.CurrentUserIsOrganisationUserOrImpersonating();

            // Assert
            actual.Should().BeFalse();

            Mock.VerifyAll(
                Mock.Get(_identityService));
        }

        [TestMethod]
        public async Task CurrentUserIsOrganisationUserOrImpersonating_ForExternalUser_ReturnsTrue()
        {
            // Arrange
            var testProvider = await GetTestProvider(new User { IsAuthenticated = true, IsExternalUser = true });

            // Act
            var actual = await testProvider.CurrentUserIsOrganisationUserOrImpersonating();

            // Assert
            actual.Should().BeTrue();

            Mock.VerifyAll(
                Mock.Get(_identityService));
        }

        [TestMethod]
        public async Task CurrentUserIsOrganisationUserOrImpersonating_ForImpersonatedExternalUser_ReturnsTrue()
        {
            // Arrange
            var testProvider = await GetTestProvider(new User { IsAuthenticated = true, IsExternalUser = false, CanViewAsOrganisation = true, Ukprn = 1 });

            // Act
            var actual = await testProvider.CurrentUserIsOrganisationUserOrImpersonating();

            // Assert
            actual.Should().BeTrue();

            Mock.VerifyAll(
                Mock.Get(_identityService));
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task CurrentUserIsOrganisationUserOrImpersonating_ForInternalUser_ReturnsFalse(bool canViewAsOrganisation)
        {
            // Arrange
            var testProvider = await GetTestProvider(new User { IsAuthenticated = true, IsExternalUser = false, CanViewAsOrganisation = canViewAsOrganisation, Ukprn = null });

            // Act
            var actual = await testProvider.CurrentUserIsOrganisationUserOrImpersonating();

            // Assert
            actual.Should().BeFalse();

            Mock.VerifyAll(
                Mock.Get(_identityService));
        }

        [TestMethod, DynamicData(nameof(UserInfosWithNullOrEmptyOrgIdValue))]
        public async Task GetCurrentOrganisationIdentifier_ForNullOrEmptyValue_Throws(UserInfo userInfoWithoutOrgId)
        {
            // Arrange
            var testUser = new User();

            Mock.Get(_mapper)
                .Setup(m => m.ToUserInfo(testUser))
                .Returns(userInfoWithoutOrgId);

            var testProvider = await GetTestProvider(testUser);

            // Act
            Func<Task> act = () => testProvider.GetCurrentOrganisationIdentifier();

            // Assert
            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage($"Missing organisation identifier for user {userInfoWithoutOrgId?.Principal}");

            Mock.VerifyAll(
                Mock.Get(_identityService),
                Mock.Get(_mapper));
        }

        [TestMethod]
        [DataRow(OrganisationIdentifierType.CompanyRegistrationNumber, "12345")]
        [DataRow(OrganisationIdentifierType.Ukprn, "12345")]
        [DataRow(OrganisationIdentifierType.CompanyRegistrationNumber, "54321")]
        [DataRow(OrganisationIdentifierType.Ukprn, "54321")]
        public async Task GetCurrentOrganisationIdentifier_ReturnsMappedValue(OrganisationIdentifierType expectedType, string expectedValue)
        {
            // Arrange
            var testUser = new User();

            var expectedIdentifier = new OrganisationIdentifier
            {
                Type = expectedType,
                Value = expectedValue
            };

            var testUserInfo = new UserInfo
            {
                OrganisationInfo = new OrganisationInfo
                {
                    OrganisationIdentifier = expectedIdentifier
                }
            };

            Mock.Get(_mapper)
                .Setup(m => m.ToUserInfo(testUser))
                .Returns(testUserInfo);

            var testProvider = await GetTestProvider(testUser);

            // Act
            var actual = await testProvider.GetCurrentOrganisationIdentifier();

            // Assert
            actual.Should().Be(expectedIdentifier);

            Mock.VerifyAll(
                Mock.Get(_identityService),
                Mock.Get(_mapper));
        }

        [TestMethod, DynamicData(nameof(UserInfosWithNullOrEmptyOrgIdValue))]
        public async Task GetCurrentUserChildOrganisations_ForNullOrEmptyOrgId_Throws(UserInfo userInfoWithoutOrgId)
        {
            // Arrange
            var testUser = new User();

            Mock.Get(_mapper)
                .Setup(m => m.ToUserInfo(testUser))
                .Returns(userInfoWithoutOrgId);

            var testProvider = await GetTestProvider(testUser);

            // Act
            Func<Task> act = () => testProvider.GetCurrentUserChildOrganisations();

            // Assert
            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage($"Missing organisation identifier for user {userInfoWithoutOrgId?.Principal}");

            Mock.VerifyAll(
                Mock.Get(_identityService),
                Mock.Get(_mapper));
        }

        [TestMethod]
        public async Task GetCurrentUserChildOrganisations_WhenOrgNotFound_ReturnsNull()
        {
            // Arrange
            var testUser = new User();

            var testOrgIdentifier = new OrganisationIdentifier
            {
                Type = OrganisationIdentifierType.Ukprn,
                Value = "12345678"
            };

            var testUserInfo = new UserInfo
            {
                OrganisationInfo = new OrganisationInfo
                {
                    OrganisationIdentifier = testOrgIdentifier
                }
            };

            Mock.Get(_mapper)
                .Setup(m => m.ToUserInfo(testUser))
                .Returns(testUserInfo);

            Mock.Get(_organisationApiClient)
                .Setup(o => o.GetOrganisation(testOrgIdentifier))
                .ReturnsAsync((Organisation)null);

            var testProvider = await GetTestProvider(testUser);

            // Act
            var actual = await testProvider.GetCurrentUserChildOrganisations();

            // Assert
            actual.Should().BeNull();

            Mock.VerifyAll(
                Mock.Get(_identityService),
                Mock.Get(_mapper),
                Mock.Get(_organisationApiClient));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(37)]
        [DataRow(123)]
        public async Task GetCurrentUserChildOrganisations_WhenOrgFound_ReturnsOrgChildren(int numberOfChildOrganisations)
        {
            // Arrange
            var testUser = new User();

            var testOrgIdentifier = new OrganisationIdentifier
            {
                Type = OrganisationIdentifierType.Ukprn,
                Value = "12345678"
            };

            var testUserInfo = new UserInfo
            {
                OrganisationInfo = new OrganisationInfo
                {
                    OrganisationIdentifier = testOrgIdentifier
                }
            };

            var testOrganisation = GetTestParentOrganisation(numberOfChildOrganisations);

            Mock.Get(_mapper)
                .Setup(m => m.ToUserInfo(testUser))
                .Returns(testUserInfo);

            Mock.Get(_organisationApiClient)
                .Setup(o => o.GetOrganisation(testOrgIdentifier))
                .ReturnsAsync(testOrganisation);

            var testProvider = await GetTestProvider(testUser);

            // Act
            var actual = await testProvider.GetCurrentUserChildOrganisations();

            // Assert
            actual.Should().BeEquivalentTo(testOrganisation.ChildOrganisations);

            Mock.VerifyAll(
                Mock.Get(_identityService),
                Mock.Get(_mapper),
                Mock.Get(_organisationApiClient));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(37)]
        [DataRow(123)]
        public async Task GetCurrentUserChildOrganisations_WhenCalledTwice_UsesSavedResult(int numberOfChildOrganisations)
        {
            // Arrange
            var testUser = new User();

            var testOrgIdentifier = new OrganisationIdentifier
            {
                Type = OrganisationIdentifierType.Ukprn,
                Value = "12345678"
            };

            var testUserInfo = new UserInfo
            {
                OrganisationInfo = new OrganisationInfo
                {
                    OrganisationIdentifier = testOrgIdentifier
                }
            };

            var testOrganisation = GetTestParentOrganisation(numberOfChildOrganisations);

            Mock.Get(_mapper)
                .Setup(m => m.ToUserInfo(testUser))
                .Returns(testUserInfo);

            Mock.Get(_organisationApiClient)
                .Setup(o => o.GetOrganisation(testOrgIdentifier))
                .ReturnsAsync(testOrganisation);

            var testProvider = await GetTestProvider(testUser);

            // Act
            await testProvider.GetCurrentUserChildOrganisations();
            var actual = await testProvider.GetCurrentUserChildOrganisations();

            // Assert
            actual.Should().BeEquivalentTo(testOrganisation.ChildOrganisations);

            Mock.VerifyAll(
                Mock.Get(_identityService),
                Mock.Get(_mapper),
                Mock.Get(_organisationApiClient));

            Mock.Get(_organisationApiClient)
                .Verify(o => o.GetOrganisation(testOrgIdentifier), Times.Once);
        }

        [TestMethod]
        public async Task CurrentUserShouldSeeParentView_WhenOrgNotFound_ReturnsFalse()
        {
            // Arrange
            var testUser = new User();

            var testOrgIdentifier = new OrganisationIdentifier
            {
                Type = OrganisationIdentifierType.Ukprn,
                Value = "12345678"
            };

            var testUserInfo = new UserInfo
            {
                OrganisationInfo = new OrganisationInfo
                {
                    OrganisationIdentifier = testOrgIdentifier
                }
            };

            Mock.Get(_mapper)
                .Setup(m => m.ToUserInfo(testUser))
                .Returns(testUserInfo);

            Mock.Get(_organisationApiClient)
                .Setup(o => o.GetOrganisation(testOrgIdentifier))
                .ReturnsAsync((Organisation)null);

            var testProvider = await GetTestProvider(testUser);

            // Act
            var actual = await testProvider.CurrentUserShouldSeeParentView();

            // Assert
            actual.Should().BeFalse();

            Mock.VerifyAll(
                Mock.Get(_identityService),
                Mock.Get(_mapper),
                Mock.Get(_organisationApiClient));
        }

        [TestMethod]
        [DataRow(0, false)]
        [DataRow(1, true)]
        [DataRow(37, true)]
        [DataRow(123, true)]
        public async Task CurrentUserShouldSeeParentView_WhenOrgFound_ReturnsExpected(int numberOfChildOrganisations, bool expectedResult)
        {
            // Arrange
            var testUser = new User();

            var testOrgIdentifier = new OrganisationIdentifier
            {
                Type = OrganisationIdentifierType.Ukprn,
                Value = "12345678"
            };

            var testUserInfo = new UserInfo
            {
                OrganisationInfo = new OrganisationInfo
                {
                    OrganisationIdentifier = testOrgIdentifier
                }
            };

            var testOrganisation = GetTestParentOrganisation(numberOfChildOrganisations);

            Mock.Get(_mapper)
                .Setup(m => m.ToUserInfo(testUser))
                .Returns(testUserInfo);

            Mock.Get(_organisationApiClient)
                .Setup(o => o.GetOrganisation(testOrgIdentifier))
                .ReturnsAsync(testOrganisation);

            var testProvider = await GetTestProvider(testUser);

            // Act
            var actual = await testProvider.CurrentUserShouldSeeParentView();

            // Assert
            actual.Should().Be(expectedResult);

            Mock.VerifyAll(
                Mock.Get(_identityService),
                Mock.Get(_mapper),
                Mock.Get(_organisationApiClient));
        }

        [TestMethod, DynamicData(nameof(CurrentUserIsAdvancedAgencyUser_ReturnsExpected_TestData))]
        public async Task CurrentUserIsAdvancedAgencyUser_ReturnsExpected(User user, bool expectedResult)
        {
            // Arrange
            var testProvider = await GetTestProvider(user);

            // Act
            var actual = await testProvider.CurrentUserIsAdvancedAgencyUser();

            // Assert
            actual.Should().Be(expectedResult);

            Mock.VerifyAll(
                Mock.Get(_identityService));
        }

        [TestMethod, DynamicData(nameof(GetCurrentUserAgencyTeams_ReturnsExpected_TestData))]
        public async Task GetCurrentUserAgencyTeams_ReturnsExpected(User user, string expectedResult)
        {
            // Arrange
            var testProvider = await GetTestProvider(user);

            // Act
            var actual = await testProvider.GetCurrentUserAgencyTeams();

            // Assert
            actual.Should().Be(expectedResult);

            Mock.VerifyAll(
                Mock.Get(_identityService));
        }

        [TestMethod, DynamicData(nameof(CurrentUserCanViewAsOrganisation_ReturnsExpected_TestData))]
        public async Task CurrentUserCanViewAsOrganisation_ReturnsExpected(User user, bool expectedResult)
        {
            // Arrange
            var testProvider = await GetTestProvider(user);

            // Act
            var actual = await testProvider.CurrentUserCanViewAsOrganisation();

            // Assert
            actual.Should().Be(expectedResult);

            Mock.VerifyAll(
                Mock.Get(_identityService));
        }

        [TestMethod, DynamicData(nameof(CurrentUserIsAdminUser_ReturnsExpected_TestData))]
        public async Task CurrentUserIsAdminUser_ReturnsExpected(User user, bool expectedResult)
        {
            // Arrange
            var testProvider = await GetTestProvider(user);

            // Act
            var actual = await testProvider.CurrentUserIsAdminUser();

            // Assert
            actual.Should().Be(expectedResult);

            Mock.VerifyAll(
                Mock.Get(_identityService));
        }

        private async Task<UserInformationProvider> GetTestProvider(User testUser)
        {
            Mock.Get(_identityService)
                .Setup(s => s.GetUserFromClaims(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(testUser);

            var provider = new UserInformationProvider(
                _identityService,
                _mapper,
                _organisationApiClient);

            await provider.Initialise(new ClaimsPrincipal());

            return provider;
        }

        private static IEnumerable<object[]> UserInfosWithNullOrEmptyOrgIdValue
        {
            get
            {
                yield return new object[]
                {
                    null
                };
                yield return new object[]
                {
                    new UserInfo
                    {
                        Principal = "fake user 1"
                    }
                };
                yield return new object[]
                {
                    new UserInfo
                    {
                        Principal = "fake user 2",
                        OrganisationInfo = new OrganisationInfo()
                    }
                };
                yield return new object[]
                {
                    new UserInfo
                    {
                        Principal = "fake user 3",
                        OrganisationInfo = new OrganisationInfo
                        {
                            OrganisationIdentifier = new OrganisationIdentifier()
                        }
                    }
                };
                yield return new object[]
                {
                    new UserInfo
                    {
                        Principal = "fake user 4",
                        OrganisationInfo = new OrganisationInfo
                        {
                            OrganisationIdentifier = new OrganisationIdentifier
                            {
                                Value = string.Empty
                            }
                        }
                    }
                };
            }
        }

        private static IEnumerable<object[]> CurrentUserCanAccessDocumentExchange_ReturnsExpected_TestData
        {
            get
            {
                yield return new object[]
                {
                    null,
                    false
                };
                yield return new object[]
                {
                    new User(),
                    false
                };
                yield return new object[]
                {
                    new User
                    {
                        Roles = new List<string>()
                    },
                    false
                };
                yield return new object[]
                {
                    new User
                    {
                        Roles = new List<string>
                        {
                            UserRole.DocumentExchangeUser.ToString()
                        }
                    },
                    true
                };
                yield return new object[]
                {
                    new User
                    {
                        Roles = new List<string>
                        {
                            UserRole.DocumentExchangeAdministratorFundingCentre.ToString()
                        }
                    },
                    true
                };
                yield return new object[]
                {
                    new User
                    {
                        Roles = new List<string>
                        {
                            UserRole.DocumentExchangeAdvancedUser.ToString()
                        }
                    },
                    true
                };
                yield return new object[]
                {
                    new User
                    {
                        Roles = new List<string>
                        {
                            UserRole.ViewAsProvider.ToString()
                        }
                    },
                    true
                };
                yield return new object[]
                {
                    new User
                    {
                        Roles = new List<string>
                        {
                            UserRole.DocumentExchangeAdmin.ToString()
                        }
                    },
                    true
                };
                yield return new object[]
                {
                    new User
                    {
                        Roles = new List<string>
                        {
                            UserRole.SignContractsAndAgreements.ToString()
                        }
                    },
                    false
                };
                yield return new object[]
                {
                    new User
                    {
                        Roles = new List<string>
                        {
                            UserRole.ApprenticeshipsEditor.ToString()
                        }
                    },
                    false
                };
            }
        }

        private static IEnumerable<object[]> CurrentUserCanAccessSupportTools_ReturnsExpected_TestData
        {
            get
            {
                yield return new object[]
                {
                    null,
                    false
                };
                yield return new object[]
                {
                    new User(),
                    false
                };
                yield return new object[]
                {
                    new User
                    {
                        Roles = new List<string>()
                    },
                    false
                };
                yield return new object[]
                {
                    new User
                    {
                        Roles = new List<string>
                        {
                            UserRole.DocumentExchangeUser.ToString()
                        }
                    },
                    false
                };
                yield return new object[]
                {
                    new User
                    {
                        Roles = new List<string>
                        {
                            UserRole.DocumentExchangeAdministratorFundingCentre.ToString()
                        }
                    },
                    true
                };
                yield return new object[]
                {
                    new User
                    {
                        Roles = new List<string>
                        {
                            UserRole.DocumentExchangeAdministratorRiskAssurance.ToString()
                        }
                    },
                    true
                };
                yield return new object[]
                {
                    new User
                    {
                        Roles = new List<string>
                        {
                            UserRole.DocumentExchangeAdvancedUser.ToString()
                        }
                    },
                    true
                };
                yield return new object[]
                {
                    new User
                    {
                        Roles = new List<string>
                        {
                            UserRole.ViewAsProvider.ToString()
                        }
                    },
                    false
                };
                yield return new object[]
                {
                    new User
                    {
                        Roles = new List<string>
                        {
                            UserRole.DocumentExchangeAdmin.ToString()
                        }
                    },
                    true
                };
                yield return new object[]
                {
                    new User
                    {
                        Roles = new List<string>
                        {
                            UserRole.SignContractsAndAgreements.ToString()
                        }
                    },
                    false
                };
                yield return new object[]
                {
                    new User
                    {
                        Roles = new List<string>
                        {
                            UserRole.ApprenticeshipsEditor.ToString()
                        }
                    },
                    false
                };
            }
        }

        private static IEnumerable<object[]> GetCurrentUserPermissions_ForInternal_ReturnsExpected_TestData
        {
            get
            {
                yield return new object[]
                {
                    null,
                    Array.Empty<string>()
                };
                yield return new object[]
                {
                    new User(),
                    Array.Empty<string>()
                };
                yield return new object[]
                {
                    new User
                    {
                        Roles = new List<string>()
                    },
                    Array.Empty<string>()
                };
                yield return new object[]
                {
                    new User
                    {
                        IsAuthenticated = true,
                        IsExternalUser = false,
                        Roles = new List<string>
                        {
                            UserRole.DocumentExchangeAdministratorFundingCentre.ToString()
                        }
                    },
                    new[]
                    {
                        "view or download documents uploaded by external users for your team",
                        "publish documents from your fileshare"
                    }
                };
                yield return new object[]
                {
                    new User
                    {
                        IsAuthenticated = true,
                        IsExternalUser = false,
                        Roles = new List<string>
                        {
                            UserRole.DocumentExchangeAdministratorFundingCentre.ToString(),
                            UserRole.ViewAsProvider.ToString()
                        }
                    },
                    new[]
                    {
                        "view or download documents uploaded by external users for your team",
                        "publish documents from your fileshare",
                        "view or download the documents that have been sent to any external organisation"
                    }
                };
                yield return new object[]
                {
                    new User
                    {
                        Roles = new List<string>
                        {
                            UserRole.DocumentExchangeAdvancedUser.ToString()
                        }
                    },
                    new[]
                    {
                        "publish documents from any fileshare",
                        "view, download or delete documents uploaded by external users"
                    }
                };
                yield return new object[]
                {
                    new User
                    {
                        Roles = new List<string>
                        {
                            UserRole.DocumentExchangeAdvancedUser.ToString(),
                            UserRole.DocumentExchangeAdmin.ToString()
                        }
                    },
                    new[]
                    {
                        "publish documents from any fileshare",
                        "view, download or delete documents uploaded by external users",
                        "add new products",
                        "view and edit settings"
                    }
                };
                yield return new object[]
                {
                    new User
                    {
                        Roles = new List<string>
                        {
                            UserRole.DocumentExchangeAdvancedUser.ToString(),
                            UserRole.DocumentExchangeAdmin.ToString(),
                            UserRole.ViewAsProvider.ToString()
                        }
                    },
                    new[]
                    {
                        "publish documents from any fileshare",
                        "view, download or delete documents uploaded by external users",
                        "add new products",
                        "view and edit settings",
                        "view or download the documents that have been sent to any external organisation"
                    }
                };
                yield return new object[]
                {
                    new User
                    {
                        Roles = new List<string>
                        {
                            UserRole.ViewAsProvider.ToString()
                        }
                    },
                    new[]
                    {
                        "view or download the documents that have been sent to any external organisation"
                    }
                };
                yield return new object[]
                {
                    new User
                    {
                        Roles = new List<string>
                        {
                            UserRole.DocumentExchangeAdmin.ToString()
                        }
                    },
                    new[]
                    {
                        "add new products",
                        "view and edit settings"
                    }
                };
                yield return new object[]
                {
                    new User
                    {
                        Roles = new List<string>
                        {
                            UserRole.SignContractsAndAgreements.ToString()
                        }
                    },
                    Array.Empty<string>()
                };
                yield return new object[]
                {
                    new User
                    {
                        Roles = new List<string>
                        {
                            UserRole.ApprenticeshipsEditor.ToString()
                        }
                    },
                    Array.Empty<string>()
                };
            }
        }

        private static IEnumerable<object[]> CurrentUserIsAdvancedAgencyUser_ReturnsExpected_TestData
        {
            get
            {
                yield return new object[]
                {
                    null,
                    false
                };
                yield return new object[]
                {
                    new User(),
                    false
                };
                yield return new object[]
                {
                    new User
                    {
                        Roles = new List<string>()
                    },
                    false
                };
                yield return new object[]
                {
                    new User
                    {
                        Roles = new List<string>
                        {
                            UserRole.DocumentExchangeAdministratorFundingCentre.ToString(),
                            UserRole.DocumentExchangeAdministratorRiskAssurance.ToString(),
                            UserRole.DocumentExchangeAdmin.ToString(),
                            UserRole.DocumentExchangeUser.ToString()
                        }
                    },
                    false
                };
                yield return new object[]
                {
                    new User
                    {
                        Roles = new List<string>
                        {
                            UserRole.DocumentExchangeAdministratorFundingCentre.ToString(),
                            UserRole.DocumentExchangeAdministratorRiskAssurance.ToString(),
                            UserRole.DocumentExchangeAdmin.ToString(),
                            UserRole.DocumentExchangeUser.ToString(),
                            UserRole.DocumentExchangeAdvancedUser.ToString()
                        }
                    },
                    true
                };
                yield return new object[]
                {
                    new User
                    {
                        Roles = new List<string>
                        {
                            UserRole.DocumentExchangeAdvancedUser.ToString()
                        }
                    },
                    true
                };
            }
        }

        private static IEnumerable<object[]> GetCurrentUserAgencyTeams_ReturnsExpected_TestData
        {
            get
            {
                yield return new object[]
                {
                    null,
                    string.Empty
                };
                yield return new object[]
                {
                    new User(),
                    string.Empty
                };
                yield return new object[]
                {
                    new User
                    {
                        IsAuthenticated = true,
                        IsExternalUser = true
                    },
                    string.Empty
                };
                yield return new object[]
                {
                    new User
                    {
                        IsAuthenticated = true,
                        Roles = new List<string>()
                    },
                    string.Empty
                };
                yield return new object[]
                {
                    new User
                    {
                        IsAuthenticated = true,
                        Roles = new List<string>
                        {
                            UserRole.DocumentExchangeAdministratorFundingCentre.ToString(),
                            UserRole.DocumentExchangeAdmin.ToString(),
                            UserRole.DocumentExchangeUser.ToString()
                        }
                    },
                    $"{UserRole.DocumentExchangeAdministratorFundingCentre}"
                };
                yield return new object[]
                {
                    new User
                    {
                        IsAuthenticated = true,
                        Roles = new List<string>
                        {
                            UserRole.DocumentExchangeAdministratorRiskAssurance.ToString(),
                            UserRole.DocumentExchangeAdmin.ToString(),
                            UserRole.DocumentExchangeUser.ToString()
                        }
                    },
                    $"{UserRole.DocumentExchangeAdministratorRiskAssurance}"
                };
                yield return new object[]
                {
                    new User
                    {
                        IsAuthenticated = true,
                        Roles = new List<string>
                        {
                            UserRole.DocumentExchangeAdministratorFundingCentre.ToString(),
                            UserRole.DocumentExchangeAdministratorRiskAssurance.ToString(),
                            UserRole.DocumentExchangeAdmin.ToString(),
                            UserRole.DocumentExchangeUser.ToString()
                        }
                    },
                    $"{UserRole.DocumentExchangeAdministratorFundingCentre},{UserRole.DocumentExchangeAdministratorRiskAssurance}"
                };
                yield return new object[]
                {
                    new User
                    {
                        IsAuthenticated = true,
                        Roles = new List<string>
                        {
                            UserRole.DocumentExchangeAdministratorFundingCentre.ToString(),
                            UserRole.DocumentExchangeAdministratorRiskAssurance.ToString(),
                            UserRole.DocumentExchangeAdmin.ToString(),
                            UserRole.DocumentExchangeUser.ToString(),
                            UserRole.DocumentExchangeAdvancedUser.ToString()
                        }
                    },
                    $"{UserRole.DocumentExchangeAdministratorFundingCentre},{UserRole.DocumentExchangeAdministratorRiskAssurance}"
                };
                yield return new object[]
                {
                    new User
                    {
                        IsAuthenticated = true,
                        Roles = new List<string>
                        {
                            UserRole.DocumentExchangeAdvancedUser.ToString()
                        }
                    },
                    $"{UserRole.DocumentExchangeAdministratorFundingCentre},{UserRole.DocumentExchangeAdministratorRiskAssurance}"
                };
            }
        }

        private static IEnumerable<object[]> CurrentUserCanViewAsOrganisation_ReturnsExpected_TestData
        {
            get
            {
                yield return new object[]
                {
                    null,
                    false
                };
                yield return new object[]
                {
                    new User(),
                    false
                };
                yield return new object[]
                {
                    new User
                    {
                        Roles = new List<string>()
                    },
                    false
                };
                yield return new object[]
                {
                    new User
                    {
                        Roles = new List<string>
                        {
                            UserRole.DocumentExchangeAdministratorFundingCentre.ToString(),
                            UserRole.DocumentExchangeAdministratorRiskAssurance.ToString(),
                            UserRole.DocumentExchangeAdmin.ToString(),
                            UserRole.DocumentExchangeUser.ToString()
                        }
                    },
                    false
                };
                yield return new object[]
                {
                    new User
                    {
                        Roles = new List<string>
                        {
                            UserRole.DocumentExchangeAdministratorFundingCentre.ToString(),
                            UserRole.DocumentExchangeAdministratorRiskAssurance.ToString(),
                            UserRole.ViewAsProvider.ToString(),
                            UserRole.DocumentExchangeUser.ToString(),
                            UserRole.DocumentExchangeAdvancedUser.ToString()
                        }
                    },
                    true
                };
                yield return new object[]
                {
                    new User
                    {
                        Roles = new List<string>
                        {
                            UserRole.ViewAsProvider.ToString()
                        }
                    },
                    true
                };
            }
        }

        private static IEnumerable<object[]> CurrentUserIsAdminUser_ReturnsExpected_TestData
        {
            get
            {
                yield return new object[]
                {
                    null,
                    false
                };
                yield return new object[]
                {
                    new User(),
                    false
                };
                yield return new object[]
                {
                    new User
                    {
                        Roles = new List<string>()
                    },
                    false
                };
                yield return new object[]
                {
                    new User
                    {
                        Roles = new List<string>
                        {
                            UserRole.DocumentExchangeAdministratorFundingCentre.ToString(),
                            UserRole.DocumentExchangeAdministratorRiskAssurance.ToString(),
                            UserRole.DocumentExchangeAdvancedUser.ToString(),
                            UserRole.DocumentExchangeUser.ToString()
                        }
                    },
                    false
                };
                yield return new object[]
                {
                    new User
                    {
                        Roles = new List<string>
                        {
                            UserRole.DocumentExchangeAdmin.ToString()
                        }
                    },
                    true
                };
            }
        }

        private Organisation GetTestParentOrganisation(int numberOfChildren)
            => new Organisation
            {
                Name = "parent org",
                Identifiers = new List<OrganisationIdentifier>
                {
                    new OrganisationIdentifier
                    {
                        Type = OrganisationIdentifierType.Ukprn,
                        Value = "parentOrgUkprn"
                    }
                },
                ChildOrganisations = Enumerable
                    .Range(1, numberOfChildren)
                    .Select(GetTestChildOrganisation)
            };

        private Organisation GetTestChildOrganisation(int number = 1)
            => new Organisation
            {
                Name = $"child org {number}",
                Identifiers = new List<OrganisationIdentifier>
                {
                    new OrganisationIdentifier
                    {
                        Type = OrganisationIdentifierType.Ukprn,
                        Value = $"childOrg{number}ukprn"
                    }
                }
            };
    }
}