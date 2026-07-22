namespace Pds.DocumentExchange.Web.Models.Agency
{
    /// <summary>
    /// View model representing an invalid document stored in a file share.
    /// </summary>
    public class InvalidFileShareDocument : FileShareDocument
    {
        /// <summary>
        /// Gets or sets a description of the reason that this
        /// document is invalid and unable to be published.
        /// </summary>
        public string FileNameError { get; set; }
    }
}