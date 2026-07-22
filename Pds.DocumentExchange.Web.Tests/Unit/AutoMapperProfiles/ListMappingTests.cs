using AutoMapper;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pds.Core.Web.Components.Areas.Lists.Models;
using Pds.DocumentExchange.Services.Models.Filters;
using Pds.DocumentExchange.Web.Automapper;
using System;

namespace Pds.DocumentExchange.Web.Tests.Unit.AutoMapperProfiles
{
    [TestClass]
    [TestCategory("Unit")]
    public class ListMappingTests
    {
        private readonly MapperConfiguration _mapperConfiguration;
        private readonly Mapper _mapper;

        public ListMappingTests()
        {
            var profile = new ListMapping();
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
        public void MapDateRangeFilter_ReturnsMappedDateRangeFilterViewModel()
        {
            // Arrange
            var dateRangeFilter = new DateRangeFilter
            {
                Title = "Date-range-title",
                Key = "date-range-key",
                FromTitle = "from-title",
                From = new DateTime(2020, 1, 1),
                ToTitle = "to-title",
                To = new DateTime(2020, 12, 31)
            };

            var dateRangeFilterViewModel = new DateRangeFilterViewModel
            {
                Title = dateRangeFilter.Title,
                Key = dateRangeFilter.Key,
                FromDay = dateRangeFilter.From.Value.Day,
                FromMonth = dateRangeFilter.From.Value.Month,
                FromYear = dateRangeFilter.From.Value.Year,
                ToDay = dateRangeFilter.To.Value.Day,
                ToMonth = dateRangeFilter.To.Value.Month,
                ToYear = dateRangeFilter.To.Value.Year
            };

            // Act
            var result = _mapper.Map<DateRangeFilter, DateRangeFilterViewModel>(dateRangeFilter);

            // Assert
            result.Should().BeEquivalentTo(dateRangeFilterViewModel);
        }
    }
}