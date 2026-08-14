using Pds.Core.Web.Components.Areas.Lists.Models;
using Pds.DocumentExchange.Services.Models.Filters;
using System.Collections.Generic;
using System.Linq;

namespace Pds.DocumentExchange.Web.Extensions
{
    /// <summary>
    /// Extension class to map implementations of <see cref="IFilter"/> to <see cref="IFilterCategoryViewModel"/>.
    /// </summary>
    public static class IFilterMappings
    {
        /// <summary>
        /// Extension method to convert an object implementing IFilter to an object implementing IFilterCategoryViewModel.
        /// </summary>
        /// <param name="source">the IFilter source object.</param>
        /// <returns>an IFilterCategoryViewModel object.</returns>
        public static IFilterCategoryViewModel ToFilterCategoryView(this IFilter source)
        {
            switch (source.GetType().Name)
            {
                case "ListFilter":
                    return source.ToListFilterCategoryView();
                case "DateRangeFilter":
                    return ToDateRangeFilterView(source);
                case "RadioFilter":
                    // Convert RadioFilter to RadioFilterViewModel
                    var radioFilter = (RadioFilter)source;
                    List<RadioFilterValueViewModel> rFVViews = radioFilter.GetRadioFilterValueViews();

                    return new RadioFilterViewModel
                    {
                        Title = radioFilter.Title,
                        Key = radioFilter.Key,
                        Values = rFVViews
                    };
                case "TextBoxFilter":
                    // Convert TextBoxFilter to TextBoxFilterViewModel
                    var textBoxFilter = (TextBoxFilter)source;
                    return new TextBoxFilterViewModel
                    {
                        Title = textBoxFilter.Title,
                        Key = textBoxFilter.Key,
                        Value = textBoxFilter.Value,
                        Hint = textBoxFilter.Hint,
                        Regex = textBoxFilter.Regex,
                        ValidationErrorMessage = textBoxFilter.ValidationErrorMessage
                    };
                default:
                    return null;
            }
        }

        private static List<RadioFilterValueViewModel> GetRadioFilterValueViews(this RadioFilter radioFilter)
        {
            var rFVViews = new List<RadioFilterValueViewModel>();

            // Convert RadioFilterValues to RadioFilterValueViewModels
            foreach (var value in radioFilter.Values)
            {
                var rFVViewModel = new RadioFilterValueViewModel
                {
                    Title = value.Title,
                    Value = value.Value,
                    Selected = value.Selected
                };
                rFVViews.Add(rFVViewModel);
            }

            return rFVViews;
        }

        private static IFilterCategoryViewModel ToDateRangeFilterView(IFilter source)
        {
            // convert DateRangeFilter to DateRangeFilterViewModel
            var dateRangeFilter = (DateRangeFilter)source;
            return new DateRangeFilterViewModel
            {
                Title = dateRangeFilter.Title,
                Key = dateRangeFilter.Key,
                FromDay = dateRangeFilter.From.HasValue ? dateRangeFilter.From.Value.Day : null as int?,
                FromMonth = dateRangeFilter.From.HasValue ? dateRangeFilter.From.Value.Month : null as int?,
                FromYear = dateRangeFilter.From.HasValue ? dateRangeFilter.From.Value.Year : null as int?,
                ToDay = dateRangeFilter.From.HasValue ? dateRangeFilter.To.Value.Day : null as int?,
                ToMonth = dateRangeFilter.From.HasValue ? dateRangeFilter.To.Value.Month : null as int?,
                ToYear = dateRangeFilter.From.HasValue ? dateRangeFilter.To.Value.Year : null as int?
            };
        }

        private static IFilterCategoryViewModel ToListFilterCategoryView(this IFilter source)
        {
            // convert ListFilter to ListFilterCategoryViewModel
            var listFilter = (ListFilter)source;

            // convert ListFilter Values (FilterValues) to FilterValueViewModels
            List<FilterValueViewModel> fVViews = listFilter.GetFilterValueViews();

            // convert ListFilter Groups (FilterGroups) to ListGroupFilterViewModels
            List<ListGroupFilterViewModel> groups = listFilter.GetListGroupFilterViews();

            return new ListFilterCategoryViewModel
            {
                Title = listFilter.Title,
                Key = listFilter.Key,
                Values = fVViews,
                Groups = groups
            };
        }

        private static List<FilterValueViewModel> GetFilterValueViews(this ListFilter listFilter)
        {
            var fVViews = new List<FilterValueViewModel>();
            foreach (FilterValue filterValue in listFilter.Values)
            {
                var fVViewModel = new FilterValueViewModel
                {
                    Title = filterValue.Title,
                    Value = filterValue.Value,
                    Selected = filterValue.Selected,
                    Count = filterValue.Count
                };
                fVViews.Add(fVViewModel);
            }

            return fVViews;
        }

        private static List<ListGroupFilterViewModel> GetListGroupFilterViews(this ListFilter listFilter)
        {
            var groups = new List<ListGroupFilterViewModel>();

            if (listFilter.Groups != null && listFilter.Groups.Any())
            {
                foreach (FilterGroup group in listFilter.Groups)
                {
                    // convert FilterGroup's Values (FilterValues) to FilterValueViewModels
                    var filterValueViews = new List<FilterValueViewModel>();
                    foreach (FilterValue filterValue in group.Values)
                    {
                        var fVViewModel = new FilterValueViewModel
                        {
                            Title = filterValue.Title,
                            Value = filterValue.Value,
                            Selected = filterValue.Selected,
                            Count = filterValue.Count
                        };

                        filterValueViews.Add(fVViewModel);
                    }

                    var lGFViewModel = new ListGroupFilterViewModel
                    {
                        Title = group.Title,
                        Values = filterValueViews,
                    };
                    groups.Add(lGFViewModel);
                }
            }

            return groups;
        }
    }
}