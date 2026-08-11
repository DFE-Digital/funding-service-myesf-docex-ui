using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pds.Core.Web.Components.Areas.Lists.Models;
using Pds.DocumentExchange.Services.Enums;
using Pds.DocumentExchange.Services.Models.Filters;
using Pds.DocumentExchange.Web.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pds.DocumentExchange.Web.Tests.Unit.Extensions
{
    [TestClass]
    [TestCategory("Unit")]
    public class IFilterMappingTests
    {
        [TestMethod]
        public void ToFilterCategoryView_WhenCalledOnListFilter_ReturnsMappedListFilterViewModel()
        {
            // Arrange
            var sourceListFilter = new ListFilter
            {
                Title = "Test Filter",
                Key = "testfilter",
                Type = FilterType.ListFilter.ToString(),
                Values = new[]
                {
                    new FilterValue
                    {
                        Title = "Test Filter Value",
                        Value = "testfiltervalue",
                        Selected = false,
                        Count = 2
                    }
                },
                Groups = new[]
                {
                    new FilterGroup
                    {
                        Title = "Test Group Value",
                        Values = new[]
                        {
                            new FilterValue
                            {
                                Title = "Test Filter Value 2",
                                Value = "testfiltervalue2",
                                Selected = false,
                                Count = 3
                            }
                        }
                    }
                }
            };

            var destinationListFilterCategoryViewModel = new ListFilterCategoryViewModel
            {
                Title = sourceListFilter.Title,
                Key = sourceListFilter.Key,
                Values = new List<FilterValueViewModel>
                {
                    new FilterValueViewModel
                    {
                        Title = ((FilterValue[])sourceListFilter.Values)[0].Title,
                        Value = ((FilterValue[])sourceListFilter.Values)[0].Value,
                        Count = ((FilterValue[])sourceListFilter.Values)[0].Count,
                        Selected = ((FilterValue[])sourceListFilter.Values)[0].Selected,
                    }
                },
                Groups = new List<ListGroupFilterViewModel>
                {
                    new ListGroupFilterViewModel
                    {
                        Title = ((FilterGroup[])sourceListFilter.Groups)[0].Title,
                        Values = new List<FilterValueViewModel>
                        {
                            new FilterValueViewModel
                            {
                                Title = ((FilterGroup[])sourceListFilter.Groups)[0].Values.First().Title,
                                Value = ((FilterGroup[])sourceListFilter.Groups)[0].Values.First().Value,
                                Selected = ((FilterGroup[])sourceListFilter.Groups)[0].Values.First().Selected,
                                Count = ((FilterGroup[])sourceListFilter.Groups)[0].Values.First().Count
                            }
                        }
                    }
                }
            };

            // Act
            var result = sourceListFilter.ToFilterCategoryView();

            // Assert
            result.Should().BeEquivalentTo(destinationListFilterCategoryViewModel);
        }

        [TestMethod]
        public void ToFilterCategoryView_WhenCalledOnDateRangeFilter_ReturnsMappedDateRangeFilterViewModel()
        {
            // Arrange
            var sourceDateRangeFilter = new DateRangeFilter
            {
                Title = "Date-range-title",
                Key = "date-range-key",
                FromTitle = "from-title",
                From = new DateTime(2020, 1, 1),
                ToTitle = "to-title",
                To = new DateTime(2020, 12, 31)
            };

            var destinationDateRangeFilterViewModel = new DateRangeFilterViewModel
            {
                Title = sourceDateRangeFilter.Title,
                Key = sourceDateRangeFilter.Key,
                FromDay = sourceDateRangeFilter.From.Value.Day,
                FromMonth = sourceDateRangeFilter.From.Value.Month,
                FromYear = sourceDateRangeFilter.From.Value.Year,
                ToDay = sourceDateRangeFilter.To.Value.Day,
                ToMonth = sourceDateRangeFilter.To.Value.Month,
                ToYear = sourceDateRangeFilter.To.Value.Year
            };

            // Act
            var result = sourceDateRangeFilter.ToFilterCategoryView();

            // Assert
            result.Should().BeEquivalentTo(destinationDateRangeFilterViewModel);
        }

        [TestMethod]
        public void ToFilterCategoryView_WhenCalledOnRadioFilter_ReturnsMappedRadioFilterViewModel()
        {
            // Arrange
            var sourceRadioFilter = new RadioFilter
            {
                Key = "radioFilterKey1",
                Title = "Filter 1",
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

            var destinationRadioFilterViewModel = new RadioFilterViewModel
            {
                Title = sourceRadioFilter.Title,
                Key = sourceRadioFilter.Key,
                Values = new List<RadioFilterValueViewModel>
                {
                    new RadioFilterValueViewModel
                    {
                        Title = ((List<RadioFilterValue>)sourceRadioFilter.Values)[0].Title,
                        Value = ((List<RadioFilterValue>)sourceRadioFilter.Values)[0].Value,
                        Selected = ((List<RadioFilterValue>)sourceRadioFilter.Values)[0].Selected
                    },
                    new RadioFilterValueViewModel
                    {
                        Title = ((List<RadioFilterValue>)sourceRadioFilter.Values)[1].Title,
                        Value = ((List<RadioFilterValue>)sourceRadioFilter.Values)[1].Value,
                        Selected = ((List<RadioFilterValue>)sourceRadioFilter.Values)[1].Selected
                    },
                }
            };

            // Act
            var result = sourceRadioFilter.ToFilterCategoryView();

            // Assert
            result.Should().BeEquivalentTo(destinationRadioFilterViewModel);
        }

        [TestMethod]
        public void ToFilterCategoryView_WhenCalledOnTextBoxFilter_ReturnsMappedTextBoxFilterViewModel()
        {
            // Arrange
            var sourceTextBoxFilter = new TextBoxFilter
            {
                Title = "Filter 1",
                Key = "textBoxFilterKey1",
                Value = "value-1",
                Hint = "Hint1",
                Regex = "[a-z]",
                ValidationErrorMessage = "Error-message1"
            };

            var destinationTextBoxFilterViewModel = new TextBoxFilterViewModel
            {
                Title = sourceTextBoxFilter.Title,
                Key = sourceTextBoxFilter.Key,
                Value = sourceTextBoxFilter.Value,
                Hint = sourceTextBoxFilter.Hint,
                Regex = sourceTextBoxFilter.Regex,
                ValidationErrorMessage = sourceTextBoxFilter.ValidationErrorMessage
            };

            // Act
            var result = sourceTextBoxFilter.ToFilterCategoryView();

            // Assert
            result.Should().BeEquivalentTo(destinationTextBoxFilterViewModel);
        }
    }
}