namespace Pds.DocumentExchange.Web.Models.SupportTools
{
    /// <summary>
    /// ManagementInformationReportModel.
    /// </summary>
    public class ManagementInformationReportViewModel : BaseDocumentExchangePageViewModel
    {
        /// <summary>
        /// Gets or sets Day in From date.
        /// </summary>
        public int? FromDay { get; set; }

        /// <summary>
        /// Gets or sets Month in From date.
        /// </summary>
        public int? FromMonth { get; set; }

        /// <summary>
        /// Gets or sets Year in From date.
        /// </summary>
        public int? FromYear { get; set; }

        /// <summary>
        /// Gets or sets Day in From date.
        /// </summary>
        public int? ToDay { get; set; }

        /// <summary>
        /// Gets or sets Month in From date.
        /// </summary>
        public int? ToMonth { get; set; }

        /// <summary>
        /// Gets or sets Year in From date.
        /// </summary>
        public int? ToYear { get; set; }

        #region Base view model overrides

        /// <inheritdoc/>
        protected override string Title
            => "Download management information report";

        #endregion
    }
}
