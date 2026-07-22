using Microsoft.AspNetCore.StaticFiles;
using Pds.DocumentExchange.Services.Interfaces;

namespace Pds.DocumentExchange.Services.Implementations
{
    /// <summary>
    /// A wrapper service for <see cref="FileExtensionContentTypeProvider"/>
    /// that exposes a method to derive the content type from a file name.
    /// </summary>
    public class MimeMappingService : IMimeMappingService
    {
        private static readonly FileExtensionContentTypeProvider _contentTypeProvider
            = new FileExtensionContentTypeProvider();

        /// <inheritdoc/>
        public string GetContentType(string fileName)
        {
            if (_contentTypeProvider.TryGetContentType(fileName, out var contentType))
            {
                return contentType;
            }

            return "application/octet-stream";
        }
    }
}