using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.Core.Web.Components.Areas.Lists.DTOs;
using Pds.Core.Web.Components.Areas.Lists.Models;
using Pds.DocumentExchange.Services.Enums;
using Pds.DocumentExchange.Services.Interfaces;
using Pds.DocumentExchange.Services.Models;
using Pds.DocumentExchange.Services.Models.Filters;
using Pds.DocumentExchange.Web.DTOs;
using Pds.DocumentExchange.Web.Implementations.Coordinators;
using Pds.DocumentExchange.Web.Interfaces.Helpers;
using Pds.DocumentExchange.Web.Interfaces.Providers;
using Pds.DocumentExchange.Web.Models;
using Pds.DocumentExchange.Web.Models.Agency;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pds.DocumentExchange.Web.Tests.Unit.Coordinators
{
    [TestClass, TestCategory("Unit")]
    public sealed class AgencyFileShareListRequestCoordinatorTests
    {
        private const string FakeTeamsList = "a,b,c";

        private static readonly DocumentExchangeConfiguration Configuration
            = new DocumentExchangeConfiguration
            {
                ListPageSize = 37
            };

        private readonly IUserInformationProvider _userInfoProvider
            = Mock.Of<IUserInformationProvider>(MockBehavior.Strict);

        private readonly IAgencyApiClient _agencyApiClient
            = Mock.Of<IAgencyApiClient>(MockBehavior.Strict);

        private readonly IListHelper _listHelper
            = Mock.Of<IListHelper>(MockBehavior.Strict);

        private readonly IMapper _mapper
            = Mock.Of<IMapper>(MockBehavior.Strict);

        [TestMethod, DynamicData(nameof(GetDocuments_TestData))]
        public async Task GetDocumentsToPublish_ReturnsExpected(ListRequest request, int numberOfDocuments)
        {
            // Arrange
            AgencyListDocumentOptions actualOptions = null;

            var fakeDocuments = Enumerable
                .Range(0, numberOfDocuments)
                .Select(id => new AgencyDocument
                {
                    FileName = $"file-{id}",
                    Team = $"team-{id}",
                    Product = new Product
                    {
                        Name = $"product-{id}"
                    }
                }).ToList();

            var fakeListResult = new ListResult<AgencyDocument>
            {
                Items = fakeDocuments
            };

            var fakeFilterOptions = Enumerable.Empty<IFilterOption>().ToList();
            var fakePagination = new PaginationViewModel();
            var fakeFilterCategories = Enumerable.Empty<IFilterCategoryViewModel>().ToList();
            var fakeFileShareDocuments = Enumerable
                .Range(0, numberOfDocuments)
                .Select(id => new FileShareDocument
                {
                    DocumentReference = new FileShareDocumentReference
                    {
                        FileName = $"file-{id}",
                        Team = $"team-{id}"
                    },
                    ProductName = $"product-{id}"
                })
                .ToList();

            var fakeProduct = new Models.Shared.Product
            {
                Name = "test product name",
                Identifier = 12345
            };

            var expectedResult = new DocumentsToPublish
            {
                Pagination = fakePagination,
                ListItems = fakeFileShareDocuments,
                FilterCategories = fakeFilterCategories,
                AnyDocumentsAvailable = numberOfDocuments > 0,
                SelectedProduct = numberOfDocuments == 0 ? null : fakeProduct,
                SelectedTeam = fakeDocuments.FirstOrDefault()?.Team
            };

            Mock.Get(_userInfoProvider)
                .Setup(u => u.GetCurrentUserAgencyTeams())
                .ReturnsAsync(FakeTeamsList);

            Mock.Get(_agencyApiClient)
                .Setup(e => e.ListTeamDocuments(
                    FakeTeamsList,
                    It.IsAny<AgencyListDocumentOptions>()))
                .ReturnsAsync((string teams, AgencyListDocumentOptions options) =>
                {
                    actualOptions = options;
                    return fakeListResult;
                });

            Mock.Get(_listHelper)
                .Setup(lh => lh.GetFilterOptions(request, false))
                .Returns(fakeFilterOptions);

            Mock.Get(_listHelper)
                .Setup(lh => lh.GetPaginationViewModel(request, fakeListResult))
                .Returns(fakePagination);

            Mock.Get(_listHelper)
                .Setup(lh => lh.GetFilterCategories(fakeListResult))
                .Returns(fakeFilterCategories);

            Mock.Get(_listHelper)
                .Setup(lh => lh.AnyDocumentsAvailable(request, fakeListResult))
                .Returns(numberOfDocuments > 0);

            Mock.Get(_mapper)
                .Setup(mapper => mapper.Map<Models.Shared.Product>(It.IsAny<Product>()))
                .Returns(
                    (Product product) =>
                    {
                        if (product == null)
                        {
                            return null;
                        }

                        return fakeProduct;
                    });

            var coordinator = GetTestCoordinator();

            // Act
            var actual = await coordinator.GetDocumentsToPublish(request);

            // Assert
            actual.Should().BeEquivalentTo(expectedResult);

            actualOptions.Should().BeEquivalentTo(
                new AgencyListDocumentOptions
                {
                    Validity = AgencyDocumentValidity.Valid,
                    PageSize = Configuration.ListPageSize,
                    PageNumber = request?.Page ?? 1,
                    FilterOptions = fakeFilterOptions
                });

            Mock.VerifyAll(
                Mock.Get(_userInfoProvider),
                Mock.Get(_agencyApiClient),
                Mock.Get(_listHelper));
        }

        [TestMethod, DynamicData(nameof(GetDocuments_TestData))]
        public async Task GetDocumentsToPublishData_ReturnsExpected(ListRequest request, int numberOfDocuments)
        {
            // Arrange
            AgencyListDocumentOptions actualOptions = null;
            IListViewModel actualDocumentsToPublish = null;

            var fakeDocuments = Enumerable
                .Range(0, numberOfDocuments)
                .Select(id => new AgencyDocument
                {
                    FileName = $"file-{id}",
                    Team = $"team-{id}",
                    Product = new Product
                    {
                        Name = $"product-{id}",
                        Identifier = 10000 + id
                    }
                }).ToList();

            var fakeListResult = new ListResult<AgencyDocument>
            {
                Items = fakeDocuments
            };

            var fakeFilterOptions = Enumerable.Empty<IFilterOption>().ToList();
            var fakePagination = new PaginationViewModel
            {
                TotalItems = numberOfDocuments
            };
            var fakeFilterCategories = Enumerable.Empty<IFilterCategoryViewModel>().ToList();
            var fakeFileShareDocuments = Enumerable
                .Range(0, numberOfDocuments)
                .Select(id => new FileShareDocument
                {
                    DocumentReference = new FileShareDocumentReference
                    {
                        FileName = $"file-{id}",
                        Team = $"team-{id}"
                    },
                    ProductName = $"product-{id}"
                })
                .ToList();

            var fakeProduct = new Models.Shared.Product
            {
                Name = "test product name",
                PluralName = "test product names",
                Identifier = 12345
            };

            var expectedDocumentsToPublish = new DocumentsToPublish
            {
                Pagination = fakePagination,
                ListItems = fakeFileShareDocuments,
                FilterCategories = fakeFilterCategories,
                AnyDocumentsAvailable = numberOfDocuments > 0,
                SelectedProduct = numberOfDocuments == 0 ? null : fakeProduct,
                SelectedTeam = fakeDocuments.FirstOrDefault()?.Team
            };

            var expectedResult = numberOfDocuments > 0
                ? new DocumentListUpdateData
                {
                    PageUpdateItems = new[]
                    {
                        new PageUpdateItem("#total-documents-for-selected-product", $"{numberOfDocuments}"),
                        new PageUpdateItem(
                            "#selected-product-name",
                            numberOfDocuments == 1 ? fakeProduct.Name : fakeProduct.PluralName)
                    }
                }
                : new DocumentListUpdateData();

            Mock.Get(_userInfoProvider)
                .Setup(u => u.GetCurrentUserAgencyTeams())
                .ReturnsAsync(FakeTeamsList);

            Mock.Get(_agencyApiClient)
                .Setup(e => e.ListTeamDocuments(
                    FakeTeamsList,
                    It.IsAny<AgencyListDocumentOptions>()))
                .ReturnsAsync((string teams, AgencyListDocumentOptions options) =>
                {
                    actualOptions = options;
                    return fakeListResult;
                });

            Mock.Get(_listHelper)
                .Setup(lh => lh.GetFilterOptions(request, false))
                .Returns(fakeFilterOptions);

            Mock.Get(_listHelper)
                .Setup(lh => lh.GetPaginationViewModel(request, fakeListResult))
                .Returns(fakePagination);

            Mock.Get(_listHelper)
                .Setup(lh => lh.GetFilterCategories(fakeListResult))
                .Returns(fakeFilterCategories);

            Mock.Get(_listHelper)
                .Setup(lh => lh.AnyDocumentsAvailable(request, fakeListResult))
                .Returns(numberOfDocuments > 0);

            Mock.Get(_listHelper)
                .Setup(lh => lh.GetDocumentListUpdateData(It.IsAny<IListViewModel>()))
                .Returns((IListViewModel viewModel) =>
                {
                    actualDocumentsToPublish = viewModel;
                    return new DocumentListUpdateData();
                });

            Mock.Get(_mapper)
                .Setup(mapper => mapper.Map<Models.Shared.Product>(It.IsAny<Product>()))
                .Returns(
                    (Product product) =>
                    {
                        if (product == null)
                        {
                            return null;
                        }

                        return fakeProduct;
                    });

            var coordinator = GetTestCoordinator();

            // Act
            var actual = await coordinator.GetDocumentsToPublishData(request);

            // Assert
            actual.Should().BeEquivalentTo(expectedResult);

            actualDocumentsToPublish.Should().BeEquivalentTo(expectedDocumentsToPublish);

            actualOptions.Should().BeEquivalentTo(
                new AgencyListDocumentOptions
                {
                    Validity = AgencyDocumentValidity.Valid,
                    PageSize = Configuration.ListPageSize,
                    PageNumber = request?.Page ?? 1,
                    FilterOptions = fakeFilterOptions,
                });

            Mock.VerifyAll(
                Mock.Get(_userInfoProvider),
                Mock.Get(_agencyApiClient),
                Mock.Get(_listHelper));
        }

        [TestMethod, DynamicData(nameof(GetDocuments_TestData))]
        public async Task GetDocumentsToReview_ReturnsExpected(ListRequest request, int numberOfDocuments)
        {
            // Arrange
            AgencyListDocumentOptions actualOptions = null;

            var fakeDocuments = Enumerable
                .Range(0, numberOfDocuments)
                .Select(id => new AgencyDocument
                {
                    FileName = $"file-{id}",
                    Team = $"team-{id}",
                    Product = new Product
                    {
                        Name = $"product-{id}"
                    },
                    FileNameError = $"error-{id}"
                }).ToList();

            var fakeListResult = new ListResult<AgencyDocument>
            {
                Items = fakeDocuments
            };

            var fakeFilterOptions = Enumerable.Empty<IFilterOption>().ToList();
            var fakePagination = new PaginationViewModel();
            var fakeFilterCategories = Enumerable.Empty<IFilterCategoryViewModel>().ToList();
            var fakeFileShareDocuments = Enumerable
                .Range(0, numberOfDocuments)
                .Select(id => new InvalidFileShareDocument
                {
                    DocumentReference = new FileShareDocumentReference
                    {
                        FileName = $"file-{id}",
                        Team = $"team-{id}"
                    },
                    ProductName = $"product-{id}",
                    FileNameError = $"error-{id}"
                })
                .ToList();

            var expectedResult = new DocumentsToReview
            {
                Pagination = fakePagination,
                ListItems = fakeFileShareDocuments,
                FilterCategories = fakeFilterCategories,
                AnyDocumentsAvailable = numberOfDocuments > 0
            };

            Mock.Get(_userInfoProvider)
                .Setup(u => u.GetCurrentUserAgencyTeams())
                .ReturnsAsync(FakeTeamsList);

            Mock.Get(_agencyApiClient)
                .Setup(e => e.ListTeamDocuments(
                    FakeTeamsList,
                    It.IsAny<AgencyListDocumentOptions>()))
                .ReturnsAsync((string teams, AgencyListDocumentOptions options) =>
                {
                    actualOptions = options;
                    return fakeListResult;
                });

            Mock.Get(_listHelper)
                .Setup(lh => lh.GetFilterOptions(request, false))
                .Returns(fakeFilterOptions);

            Mock.Get(_listHelper)
                .Setup(lh => lh.GetPaginationViewModel(request, fakeListResult))
                .Returns(fakePagination);

            Mock.Get(_listHelper)
                .Setup(lh => lh.GetFilterCategories(fakeListResult))
                .Returns(fakeFilterCategories);

            Mock.Get(_listHelper)
                .Setup(lh => lh.AnyDocumentsAvailable(request, fakeListResult))
                .Returns(numberOfDocuments > 0);

            var coordinator = GetTestCoordinator();

            // Act
            var actual = await coordinator.GetDocumentsToReview(request);

            // Assert
            actual.Should().BeEquivalentTo(expectedResult);

            actualOptions.Should().BeEquivalentTo(
                new AgencyListDocumentOptions
                {
                    Validity = AgencyDocumentValidity.Invalid,
                    PageSize = Configuration.ListPageSize,
                    PageNumber = request?.Page ?? 1,
                    FilterOptions = fakeFilterOptions
                });

            Mock.VerifyAll(
                Mock.Get(_userInfoProvider),
                Mock.Get(_agencyApiClient),
                Mock.Get(_listHelper));
        }

        [TestMethod, DynamicData(nameof(GetDocuments_TestData))]
        public async Task GetDocumentsToReview_ForNullProduct_ReturnsExpected(ListRequest request, int numberOfDocuments)
        {
            // Arrange
            AgencyListDocumentOptions actualOptions = null;

            var fakeDocuments = Enumerable
                .Range(0, numberOfDocuments)
                .Select(id => new AgencyDocument
                {
                    FileName = $"file-{id}",
                    Team = $"team-{id}",
                    FileNameError = $"error-{id}"
                }).ToList();

            var fakeListResult = new ListResult<AgencyDocument>
            {
                Items = fakeDocuments
            };

            var fakeFilterOptions = Enumerable.Empty<IFilterOption>().ToList();
            var fakePagination = new PaginationViewModel();
            var fakeFilterCategories = Enumerable.Empty<IFilterCategoryViewModel>().ToList();
            var fakeFileShareDocuments = Enumerable
                .Range(0, numberOfDocuments)
                .Select(id => new InvalidFileShareDocument
                {
                    DocumentReference = new FileShareDocumentReference
                    {
                        FileName = $"file-{id}",
                        Team = $"team-{id}"
                    },
                    FileNameError = $"error-{id}"
                })
                .ToList();

            var expectedResult = new DocumentsToReview
            {
                Pagination = fakePagination,
                ListItems = fakeFileShareDocuments,
                FilterCategories = fakeFilterCategories,
                AnyDocumentsAvailable = numberOfDocuments > 0
            };

            Mock.Get(_userInfoProvider)
                .Setup(u => u.GetCurrentUserAgencyTeams())
                .ReturnsAsync(FakeTeamsList);

            Mock.Get(_agencyApiClient)
                .Setup(e => e.ListTeamDocuments(
                    FakeTeamsList,
                    It.IsAny<AgencyListDocumentOptions>()))
                .ReturnsAsync((string teams, AgencyListDocumentOptions options) =>
                {
                    actualOptions = options;
                    return fakeListResult;
                });

            Mock.Get(_listHelper)
                .Setup(lh => lh.GetFilterOptions(request, false))
                .Returns(fakeFilterOptions);

            Mock.Get(_listHelper)
                .Setup(lh => lh.GetPaginationViewModel(request, fakeListResult))
                .Returns(fakePagination);

            Mock.Get(_listHelper)
                .Setup(lh => lh.GetFilterCategories(fakeListResult))
                .Returns(fakeFilterCategories);

            Mock.Get(_listHelper)
                .Setup(lh => lh.AnyDocumentsAvailable(request, fakeListResult))
                .Returns(numberOfDocuments > 0);

            var coordinator = GetTestCoordinator();

            // Act
            var actual = await coordinator.GetDocumentsToReview(request);

            // Assert
            actual.Should().BeEquivalentTo(expectedResult);

            actualOptions.Should().BeEquivalentTo(
                new AgencyListDocumentOptions
                {
                    Validity = AgencyDocumentValidity.Invalid,
                    PageSize = Configuration.ListPageSize,
                    PageNumber = request?.Page ?? 1,
                    FilterOptions = fakeFilterOptions
                });

            Mock.VerifyAll(
                Mock.Get(_userInfoProvider),
                Mock.Get(_agencyApiClient),
                Mock.Get(_listHelper));
        }

        [TestMethod, DynamicData(nameof(GetDocuments_TestData))]
        public async Task GetDocumentsToReviewData_ReturnsExpected(ListRequest request, int numberOfDocuments)
        {
            // Arrange
            AgencyListDocumentOptions actualOptions = null;
            IListViewModel actualDocumentsToReview = null;

            var fakeDocuments = Enumerable
                .Range(0, numberOfDocuments)
                .Select(id => new AgencyDocument
                {
                    FileName = $"file-{id}",
                    Team = $"team-{id}",
                    Product = new Product
                    {
                        Name = $"product-{id}"
                    },
                    FileNameError = $"error-{id}"
                }).ToList();

            var fakeListResult = new ListResult<AgencyDocument>
            {
                Items = fakeDocuments
            };

            var fakeFilterOptions = Enumerable.Empty<IFilterOption>().ToList();
            var fakePagination = new PaginationViewModel();
            var fakeFilterCategories = Enumerable.Empty<IFilterCategoryViewModel>().ToList();
            var fakeFileShareDocuments = Enumerable
                .Range(0, numberOfDocuments)
                .Select(id => new InvalidFileShareDocument
                {
                    DocumentReference = new FileShareDocumentReference
                    {
                        FileName = $"file-{id}",
                        Team = $"team-{id}"
                    },
                    ProductName = $"product-{id}",
                    FileNameError = $"error-{id}"
                })
                .ToList();

            var expectedResult = new DocumentsToReview
            {
                Pagination = fakePagination,
                ListItems = fakeFileShareDocuments,
                FilterCategories = fakeFilterCategories,
                AnyDocumentsAvailable = numberOfDocuments > 0
            };

            var fakeResult = new DocumentListUpdateData();

            Mock.Get(_userInfoProvider)
                .Setup(u => u.GetCurrentUserAgencyTeams())
                .ReturnsAsync(FakeTeamsList);

            Mock.Get(_agencyApiClient)
                .Setup(e => e.ListTeamDocuments(
                    FakeTeamsList,
                    It.IsAny<AgencyListDocumentOptions>()))
                .ReturnsAsync((string teams, AgencyListDocumentOptions options) =>
                {
                    actualOptions = options;
                    return fakeListResult;
                });

            Mock.Get(_listHelper)
                .Setup(lh => lh.GetFilterOptions(request, false))
                .Returns(fakeFilterOptions);

            Mock.Get(_listHelper)
                .Setup(lh => lh.GetPaginationViewModel(request, fakeListResult))
                .Returns(fakePagination);

            Mock.Get(_listHelper)
                .Setup(lh => lh.GetFilterCategories(fakeListResult))
                .Returns(fakeFilterCategories);

            Mock.Get(_listHelper)
                .Setup(lh => lh.AnyDocumentsAvailable(request, fakeListResult))
                .Returns(numberOfDocuments > 0);

            Mock.Get(_listHelper)
                .Setup(lh => lh.GetDocumentListUpdateData(It.IsAny<IListViewModel>()))
                .Returns((IListViewModel viewModel) =>
                {
                    actualDocumentsToReview = viewModel;
                    return fakeResult;
                });

            var coordinator = GetTestCoordinator();

            // Act
            var actual = await coordinator.GetDocumentsToReviewData(request);

            // Assert
            actual.Should().Be(fakeResult);

            actualDocumentsToReview.Should().BeEquivalentTo(expectedResult);

            actualOptions.Should().BeEquivalentTo(
                new AgencyListDocumentOptions
                {
                    Validity = AgencyDocumentValidity.Invalid,
                    PageSize = Configuration.ListPageSize,
                    PageNumber = request?.Page ?? 1,
                    FilterOptions = fakeFilterOptions
                });

            Mock.VerifyAll(
                Mock.Get(_userInfoProvider),
                Mock.Get(_agencyApiClient),
                Mock.Get(_listHelper));
        }

        private static IEnumerable<object[]> GetDocuments_TestData
        {
            get
            {
                yield return new object[]
                {
                    null,
                    0
                };
                yield return new object[]
                {
                    null,
                    1
                };
                yield return new object[]
                {
                    null,
                    2
                };
                yield return new object[]
                {
                    new ListRequest
                    {
                        Page = 123
                    },
                    37
                };
            }
        }

        private AgencyFileShareListRequestCoordinator GetTestCoordinator()
        {
            return new AgencyFileShareListRequestCoordinator(
                _userInfoProvider,
                _agencyApiClient,
                _listHelper,
                Options.Create(Configuration),
                _mapper);
        }
    }
}