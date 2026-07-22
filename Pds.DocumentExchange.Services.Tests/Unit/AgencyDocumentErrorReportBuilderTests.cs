using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.Core.Documents.Aspose.Interfaces;
using Pds.Core.Documents.Aspose.Models;
using Pds.Core.Logging;
using Pds.DocumentExchange.Services.Enums;
using Pds.DocumentExchange.Services.Implementations;
using Pds.DocumentExchange.Services.Interfaces;
using Pds.DocumentExchange.Services.Models;
using Pds.DocumentExchange.Services.Models.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pds.DocumentExchange.Services.Tests.Unit
{
    [TestClass]
    public class AgencyDocumentErrorReportBuilderTests
    {
        private readonly IAgencyApiClient _agencyApiClient
            = Mock.Of<IAgencyApiClient>();

        private readonly ISpreadsheetBuilder _spreadsheetBuilder
            = Mock.Of<ISpreadsheetBuilder>();

        private readonly ILoggerAdapter<AgencyDocumentErrorReportBuilder> _logger
            = Mock.Of<ILoggerAdapter<AgencyDocumentErrorReportBuilder>>();

        [TestMethod, TestCategory("Unit")]
        public void BuildErrorReportSpreadsheet_WhenNoTeamPassed_ReturnsArgumentException()
        {
            // Arrange.
            var agencyDocumentErrorReportBuilderClass = GetAgencyDocumentErrorReportBuilderClass();
            var team = string.Empty;
            var listOptions = new AgencyListDocumentOptions();

            // Act.
            Func<Task> act = async () => await agencyDocumentErrorReportBuilderClass.BuildErrorReportSpreadsheet(team, listOptions);

            // Assert.
            act.Should().ThrowAsync<ArgumentException>();
        }

        [TestMethod, TestCategory("Unit")]
        public void BuildErrorReportSpreadsheet_WhenAgencyDocumentIsValid_ReturnsArgumentException()
        {
            // Arrange.
            var agencyDocumentErrorReportBuilderClass = GetAgencyDocumentErrorReportBuilderClass();
            var team = "FakeTeam";
            var listOptions = new AgencyListDocumentOptions()
            {
                Validity = AgencyDocumentValidity.Valid
            };

            // Act.
            Func<Task> act = async () => await agencyDocumentErrorReportBuilderClass.BuildErrorReportSpreadsheet(team, listOptions);

            // Assert.
            act.Should().ThrowAsync<ArgumentException>();
        }

        [TestMethod, TestCategory("Unit")]
        public async Task BuildErrorReportSpreadsheet_WhenSuccessful_BuildsSpreadsheetWithData()
        {
            // Arrange.
            var agencyDocumentErrorReportBuilderClass = GetAgencyDocumentErrorReportBuilderClass();
            var team = "FakeTeam";
            var listOptions = new AgencyListDocumentOptions()
            {
                Validity = AgencyDocumentValidity.Invalid
            };

            Mock.Get(_agencyApiClient)
                .Setup(c => c.ListTeamDocuments(It.IsAny<string>(), It.IsAny<AgencyListDocumentOptions>()))
                .ReturnsAsync(new ListResult<AgencyDocument>()
                {
                    Filters = new List<IFilter>(),
                    Items = Enumerable.Empty<AgencyDocument>()
                })
                .Verifiable();

            var expectedResult = new byte[1];
            Mock.Get(_spreadsheetBuilder)
                .Setup(s => s.BuildSpreadsheetWithData(It.IsAny<Spreadsheet>(), true, false))
                .Returns(expectedResult)
                .Verifiable();

            Mock.Get(_logger)
                .Setup(l => l.LogInformation(It.IsAny<string>()))
                .Verifiable();

            // Act.
            var result = await agencyDocumentErrorReportBuilderClass.BuildErrorReportSpreadsheet(team, listOptions);

            // Assert.
            Mock.Verify();
            result.Should().BeEquivalentTo(expectedResult);
        }

        private AgencyDocumentErrorReportBuilder GetAgencyDocumentErrorReportBuilderClass()
        {
            return new AgencyDocumentErrorReportBuilder(
                _agencyApiClient,
                _spreadsheetBuilder,
                _logger);
        }
    }
}