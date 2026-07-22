using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.DocumentExchange.Services.Models;
using Pds.DocumentExchange.Web.Implementations.Renderers;
using System.Collections.Generic;

namespace Pds.DocumentExchange.Web.Tests.Unit.Renderers
{
    [TestClass, TestCategory("Unit")]
    public class DocumentVersionRendererTests
    {
        private string FakeHost => "fakehost";

        private string HttpScheme => "http";

        [TestMethod]
        public void DocumentVersionRenderer_GetHyperLinkedVersionLabels_ReturnsExpected()
        {
            // Arrange
            var mockHttpContextAccessor = Mock.Of<IHttpContextAccessor>();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Host = new HostString(FakeHost);
            httpContext.Request.Scheme = HttpScheme;
            mockHttpContextAccessor.HttpContext = httpContext;

            var mockExchangeDocument = Mock.Of<ExchangeDocument>();
            mockExchangeDocument.Version = 3;
            mockExchangeDocument.DocumentReference = new DocumentReference { BatchIdentifier = "batch-3", FileName = "file-3", ParentBatchIdentifier = "parent-batch-3" };
            mockExchangeDocument.PreviousVersions = new List<ExchangeDocument>
            {
                new ExchangeDocument { Version = 2, DocumentReference = new DocumentReference { BatchIdentifier = "batch-2", FileName = "file-2", ParentBatchIdentifier = "parent-batch-2" } },
                new ExchangeDocument { Version = 1, DocumentReference = new DocumentReference { BatchIdentifier = "batch-1", FileName = "file-1", ParentBatchIdentifier = "parent-batch-1" } }
            };
            var expected = GetExpectedDocumentVersionLabels();
            var renderer = new DocumentVersionRenderer(mockHttpContextAccessor);

            // Act
            var result = renderer.GetHyperLinkedDocumentVersionLabels(mockExchangeDocument);

            // Assert
            result.Should().Be(expected);
        }

        [TestMethod]
        public void DocumentVersionRenderer_GetHyperLinkedVersionLabelsWithPrefix_ReturnsExpected()
        {
            // Arrange
            var mockHttpContextAccessor = Mock.Of<IHttpContextAccessor>();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Host = new HostString(FakeHost);
            httpContext.Request.Scheme = HttpScheme;
            mockHttpContextAccessor.HttpContext = httpContext;

            var mockExchangeDocument = Mock.Of<ExchangeDocument>();
            mockExchangeDocument.Version = 3;
            mockExchangeDocument.DocumentReference = new DocumentReference { BatchIdentifier = "batch-3", FileName = "file-3", ParentBatchIdentifier = "parent-batch-3" };
            mockExchangeDocument.PreviousVersions = new List<ExchangeDocument>
            {
                new ExchangeDocument { Version = 2, DocumentReference = new DocumentReference { BatchIdentifier = "batch-2", FileName = "file-2", ParentBatchIdentifier = "parent-batch-2" } },
                new ExchangeDocument { Version = 1, DocumentReference = new DocumentReference { BatchIdentifier = "batch-1", FileName = "file-1", ParentBatchIdentifier = "parent-batch-1" } }
            };
            var expected = GetExpectedDocumentVersionLabelsWithPrefix();
            var renderer = new DocumentVersionRenderer(mockHttpContextAccessor);

            // Act
            var result = renderer.GetHyperLinkedDocumentVersionLabelWithPrefix(mockExchangeDocument, "version ");

            // Assert
            result.Should().Be(expected);
        }

        [TestMethod]
        public void DocumentVersionRenderer_GetDocumentVersionsWithoutHyperlinks_ReturnsExpected()
        {
            // Arrange
            var mockHttpContextAccessor = Mock.Of<IHttpContextAccessor>();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Host = new HostString(FakeHost);
            httpContext.Request.Scheme = HttpScheme;
            mockHttpContextAccessor.HttpContext = httpContext;

            var mockExchangeDocument = Mock.Of<ExchangeDocument>();
            mockExchangeDocument.Version = 3;
            mockExchangeDocument.DocumentReference = new DocumentReference { BatchIdentifier = "batch-3", FileName = "file-3", ParentBatchIdentifier = "parent-batch-3" };
            mockExchangeDocument.PreviousVersions = new List<ExchangeDocument>
            {
                new ExchangeDocument { Version = 2, DocumentReference = new DocumentReference { BatchIdentifier = "batch-2", FileName = "file-2", ParentBatchIdentifier = "parent-batch-2" } },
                new ExchangeDocument { Version = 1, DocumentReference = new DocumentReference { BatchIdentifier = "batch-1", FileName = "file-1", ParentBatchIdentifier = "parent-batch-1" } }
            };
            var expected = GetExpectedDocumentVersionsWithoutHyperlinks();
            var renderer = new DocumentVersionRenderer(mockHttpContextAccessor);

            // Act
            var result = renderer.GetDocumentVersionsWithoutHyperlinks(mockExchangeDocument);

            // Assert
            result.Should().Be(expected);
        }

        private string GetExpectedDocumentVersionLabels()
        {
            return $"<a class=document-version-label href={HttpScheme}://{FakeHost}{ServiceConstants.PathBase}/download-exchange-document?documentReferenceString=file-3%7cbatch-3%7cparent-batch-3> 3</a>, <a class=document-version-label href={HttpScheme}://{FakeHost}{ServiceConstants.PathBase}/download-exchange-document?documentReferenceString=file-2%7cbatch-2%7cparent-batch-2> 2</a>, <a class=document-version-label href={HttpScheme}://{FakeHost}{ServiceConstants.PathBase}/download-exchange-document?documentReferenceString=file-1%7cbatch-1%7cparent-batch-1> 1</a>";
        }

        private string GetExpectedDocumentVersionLabelsWithPrefix()
        {
            return $"<a class=document-version-label href={HttpScheme}://{FakeHost}{ServiceConstants.PathBase}/download-exchange-document?documentReferenceString=file-3%7cbatch-3%7cparent-batch-3> version 3</a>";
        }

        private string GetExpectedDocumentVersionsWithoutHyperlinks()
        {
            return "3, 2, 1";
        }
    }
}