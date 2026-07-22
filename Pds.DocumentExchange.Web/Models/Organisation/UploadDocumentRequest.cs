using Microsoft.AspNetCore.Http;

namespace Pds.DocumentExchange.Web.Models.Organisation
{
    /// <summary>A model containing the details of the file being uploaded.</summary>
    public class UploadDocumentRequest
    {
        /// <summary>Gets or sets the file input.</summary>
        public IFormFile FileInput { get; set; }

        /// <summary>Gets or sets the 'uploading for' UKPRN. </summary>
        public string UploadingForUkprn { get; set; }

        /// <summary>Gets or sets the product identifier. </summary>
        public int ProductIdentifier { get; set; }
    }
}