namespace Pds.DocumentExchange.Services.Models
{
    /// <summary>
    /// Represents a downloaded file.
    /// </summary>
    public class DownloadedFile
    {
        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the file content.
        /// </summary>
        public byte[] Content { get; set; }

        /// <summary>
        /// Gets or sets the content type.
        /// </summary>
        public string ContentType { get; set; }
    }
}