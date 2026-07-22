using AutoMapper;
using Pds.Core.Web.Components.Areas.Lists.Models;

namespace Pds.DocumentExchange.Web.Automapper
{
    /// <summary>
    /// Automapper profile for mapping types relating to lists of entities.
    /// </summary>
    public class ListMapping : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListMapping"/> class.
        /// </summary>
        public ListMapping()
        {
            CreateMap<Services.Models.Product, Models.Shared.Product>();

            CreateMap<Services.Models.Filters.IFilter, IFilterCategoryViewModel>()
                .Include<Services.Models.Filters.ListFilter, ListFilterCategoryViewModel>()
                .Include<Services.Models.Filters.DateRangeFilter, DateRangeFilterViewModel>()
                .Include<Services.Models.Filters.RadioFilter, RadioFilterViewModel>()
                .Include<Services.Models.Filters.TextBoxFilter, TextBoxFilterViewModel>();

            CreateMap<Services.Models.Filters.ListFilter, ListFilterCategoryViewModel>();
            CreateMap<Services.Models.Filters.FilterValue, FilterValueViewModel>();
            CreateMap<Services.Models.Filters.FilterGroup, ListGroupFilterViewModel>();

            CreateMap<Services.Models.Filters.RadioFilter, RadioFilterViewModel>();
            CreateMap<Services.Models.Filters.RadioFilterValue, RadioFilterValueViewModel>();

            CreateMap<Services.Models.Filters.TextBoxFilter, TextBoxFilterViewModel>();

            CreateMap<Services.Models.Filters.DateRangeFilter, DateRangeFilterViewModel>()
                .ConvertUsing(source => new DateRangeFilterViewModel
                {
                    Title = source.Title,
                    Key = source.Key,
                    FromDay = source.From.HasValue ? source.From.Value.Day : null as int?,
                    FromMonth = source.From.HasValue ? source.From.Value.Month : null as int?,
                    FromYear = source.From.HasValue ? source.From.Value.Year : null as int?,
                    ToDay = source.From.HasValue ? source.To.Value.Day : null as int?,
                    ToMonth = source.From.HasValue ? source.To.Value.Month : null as int?,
                    ToYear = source.From.HasValue ? source.To.Value.Year : null as int?
                });
        }
    }
}