using Microsoft.Extensions.Options;
using Pds.Core.Utils;
using Pds.Core.Web.Components.Areas.Lists.Builders;
using Pds.Core.Web.Components.Areas.Lists.DTOs;
using Pds.Core.Web.Components.Areas.Lists.Models;
using Pds.DocumentExchange.Services.Enums;
using Pds.DocumentExchange.Services.Models;
using Pds.DocumentExchange.Services.Models.Filters;
using Pds.DocumentExchange.Web.DTOs;
using Pds.DocumentExchange.Web.Enums;
using Pds.DocumentExchange.Web.Interfaces.Helpers;
using Pds.DocumentExchange.Web.Models;
using System.Collections.Generic;
using System.Linq;

namespace Pds.DocumentExchange.Web.Implementations.Helpers
{
    /// <inheritdoc cref="IListHelper"/>
    public sealed class ListHelper : IListHelper
    {
        private readonly IRouteValueDictionaryBuilder _routeValueDictBuilder;
        private readonly IMapper _mapper;
        private readonly ISystemProvider _systemProvider;
        private readonly DocumentExchangeConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListHelper"/> class.
        /// </summary>
        /// <param name="routeValueDictBuilder"><see cref="IRouteValueDictionaryBuilder"/>.</param>
        /// <param name="mapper"><see cref="IMapper"/>.</param>
        /// <param name="configurationOptions">Options containing <see cref="DocumentExchangeConfiguration"/>.</param>
        public ListHelper(
            IRouteValueDictionaryBuilder routeValueDictBuilder,
            IMapper mapper,
            ISystemProvider systemProvider,
            IOptions<DocumentExchangeConfiguration> configurationOptions)
        {
            _routeValueDictBuilder = routeValueDictBuilder;
            _mapper = mapper;
            _systemProvider = systemProvider;
            _configuration = configurationOptions.Value;
        }

        /// <inheritdoc/>
        public DocumentListUpdateData GetDocumentListUpdateData(IListViewModel viewModel)
        {
            return new DocumentListUpdateData
            {
                ListItems = viewModel.ListItems,
                Pagination = new PaginationUpdateData
                {
                    TotalItems = viewModel.Pagination.TotalItems,
                    TotalPages = viewModel.Pagination.TotalPages,
                    FirstItemNumberOnSelectedPage = viewModel.Pagination.FirstItemNumberOnSelectedPage,
                    LastItemNumberOnSelectedPage = viewModel.Pagination.LastItemNumberOnSelectedPage,
                    FirstPageNumberControl = viewModel.Pagination.FirstPageNumberControl,
                    LastPageNumberControl = viewModel.Pagination.LastPageNumberControl
                }
            };
        }

        /// <inheritdoc/>
        public bool AnyDocumentsAvailable<T>(ListRequest request, ListResult<T> listResult)
        {
            if (listResult?.TotalItems > 0 || listResult?.Items?.Any() == true)
            {
                return true;
            }

            // If any filters were returned then assume some docs are available but they've all been filtered out.
            return listResult?.Filters?.Any() == true;
        }

        /// <inheritdoc/>
        public IEnumerable<IFilterOption> GetFilterOptions(ListRequest request, bool uses6MonthLimit = false)
        {
            if (uses6MonthLimit && request?.FilterRequest?.Filters?.Length == 0)
            {
                return new List<IFilterOption>()
                {
                    new DateRangeFilterOption()
                    {
                        Key = FilterKey.UploadDate.ToString(),
                        Type = FilterOptionType.DateRangeFilterOption.ToString(),
                        From = _systemProvider.DateTime.UtcNow().AddMonths(-6),
                        To = _systemProvider.DateTime.UtcNow()
                    }
                };
            }
            else
            {
                return request?.FilterRequest?.Filters?.Select<IFilterCategory, IFilterOption>(filter =>
                {
                    return filter switch
                    {
                        ListFilterCategory listFilter => new ListFilterOption
                        {
                            Key = listFilter.Key,
                            Type = FilterOptionType.ListFilterOption.ToString(),
                            Values = listFilter.Values
                        },
                        DateRangeFilterCategory dateFilter => new DateRangeFilterOption
                        {
                            Key = dateFilter.Key,
                            Type = FilterOptionType.DateRangeFilterOption.ToString(),
                            From = dateFilter.From,
                            To = dateFilter.To
                        },
                        RadioFilterCategory radioFilter => new RadioFilterOption
                        {
                            Key = radioFilter.Key,
                            Type = FilterOptionType.RadioFilterOption.ToString(),
                            Value = radioFilter.Value
                        },
                        TextBoxFilterCategory textBoxFilter => new TextBoxFilterOption
                        {
                            Key = textBoxFilter.Key,
                            Type = FilterOptionType.TextBoxFilterOption.ToString(),
                            Value = textBoxFilter.Value
                        },
                        _ => null,
                    };
                });
            }
        }

        /// <inheritdoc/>
        public PaginationViewModel GetPaginationViewModel<T>(ListRequest request, ListResult<T> listResult)
        {
            return new PaginationViewModel
            {
                TotalItems = listResult.TotalItems,
                TotalPages = listResult.TotalPages,
                Page = request?.Page ?? 1,
                PageSize = _configuration.ListPageSize,
                BuildPageLinkRouteValues = page =>
                   _routeValueDictBuilder.BuildListPageRouteValues(page, request?.FilterRequest)
            };
        }

        /// <inheritdoc/>
        public IEnumerable<IFilterCategoryViewModel> GetFilterCategories<T>(ListResult<T> listResult)
        {
            return listResult.Filters.Select(
               filter => _mapper.ToFilterCategoryView(filter));
        }
    }
}