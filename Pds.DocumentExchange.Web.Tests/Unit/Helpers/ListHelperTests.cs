using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.Core.Utils;
using Pds.Core.Utils.Interfaces;
using Pds.Core.Web.Components.Areas.Lists.Builders;
using Pds.Core.Web.Components.Areas.Lists.DTOs;
using Pds.Core.Web.Components.Areas.Lists.Models;
using Pds.DocumentExchange.Services.Enums;
using Pds.DocumentExchange.Services.Models;
using Pds.DocumentExchange.Services.Models.Filters;
using Pds.DocumentExchange.Web.DTOs;
using Pds.DocumentExchange.Web.Enums;
using Pds.DocumentExchange.Web.Implementations.Helpers;
using Pds.DocumentExchange.Web.Interfaces.Helpers;
using Pds.DocumentExchange.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pds.DocumentExchange.Web.Tests.Unit.Helpers
{
    [TestClass, TestCategory("Unit")]
    public sealed class ListHelperTests
    {
        private static readonly DocumentExchangeConfiguration _configuration
            = new DocumentExchangeConfiguration
            {
                ListPageSize = 1337
            };

        private readonly IRouteValueDictionaryBuilder _routeValueDictBuilder
            = Mock.Of<IRouteValueDictionaryBuilder>(MockBehavior.Strict);

        private readonly IMapper _mapper
            = Mock.Of<IMapper>(MockBehavior.Strict);

        private readonly IDateTimeProvider _dateTimeProvider
            = Mock.Of<IDateTimeProvider>(MockBehavior.Strict);

        private readonly ISystemProvider _systemProvider
            = Mock.Of<ISystemProvider>(MockBehavior.Strict);

        [TestMethod]
        public void GetDocumentListUpdateData_ReturnsExpected()
        {
            // Arrange
            var listItems = Enumerable.Range(0, 8765).Select(id => new Mock<BaseListItem>().Object).ToList();

            var pagination = new PaginationViewModel
            {
                TotalItems = 8765,
                TotalPages = 1234
            };

            var viewModel = Mock.Of<IListViewModel>(MockBehavior.Strict);

            Mock.Get(viewModel)
                .SetupGet(vm => vm.ListItems)
                .Returns(listItems);

            Mock.Get(viewModel)
                .SetupGet(vm => vm.Pagination)
                .Returns(pagination);

            var helper = GetTestHelper();

            // Act
            var actual = helper.GetDocumentListUpdateData(viewModel);

            // Assert
            actual.Should().BeEquivalentTo(new DocumentListUpdateData
            {
                ListItems = listItems,
                Pagination = new PaginationUpdateData
                {
                    TotalItems = pagination.TotalItems,
                    TotalPages = pagination.TotalPages,
                    FirstItemNumberOnSelectedPage = pagination.FirstItemNumberOnSelectedPage,
                    LastItemNumberOnSelectedPage = pagination.LastItemNumberOnSelectedPage,
                    FirstPageNumberControl = pagination.FirstPageNumberControl,
                    LastPageNumberControl = pagination.LastPageNumberControl
                }
            });

            Mock.VerifyAll(Mock.Get(viewModel));
        }

        [TestMethod, DynamicData(nameof(AnyDocumentsAvailable_TestData))]
        public void AnyDocumentsAvailable_ReturnsExpected(ListRequest request, ListResult<object> listResult, bool expected)
        {
            // Arrange
            var helper = GetTestHelper();

            // Act
            var actual = helper.AnyDocumentsAvailable(request, listResult);

            // Assert
            actual.Should().Be(expected);
        }

        [TestMethod, DynamicData(nameof(GetFilterOptions_TestData))]
        public void GetFilterOptions_ReturnsExpected(ListRequest request, bool uses6MonthLimit, IEnumerable<IFilterOption> expected)
        {
            // Arrange
            var helper = GetTestHelper();

            // Act
            var actual = helper.GetFilterOptions(request, uses6MonthLimit);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [TestMethod, DynamicData(nameof(GetPaginationViewModel_TestData))]
        public void GetPaginationViewModel_ReturnsExpected(ListRequest request, ListResult<object> listResult, PaginationViewModel expected)
        {
            // Arrange
            FilterRequest actualFilterRequest = null;

            Mock.Get(_routeValueDictBuilder)
                .Setup(b => b.BuildListPageRouteValues(37, It.IsAny<FilterRequest>(), null))
                .Returns((int pageNumber, FilterRequest filterRequest, IDictionary<string, object> additionalRouteValues) =>
                {
                    actualFilterRequest = filterRequest;
                    return Mock.Of<IDictionary<string, object>>();
                });

            var helper = GetTestHelper();

            // Act
            var actual = helper.GetPaginationViewModel(request, listResult);

            // Assert
            actual.Should().BeEquivalentTo(expected, options => options.Excluding(vm => vm.BuildPageLinkRouteValues));

            actual.BuildPageLinkRouteValues(37);
            actualFilterRequest.Should().BeEquivalentTo(request?.FilterRequest);
        }

        [TestMethod]
        public void GetFilterCategories_ReturnsMappedFilters()
        {
            // Arrange
            var fakeFilters = Enumerable.Range(0, 37).Select(id => Mock.Of<IFilter>()).ToList();

            var fakeFilterCategories = Enumerable.Range(0, 37).Select(id => Mock.Of<IFilterCategoryViewModel>()).ToList();

            var fakeListResult = new ListResult<object>
            {
                Filters = fakeFilters
            };

            Mock.Get(_mapper)
                .Setup(m => m.ToFilterCategoryView(It.IsAny<IFilter>()))
                .Returns((IFilter filter) =>
                {
                    var index = fakeFilters.IndexOf(filter);
                    return fakeFilterCategories.ElementAt(index);
                });

            var helper = GetTestHelper();

            // Act
            var actual = helper.GetFilterCategories(fakeListResult);

            // Assert
            actual.Should().BeEquivalentTo(fakeFilterCategories);

            Mock.VerifyAll(Mock.Get(_mapper));
        }

        private static IEnumerable<object[]> AnyDocumentsAvailable_TestData
        {
            get
            {
                yield return new object[]
                {
                    null, null, false
                };
                yield return new object[]
                {
                    new ListRequest(), new ListResult<object>(), false
                };
                yield return new object[]
                {
                    new ListRequest(), new ListResult<object> { Items = Enumerable.Empty<object>() }, false
                };
                yield return new object[]
                {
                    new ListRequest(), new ListResult<object> { TotalItems = 1 }, true
                };
                yield return new object[]
                {
                    new ListRequest(), new ListResult<object> { Items = new[] { new object() } }, true
                };
                yield return new object[]
                {
                    new ListRequest { FilterRequest = new FilterRequest() }, null, false
                };
                yield return new object[]
                {
                    new ListRequest { FilterRequest = new FilterRequest { Filters = new IFilterCategory[0] } }, null, false
                };
                yield return new object[]
                {
                    new ListRequest { FilterRequest = new FilterRequest { Filters = new[] { Mock.Of<IFilterCategory>() } } }, null, false
                };
                yield return new object[]
                {
                    new ListRequest(), new ListResult<object> { Filters = new[] { Mock.Of<IFilter>() } }, true
                };
            }
        }

        private static IEnumerable<object[]> GetFilterOptions_TestData
        {
            get
            {
                yield return new object[]
                {
                    null, false, null
                };
                yield return new object[]
                {
                    null, true, null
                };
                yield return new object[]
                {
                    new ListRequest { FilterRequest = null }, false, null
                };
                yield return new object[]
                {
                    new ListRequest { FilterRequest = null }, true, null
                };
                yield return new object[]
                {
                    new ListRequest { FilterRequest = new FilterRequest { Filters = null } },
                    false,
                    null
                };
                yield return new object[]
                {
                    new ListRequest { FilterRequest = new FilterRequest { Filters = null } },
                    true,
                    null
                };
                yield return new object[]
                {
                    new ListRequest
                    {
                        FilterRequest = new FilterRequest
                        {
                            Filters = new IFilterCategory[0]
                        }
                    },
                    false,
                    Enumerable.Empty<IFilterOption>()
                };
                yield return new object[]
                {
                    new ListRequest
                    {
                        FilterRequest = new FilterRequest
                        {
                            Filters = new IFilterCategory[0]
                        }
                    },
                    true,
                    new IFilterOption[]
                    {
                        new DateRangeFilterOption
                        {
                            Key = FilterKey.UploadDate.ToString(),
                            Type = FilterOptionType.DateRangeFilterOption.ToString(),
                            From = DateTime.UtcNow.AddMonths(-6),
                            To = DateTime.UtcNow
                        }
                    }
                };
                yield return new object[]
                {
                    new ListRequest
                    {
                        FilterRequest = new FilterRequest
                        {
                            Filters = new IFilterCategory[]
                            {
                                Mock.Of<IFilterCategory>()
                            }
                        }
                    },
                    false,
                    new[] { (IFilterOption)null }
                };
                yield return new object[]
                {
                    new ListRequest
                    {
                        FilterRequest = new FilterRequest
                        {
                            Filters = new IFilterCategory[]
                            {
                                Mock.Of<IFilterCategory>()
                            }
                        }
                    },
                    true,
                    new[] { (IFilterOption)null }
                };
                yield return new object[]
                {
                    new ListRequest
                    {
                        FilterRequest = new FilterRequest
                        {
                            Filters = new IFilterCategory[]
                            {
                                new ListFilterCategory
                                {
                                    Key = "list-key",
                                    Values = new[] { "1", "2" }
                                }
                            }
                        }
                    },
                    false,
                    new IFilterOption[]
                    {
                        new ListFilterOption
                        {
                            Key = "list-key",
                            Type = FilterOptionType.ListFilterOption.ToString(),
                            Values = new[] { "1", "2" }
                        }
                    }
                };
                yield return new object[]
                {
                    new ListRequest
                    {
                        FilterRequest = new FilterRequest
                        {
                            Filters = new IFilterCategory[]
                            {
                                new ListFilterCategory
                                {
                                    Key = "list-key",
                                    Values = new[] { "1", "2" }
                                }
                            }
                        }
                    },
                    true,
                    new IFilterOption[]
                    {
                        new ListFilterOption
                        {
                            Key = "list-key",
                            Type = FilterOptionType.ListFilterOption.ToString(),
                            Values = new[] { "1", "2" }
                        }
                    }
                };
                yield return new object[]
                {
                    new ListRequest
                    {
                        FilterRequest = new FilterRequest
                        {
                            Filters = new IFilterCategory[]
                            {
                                new DateRangeFilterCategory
                                {
                                    Key = "date-key",
                                    From = new DateTime(2020, 12, 25),
                                    To = new DateTime(2021, 1, 1)
                                }
                            }
                        }
                    },
                    false,
                    new IFilterOption[]
                    {
                        new DateRangeFilterOption
                        {
                            Key = "date-key",
                            Type = FilterOptionType.DateRangeFilterOption.ToString(),
                            From = new DateTime(2020, 12, 25),
                            To = new DateTime(2021, 1, 1)
                        }
                    }
                };
                yield return new object[]
                {
                    new ListRequest
                    {
                        FilterRequest = new FilterRequest
                        {
                            Filters = new IFilterCategory[]
                            {
                                new DateRangeFilterCategory
                                {
                                    Key = "date-key",
                                    From = new DateTime(2020, 12, 25),
                                    To = new DateTime(2021, 1, 1)
                                }
                            }
                        }
                    },
                    true,
                    new IFilterOption[]
                    {
                        new DateRangeFilterOption
                        {
                            Key = "date-key",
                            Type = FilterOptionType.DateRangeFilterOption.ToString(),
                            From = new DateTime(2020, 12, 25),
                            To = new DateTime(2021, 1, 1)
                        }
                    }
                };
                yield return new object[]
                {
                    new ListRequest
                    {
                        FilterRequest = new FilterRequest
                        {
                            Filters = new IFilterCategory[]
                            {
                                new RadioFilterCategory
                                {
                                    Key = "radio-key",
                                    Value = "radio-val"
                                }
                            }
                        }
                    },
                    false,
                    new IFilterOption[]
                    {
                        new RadioFilterOption
                        {
                            Key = "radio-key",
                            Type = FilterOptionType.RadioFilterOption.ToString(),
                            Value = "radio-val"
                        }
                    }
                };
                yield return new object[]
                {
                    new ListRequest
                    {
                        FilterRequest = new FilterRequest
                        {
                            Filters = new IFilterCategory[]
                            {
                                new RadioFilterCategory
                                {
                                    Key = "radio-key",
                                    Value = "radio-val"
                                }
                            }
                        }
                    },
                    true,
                    new IFilterOption[]
                    {
                        new RadioFilterOption
                        {
                            Key = "radio-key",
                            Type = FilterOptionType.RadioFilterOption.ToString(),
                            Value = "radio-val"
                        }
                    }
                };
                yield return new object[]
                {
                    new ListRequest
                    {
                        FilterRequest = new FilterRequest
                        {
                            Filters = new IFilterCategory[]
                            {
                                new TextBoxFilterCategory
                                {
                                    Key = "txt-key",
                                    Value = "txt-val"
                                }
                            }
                        }
                    },
                    false,
                    new IFilterOption[]
                    {
                        new TextBoxFilterOption
                        {
                            Key = "txt-key",
                            Type = FilterOptionType.TextBoxFilterOption.ToString(),
                            Value = "txt-val"
                        }
                    }
                };
                yield return new object[]
                {
                    new ListRequest
                    {
                        FilterRequest = new FilterRequest
                        {
                            Filters = new IFilterCategory[]
                            {
                                new TextBoxFilterCategory
                                {
                                    Key = "txt-key",
                                    Value = "txt-val"
                                }
                            }
                        }
                    },
                    true,
                    new IFilterOption[]
                    {
                        new TextBoxFilterOption
                        {
                            Key = "txt-key",
                            Type = FilterOptionType.TextBoxFilterOption.ToString(),
                            Value = "txt-val"
                        }
                    }
                };
            }
        }

        private static IEnumerable<object[]> GetPaginationViewModel_TestData
        {
            get
            {
                yield return new object[]
                {
                    null,
                    new ListResult<object>
                    {
                        TotalItems = 123,
                        TotalPages = 456
                    },
                    new PaginationViewModel
                    {
                        TotalItems = 123,
                        TotalPages = 456,
                        Page = 1,
                        PageSize = _configuration.ListPageSize
                    }
                };
                yield return new object[]
                {
                    new ListRequest
                    {
                        Page = 8765,
                        FilterRequest = new FilterRequest()
                    },
                    new ListResult<object>
                    {
                        TotalItems = 123,
                        TotalPages = 456
                    },
                    new PaginationViewModel
                    {
                        TotalItems = 123,
                        TotalPages = 456,
                        Page = 8765,
                        PageSize = _configuration.ListPageSize
                    }
                };
            }
        }

        private ListHelper GetTestHelper()
        {
            Mock.Get(_systemProvider)
                    .SetupGet(p => p.DateTime)
                    .Returns(_dateTimeProvider);

            Mock.Get(_dateTimeProvider)
                    .Setup(p => p.UtcNow())
                    .Returns(DateTime.UtcNow);

            return new ListHelper(
                _routeValueDictBuilder,
                _mapper,
                _systemProvider,
                Options.Create(_configuration));
        }
    }
}