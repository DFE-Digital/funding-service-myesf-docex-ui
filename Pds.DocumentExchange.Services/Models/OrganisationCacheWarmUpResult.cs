using System;

namespace Pds.DocumentExchange.Services.Models
{
    /// <summary>
    /// The organisation cache warm-up summary.
    /// </summary>
    public class OrganisationCacheWarmUpResult
    {
        /// <summary>
        /// Gets or sets the function instance ID.
        /// </summary>
        public Guid InstanceId { get; set; }

        /// <summary>
        /// Gets or sets the finished at time.
        /// </summary>
        public DateTime FinishedAt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the parent organisations process was successful.
        /// </summary>
        public bool? ParentIsSuccess { get; set; }

        /// <summary>
        /// Gets or sets the number of successfully processed parent organisations.
        /// </summary>
        public int? NumberOfSuccessfullyProcessedParentOrganisations { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the child organisations process was successful.
        /// </summary>
        public bool? ChildIsSuccess { get; set; }

        /// <summary>
        /// Gets or sets the number of successfully processed child organisations.
        /// </summary>
        public int? NumberOfSuccessfullyProcessedChildOrganisations { get; set; }
    }
}