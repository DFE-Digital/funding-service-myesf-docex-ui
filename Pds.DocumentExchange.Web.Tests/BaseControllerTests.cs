using Moq;
using Pds.Core.Common.Identity.Enums;
using Pds.Core.Common.Identity.Models;
using Pds.Core.Common.Organisation.Enums;
using Pds.Core.Common.Organisation.Models;
using Pds.Core.Web.Components.Areas.Lists.DTOs;
using Pds.Core.Web.Components.Areas.Lists.Models;
using Pds.DocumentExchange.Services.Enums;
using Pds.DocumentExchange.Services.Interfaces;
using Pds.DocumentExchange.Services.Models;
using Pds.DocumentExchange.Services.Models.Filters;
using Pds.DocumentExchange.Web.Models;
using Pds.DocumentExchange.Web.Models.Agency;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pds.DocumentExchange.Web.Tests
{
    public abstract class BaseControllerTests
    {
        protected const string FakeString = "Fake";

        protected const int MaxNumberOfPages = 5;

        protected DocumentExchangeConfiguration TestConfiguration
            => new DocumentExchangeConfiguration
            {
                ListPageSize = 13
            };

        protected IExchangeApiClient ExchangeApiClient { get; } = Mock.Of<IExchangeApiClient>(MockBehavior.Strict);

        protected User TestAgencyUser
            => new User
            {
                IsAuthenticated = true,
                IsExternalUser = false,
                Ukprn = null,
                Roles = new List<string>
                {
                    UserRole.DocumentExchangeAdministratorFundingCentre.ToString()
                },
                Email = "agencyuser@fake.com",
                ProviderName = null,
                FirstName = "agency test",
                LastName = "user",
                FullName = "agency test user",
                Principal = "agencytestuser",
                CanViewAsOrganisation = true
            };

        protected User TestOrganisationUser
         => new User
         {
             IsAuthenticated = true,
             IsExternalUser = true,
             Ukprn = 12345678,
             Roles = new List<string>
             {
                    UserRole.DocumentExchangeUser.ToString()
             },
             Email = "organisationuser@fake.com",
             ProviderName = null,
             FirstName = "organisationuser test",
             LastName = "user",
             FullName = "organisationuser test user",
             Principal = "organisationusertestuser",
             CanViewAsOrganisation = false
         };

        protected User TestNoOrganisationUser
             => new User
             {
                 IsAuthenticated = true,
                 IsExternalUser = true,
                 Ukprn = null,
                 Roles = new List<string>
                 {
                        UserRole.DocumentExchangeUser.ToString()
                 },
                 Email = "noorganisationuser@fake.com",
                 ProviderName = null,
                 FirstName = "no organisationuser test",
                 LastName = "user",
                 FullName = "no organisationuser test user",
                 Principal = "no organisationusertestuser",
                 CanViewAsOrganisation = false
             };

        protected User TestAgencyAdvancedUser
            => new User
            {
                IsAuthenticated = true,
                IsExternalUser = false,
                Ukprn = null,
                Roles = new List<string>
                {
                    UserRole.DocumentExchangeAdvancedUser.ToString()
                },
                Email = "agencyadvanceduser@fake.com",
                ProviderName = null,
                FirstName = "agency advanced test",
                LastName = "user",
                FullName = "agency advanced test user",
                Principal = "agencyadvancedtestuser",
                CanViewAsOrganisation = true
            };

        protected User TestAgencyAdvancedUser2
            => new User
            {
                IsAuthenticated = true,
                IsExternalUser = false,
                Ukprn = null,
                Roles = new List<string>
                {
                            UserRole.DocumentExchangeAdministratorFundingCentre.ToString(),
                            UserRole.DocumentExchangeAdministratorRiskAssurance.ToString(),
                            UserRole.DocumentExchangeAdvancedUser.ToString()
                },
                Email = "agencyadvanceduser@fake.com",
                ProviderName = null,
                FirstName = "agency advanced test",
                LastName = "user",
                FullName = "agency advanced test user",
                Principal = "agencyadvancedtestuser",
                CanViewAsOrganisation = true
            };

        protected UserInfo TestAgencyUserInfo
            => new UserInfo
            {
                FullName = "agency test user",
                Principal = "agencytestuser",
                EmailAddress = "agencyuser@fake.com",
                OrganisationInfo = new OrganisationInfo
                {
                    OrganisationIdentifier = new OrganisationIdentifier
                    {
                        Type = OrganisationIdentifierType.Ukprn,
                        Value = null
                    }
                }
            };

        protected UserInfo TestAgencyAdvancedUserInfo
            => new UserInfo
            {
                FullName = "agency advanced test user",
                Principal = "agencyadvancedtestuser",
                EmailAddress = "agencyadvanceduser@fake.com",
                OrganisationInfo = new OrganisationInfo
                {
                    OrganisationIdentifier = new OrganisationIdentifier
                    {
                        Type = OrganisationIdentifierType.Ukprn,
                        Value = null
                    }
                }
            };

        protected UserInfo TestNoOrganisationUserInfo
            => new UserInfo
            {
                FullName = "no org user",
                Principal = "no-org-user",
                EmailAddress = "noorguser@fake.com"
            };

        protected IEnumerable<InvalidFileShareDocument> GetTestInvalidFileShareDocuments(
            int numberOfPages,
            int pageNumber = 1)
        {
            if (numberOfPages == 0)
            {
                return Enumerable.Empty<InvalidFileShareDocument>();
            }

            return Enumerable
                .Range(1, numberOfPages)
                .Select(p => Enumerable
                    .Range(((p - 1) * TestConfiguration.ListPageSize) + 1, TestConfiguration.ListPageSize)
                    .Select(d => new InvalidFileShareDocument
                    {
                        DocumentReference = new FileShareDocumentReference
                        {
                            FileName = $"file{d}",
                        },
                        FileNameError = FakeString,
                        ProductName = $"product{p}",
                        Selected = false
                    })).ElementAt(pageNumber - 1);
        }

        protected IEnumerable<FileShareDocument> GetTestFileShareDocuments(
            int numberOfPages,
            int pageNumber = 1)
        {
            if (numberOfPages == 0)
            {
                return Enumerable.Empty<FileShareDocument>();
            }

            return Enumerable
                .Range(1, numberOfPages)
                .Select(p => Enumerable
                    .Range(((p - 1) * TestConfiguration.ListPageSize) + 1, TestConfiguration.ListPageSize)
                    .Select(d => new FileShareDocument
                    {
                        DocumentReference = new FileShareDocumentReference
                        {
                            FileName = $"file{d}",
                        },
                        ProductName = $"product{p}",
                        Selected = false
                    })).ElementAt(pageNumber - 1);
        }

        protected IEnumerable<AgencyExchangeDocument> GetTestDownloadFileShareDocuments(
            int numberOfPages,
            int pageNumber = 1)
        {
            if (numberOfPages == 0)
            {
                return Enumerable.Empty<AgencyExchangeDocument>();
            }

            return Enumerable
                .Range(1, numberOfPages)
                .Select(p => Enumerable
                    .Range(((p - 1) * TestConfiguration.ListPageSize) + 1, TestConfiguration.ListPageSize)
                    .Select(d => new AgencyExchangeDocument
                    {
                        FileName = $"file{d}",
                        ProductName = $"product{p}",
                        Selected = false,
                        Status = d % 2 == 0 ? DownloadDocumentStatus.New : DownloadDocumentStatus.Downloaded,
                        StatusDateTime = d % 2 == 0 ? new DateTime(2020, 6, 1) : new DateTime(2020, 7, 1),
                        DownloadedBy = d % 2 == 0 ? string.Empty : "Mr. Martin Riggs",
                        DisplayStatusDateTime = "my date"
                    })).ElementAt(pageNumber - 1);
        }

        protected IEnumerable<AgencyExchangeDocument> AddVersionsAndPublishedByToTestDownloadFileshareDocuments(List<AgencyExchangeDocument> agencyExchangeDocuments, string publishedBy = null)
        {
            var amendedAgencyExchangeDocuments = agencyExchangeDocuments;
            foreach (AgencyExchangeDocument document in amendedAgencyExchangeDocuments)
            {
                document.PublishedBy = publishedBy;
                document.VersionNumber = amendedAgencyExchangeDocuments.IndexOf(document) + 1;

                for (int i = document.VersionNumber; i > 0; i--)
                {
                    if (document.VersionNumber > 1)
                    {
                        document.VersionsWithoutHyperlinks = string.Join(i.ToString(), ", ");
                        document.Versions = string.Join(i.ToString(), ", ");
                    }
                    else
                    {
                        document.VersionsWithoutHyperlinks = i.ToString();
                        document.Versions = i.ToString();
                    }
                }
            }

            return amendedAgencyExchangeDocuments;
        }

        protected AgencyExchangeDocument GetTestDocumentsWithPreviousVersions(int numberOfPreviousVersions)
        {
            AgencyExchangeDocument testDocument = GetTestDocument(numberOfPreviousVersions + 1);

            List<AgencyExchangeDocument> previousVersions = new List<AgencyExchangeDocument>();

            for (int i = 1; i <= numberOfPreviousVersions; i++)
            {
                previousVersions.Add(GetTestDocument(i));
            }

            testDocument.PreviousVersions = previousVersions;

            return testDocument;
        }

        protected AgencyExchangeDocument GetTestDocument(int counter)
        {
            return new AgencyExchangeDocument()
            {
                FileName = $"document{counter}",
                ProductName = $"product1",
                Version = $"downloadUrl{counter}",
                VersionNumber = counter,
                ProviderName = "provider",
                ProviderUkprn = "ukprn"
            };
        }

        protected ListRequest PublishedByAgencyListRequest()
        {
            return new ListRequest()
            {
                Page = 1,
                FilterRequest = new FilterRequest()
                {
                    Filters = new IFilterCategory[2]
                    {
                        new RadioFilterCategory()
                        {
                            Key = "Team",
                            Value = "DocumentExchangeAdministratorFundingCentre"
                        },
                        new RadioFilterCategory()
                        {
                            Key = "Direction",
                            Value = "PublishedByAgency"
                        }
                    }
                }
            };
        }

        protected ListRequest SentByOrganisationListRequest()
        {
            return new ListRequest()
            {
                Page = 1,
                FilterRequest = new FilterRequest()
                {
                    Filters = new IFilterCategory[2]
                    {
                        new RadioFilterCategory()
                        {
                            Key = "Team",
                            Value = "DocumentExchangeAdministratorFundingCentre"
                        },
                        new RadioFilterCategory()
                        {
                            Key = "Direction",
                            Value = "SentByOrganisation"
                        }
                    }
                }
            };
        }

        protected IEnumerable<IFilterCategoryViewModel> GetTestViewModelFilterCategories(int numberOfFilters)
        {
            if (numberOfFilters == 0)
            {
                return Enumerable.Empty<IFilterCategoryViewModel>();
            }

            return Enumerable
                .Range(1, numberOfFilters)
                .Select<int, IFilterCategoryViewModel>(f =>
                {
                    if (f % 2 == 0)
                    {
                        return CreateListFilterCategoryViewModel(f);
                    }
                    else if (f % 3 == 0)
                    {
                        return CreateDateRangeFilterViewModel(f);
                    }
                    else if (f % 5 == 0)
                    {
                        return CreateRadioFilterViewModel(f);
                    }
                    else
                    {
                        return CreateTextBoxFilterViewModel(f);
                    }
                });
        }

        protected ListFilterCategoryViewModel CreateListFilterCategoryViewModel(int number)
            => new ListFilterCategoryViewModel
            {
                Key = $"listFilterKey{number}",
                Title = $"Filter {number}",
                Values = new List<FilterValueViewModel>
                        {
                            new FilterValueViewModel
                            {
                                Title = "Value 1",
                                Selected = false,
                                Value = "value1",
                                Count = 123
                            },
                            new FilterValueViewModel
                            {
                                Title = "Value 2",
                                Selected = false,
                                Value = "value2",
                                Count = 321
                            }
                        }
            };

        protected DateRangeFilterViewModel CreateDateRangeFilterViewModel(int number)
            => new DateRangeFilterViewModel
            {
                Key = $"dateFilterKey{number}",
                Title = $"Filter {number}",
                FromDay = 1,
                FromMonth = 11,
                FromYear = 2020,
                ToDay = 1,
                ToMonth = 12,
                ToYear = 2020,
            };

        protected RadioFilterViewModel CreateRadioFilterViewModel(int number)
           => new RadioFilterViewModel
           {
               Key = $"radioFilterKey{number}",
               Title = $"Filter {number}",
               Values = new List<RadioFilterValueViewModel>
                       {
                                    new RadioFilterValueViewModel
                                    {
                                        Title = "Value 1",
                                        Selected = false,
                                        Value = "value1"
                                    },
                                    new RadioFilterValueViewModel
                                    {
                                        Title = "Value 2",
                                        Selected = false,
                                        Value = "value2"
                                    }
                       }
           };

        protected TextBoxFilterViewModel CreateTextBoxFilterViewModel(int number)
           => new TextBoxFilterViewModel
           {
               Key = $"textBoxFilterKey{number}",
               Title = $"Filter {number}",
               Value = $"value-{number}",
               Hint = $"Hint{number}",
               ValidationErrorMessage = $"Error-message{number}",
               Regex = "[a-z]"
           };

        protected IEnumerable<IFilter> GetTestServiceFilters(int numberOfFilters)
        {
            if (numberOfFilters == 0)
            {
                return Enumerable.Empty<IFilter>();
            }

            return Enumerable
                .Range(1, numberOfFilters)
                .Select<int, IFilter>(f =>
                {
                    if (f % 2 == 0)
                    {
                        return CreateListFilter(f);
                    }
                    else if (f % 3 == 0)
                    {
                        return CreateDateRangeFilter(f);
                    }
                    else if (f % 5 == 0)
                    {
                        return CreateRadioFilter(f);
                    }
                    else
                    {
                        return CreateTextBoxFilter(f);
                    }
                });
        }

        protected ListFilter CreateListFilter(int number)
            => new ListFilter
            {
                Key = $"listFilterKey{number}",
                Title = $"Filter {number}",
                Values = new List<FilterValue>
                        {
                                    new FilterValue
                                    {
                                        Title = "Value 1",
                                        Selected = false,
                                        Value = "value1",
                                        Count = 123
                                    },
                                    new FilterValue
                                    {
                                        Title = "Value 2",
                                        Selected = false,
                                        Value = "value2",
                                        Count = 321
                                    }
                        }
            };

        protected DateRangeFilter CreateDateRangeFilter(int number)
           => new DateRangeFilter
           {
               Key = $"dateFilterKey{number}",
               Title = $"Filter {number}",
               FromTitle = "from date",
               ToTitle = "to date",
               From = new DateTime(2020, 11, 1),
               To = new DateTime(2020, 12, 1)
           };

        protected RadioFilter CreateRadioFilter(int number)
            => new RadioFilter
            {
                Key = $"radioFilterKey{number}",
                Title = $"Filter {number}",
                Values = new List<RadioFilterValue>
                        {
                                    new RadioFilterValue
                                    {
                                        Title = "Value 1",
                                        Selected = false,
                                        Value = "value1"
                                    },
                                    new RadioFilterValue
                                    {
                                        Title = "Value 2",
                                        Selected = false,
                                        Value = "value2"
                                    }
                        }
            };

        protected TextBoxFilter CreateTextBoxFilter(int number)
           => new TextBoxFilter
           {
               Key = $"textBoxFilterKey{number}",
               Title = $"Filter {number}",
               Value = $"value-{number}",
               Hint = $"Hint{number}",
               ValidationErrorMessage = $"Error-message{number}",
               Regex = "[a-z]"
           };

        protected PaginationUpdateData GetExpectedPaginationUpdateData(int numberOfItems, int numberOfPages)
        {
            if (numberOfPages == 0)
            {
                return new PaginationUpdateData
                {
                    TotalItems = numberOfItems,
                    TotalPages = numberOfPages,
                    FirstItemNumberOnSelectedPage = 0,
                    LastItemNumberOnSelectedPage = 0,
                    FirstPageNumberControl = 1,
                    LastPageNumberControl = 0
                };
            }

            return new PaginationUpdateData
            {
                TotalItems = numberOfItems,
                TotalPages = numberOfPages,
                FirstItemNumberOnSelectedPage = 1,
                LastItemNumberOnSelectedPage = TestConfiguration.ListPageSize,
                FirstPageNumberControl = 1,
                LastPageNumberControl = Math.Min(numberOfPages, MaxNumberOfPages)
            };
        }
    }
}