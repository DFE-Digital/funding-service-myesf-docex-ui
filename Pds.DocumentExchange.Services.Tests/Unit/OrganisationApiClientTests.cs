using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.Core.Common.Organisation.Enums;
using Pds.Core.Common.Organisation.Models;
using Pds.Core.Logging;
using Pds.Core.Utils.Helpers;
using Pds.DocumentExchange.Services.Configuration;
using Pds.DocumentExchange.Services.Implementations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Pds.DocumentExchange.Services.Tests.Unit
{
    [TestClass]
    [TestCategory("Unit")]
    public class OrganisationApiClientTests : BaseApiClientTests
    {
        private readonly ILoggerAdapter<OrganisationApiClient> _logger = Mock.Of<ILoggerAdapter<OrganisationApiClient>>();

        [TestMethod]
        [DataRow(OrganisationIdentifierType.Ukprn, "12345678", false)]
        [DataRow(OrganisationIdentifierType.Ukprn, "", true)]
        [DataRow(OrganisationIdentifierType.CompanyRegistrationNumber, "98765", true)]
        public async Task GetOrganisation_WithIdentifier_ReturnsResultAsExpected(OrganisationIdentifierType identifierType, string identifierValue, bool isNullResult)
        {
            // Arrange
            var identifier = new OrganisationIdentifier { Type = identifierType, Value = identifierValue };

            var expected = new Organisation
            {
                Identifiers = new[] { identifier },
                Name = "test",
                OrganisationType = "type",
                OrganisationTypeDisplay = new DisplayValues
                {
                    Plural = "types",
                    Singular = "type",
                },
                OrganisationSubType = "sub type",
                OrganisationSubTypeDisplay = new DisplayValues
                {
                    Plural = "sub types",
                    Singular = "sub type",
                },
                Status = "Open",
                ParentOrganisation = new Organisation(),
                ChildOrganisations = new[] { new Organisation() }
            };

            SetupMessageHandler(HttpStatusCode.OK, expected);

            var client = GetOrganisationApiClient();

            // Act
            var result = await client.GetOrganisation(identifier);

            // Assert
            if (isNullResult)
            {
                result.Should().BeNull();
            }
            else
            {
                result.Should().NotBeNull();
                result.Identifiers.Should().AllBeEquivalentTo(identifier);
                result.Name.Should().BeEquivalentTo("test");
                VerifyMessageHandler(HttpMethod.Get, TestBaseAddress + $"api/organisation/{identifier.Value}");
            }
        }

        [TestMethod]
        [DataRow("12345678", false)]
        [DataRow("college", false)]
        [DataRow("", true)]
        public async Task Search_WithSearchTerm_ReturnsResultAsExpected(string searchTerm, bool isEmptyResult)
        {
            // Arrange
            var expected = new[]
            {
                new Organisation
                {
                    Identifiers = new[] { new OrganisationIdentifier { Type = OrganisationIdentifierType.Ukprn, Value = "12345678" } },
                    Name = "college",
                    OrganisationType = "type",
                    OrganisationTypeDisplay = new DisplayValues
                    {
                        Plural = "types",
                        Singular = "type",
                    },
                    OrganisationSubType = "sub type",
                    OrganisationSubTypeDisplay = new DisplayValues
                    {
                        Plural = "sub types",
                        Singular = "sub type",
                    },
                    Status = "Open",
                    ParentOrganisation = new Organisation(),
                    ChildOrganisations = new[] { new Organisation() }
                }
            }.AsSafeReadOnlyList();

            SetupMessageHandler(HttpStatusCode.OK, expected);

            var client = GetOrganisationApiClient();

            // Act
            var result = await client.Search(searchTerm, 1);

            // Assert
            if (isEmptyResult)
            {
                result.Should().BeEmpty();
            }
            else
            {
                result.Should().NotBeEmpty();
                result.First().Identifiers.Should().AllBeEquivalentTo(new OrganisationIdentifier { Type = OrganisationIdentifierType.Ukprn, Value = "12345678" });
                result.First().Name.Should().BeEquivalentTo("college");
                VerifyMessageHandler(HttpMethod.Get, TestBaseAddress + $"api/organisation/search/{searchTerm}/1");
            }
        }

        private OrganisationApiClient GetOrganisationApiClient()
        {
            return new OrganisationApiClient(
                GetMockAuthenticationService<DataApiClientConfiguration>().Object,
                GetHttpClient(),
                Options.Create(GetServicesConfiguration()),
                _logger);
        }
    }
}
