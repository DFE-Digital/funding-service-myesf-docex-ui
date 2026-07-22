namespace Pds.DocumentExchange.Services.Models
{
    /// <summary>
    /// Class representing a document in an agency file share.
    /// </summary>
    public class AgencyDocument : Document
    {
        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the document is valid.
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Gets or sets the reason that the file name is invalid, if <see cref="IsValid" /> is false.
        /// </summary>
        public string FileNameError { get; set; }

        /// <summary>
        /// Gets or sets the team ID the document belongs to.
        /// </summary>
        public string Team { get; set; }
    }
}