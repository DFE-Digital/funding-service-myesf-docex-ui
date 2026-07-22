using Pds.DocumentExchange.Web.Attributes;
using System;

namespace Pds.DocumentExchange.Web.Enums
{
    /// <summary>
    /// Enumeration of possible types of Document exchange users.
    /// </summary>
    [Flags]
    public enum DocumentExchangeUserType
    {
        /// <summary>
        /// None.
        /// </summary>
        None = 0,

        /// <summary>
        /// Child organisation user.
        /// </summary>
        [UserPermission("view and download documents that were sent to your organisation")]
        [UserPermission("send documents on behalf of your organisation")]
        ChildOrganisation = 1 << 0,

        /// <summary>
        /// Parent organisation user.
        /// </summary>
        [UserPermission("view and download documents that were sent to your group of academies or your MAT")]
        [UserPermission("send documents on behalf of your group of academies or your MAT")]
        ParentOrganisation = 1 << 1,

        /// <summary>
        /// Internal user who can view as an organisation.
        /// </summary>
        [UserPermission("view or download the documents that have been sent to any external organisation")]
        ViewAsOrganisation = 1 << 2,

        /// <summary>
        /// Agency user for a specific team.
        /// </summary>
        [UserPermission("view or download documents uploaded by external users for your team")]
        [UserPermission("publish documents from your fileshare")]
        AgencyTeam = 1 << 3,

        /// <summary>
        /// Agency advanced user.
        /// </summary>
        [UserPermission("publish documents from any fileshare")]
        [UserPermission("view, download or delete documents uploaded by external users")]
        Advanced = 1 << 4,

        /// <summary>
        /// Admin user.
        /// </summary>
        [UserPermission("add new products")]
        [UserPermission("view and edit settings")]
        Admin = 1 << 5
    }
}