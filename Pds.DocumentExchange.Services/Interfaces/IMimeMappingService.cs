namespace Pds.DocumentExchange.Services.Interfaces
{
    /// <summary>
    /// Interface for a service that exposes a method to derive the content type from a file name.
    /// </summary>
    public interface IMimeMappingService
    {
        /// <summary>
        /// Gets the content type for the given file name.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <returns>The content type.</returns>
        string GetContentType(string fileName);
    }
}