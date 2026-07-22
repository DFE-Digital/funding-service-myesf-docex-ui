using Microsoft.AspNetCore.Authorization;
using Pds.Core.Common.Identity.Constants;
using Pds.Core.Common.Identity.Enums;
using System.Linq;

namespace Pds.DocumentExchange.Web.Helpers
{
    /// <summary>
    /// Helper class for configuring policy-based authorization.
    /// </summary>
    public static class AuthorizationHelper
    {
        /// <summary>
        /// A structure to hold the policy names.
        /// </summary>
        public static class Policies
        {
            /// <summary>
            /// The name of a policy requiring any role with access to upload documents for an organisation in Document Exchange.
            /// </summary>
            public const string RequireDocumentExchangeOrganisationDocumentUploaderRole = "RequireDocumentExchangeOrganisationDocumentUploaderRole";

            /// <summary>
            /// The name of a policy requiring any Document Exchange agency role.
            /// </summary>
            public const string RequireAnyDocumentExchangeAgencyRole = "RequireAnyDocumentExchangeAgencyRole";

            /// <summary>
            /// The name of a policy requiring the Document Exchange advanced user role.
            /// </summary>
            public const string RequireDocumentExchangeAgencyAdvancedUserRole = "RequireDocumentExchangeAgencyAdvancedUserRole";

            /// <summary>
            /// The name of a policy requiring any role with access to view an organisation's documents in Document Exchange.
            /// </summary>
            public const string RequireDocumentExchangeOrganisationDocumentViewerRole = "RequireDocumentExchangeOrganisationDocumentViewerRole";

            /// <summary>
            /// The name of a policy requiring Document Exchange Admin role.
            /// </summary>
            public const string RequireDocumentExchangeAdminRole = "RequireDocumentExchangeAdminRole";

            /// <summary>
            /// The name of a policy requiring View as Organisation role.
            /// </summary>
            public const string RequireViewAsOrganisationRole = "RequireViewAsOrganisationRole";

            /// <summary>
            /// The name of a policy requiring any Document Exchange internal user role.
            /// </summary>
            public const string RequireAnyDocumentExchangeInternalUserRoles = "RequireAnyDocumentExchangeInternalUserRoles";

            /// <summary>
            /// The name of a policy requiring any Document Exchange advanced agency admin role.
            /// </summary>
            public const string RequireDocumentExchangeAdvancedOrAdminRoles = "RequireDocumentExchangeAdvancedOrAdminRoles";
        }

        /// <summary>
        /// Registers authorization policies against the given <see cref="AuthorizationOptions"/> object.
        /// </summary>
        /// <param name="options">The <see cref="AuthorizationOptions"/> to which policies will be added.</param>
        public static void AddDocumentExchangePolicies(this AuthorizationOptions options)
        {
            options.AddPolicy(
                Policies.RequireDocumentExchangeOrganisationDocumentUploaderRole,
                policy => policy.RequireClaim(PdsClaimTypes.Role, DocumentExchangeOrganisationRole));

            options.AddPolicy(
                Policies.RequireDocumentExchangeOrganisationDocumentViewerRole,
                policy => policy.RequireClaim(PdsClaimTypes.Role, DocumentExchangeOrganisationRole, ViewAsOrganisationRole));

            options.AddPolicy(
                Policies.RequireAnyDocumentExchangeAgencyRole,
                policy => policy.RequireClaim(PdsClaimTypes.Role, DocumentExchangeAgencyRoles));

            options.AddPolicy(
               Policies.RequireDocumentExchangeAdminRole,
               policy => policy.RequireClaim(PdsClaimTypes.Role, DocumentExchangeAdminRole));

            options.AddPolicy(
                Policies.RequireDocumentExchangeAgencyAdvancedUserRole,
                policy => policy.RequireClaim(PdsClaimTypes.Role, DocumentExchangeAgencyAdvancedUserRole));

            options.AddPolicy(
               Policies.RequireViewAsOrganisationRole,
               policy => policy.RequireClaim(PdsClaimTypes.Role, ViewAsOrganisationRole));

            options.AddPolicy(
               Policies.RequireAnyDocumentExchangeInternalUserRoles,
               policy => policy.RequireClaim(PdsClaimTypes.Role, DocumentExchangeInternalUserRoles));

            options.AddPolicy(
              Policies.RequireDocumentExchangeAdvancedOrAdminRoles,
              policy => policy.RequireClaim(PdsClaimTypes.Role, DocumentExchangeAdvancedOrAdminRoles));
        }

        /// <summary>
        /// Gets the list of user roles for agency team users.
        /// </summary>
        public static string[] DocumentExchangeAgencyTeamRoles
            => new[]
            {
                UserRole.DocumentExchangeAdministratorFundingCentre.ToString(),
                UserRole.DocumentExchangeAdministratorRiskAssurance.ToString()
            };

        /// <summary>
        /// Gets the agency advanced user role.
        /// </summary>
        public static string DocumentExchangeAgencyAdvancedUserRole
            => UserRole.DocumentExchangeAdvancedUser.ToString();

        /// <summary>
        /// Gets the list of all roles that give access to the document exchange.
        /// </summary>
        public static string[] DocumentExchangeRoles
        {
            get
            {
                var roles = DocumentExchangeAgencyRoles.ToList();
                roles.Add(DocumentExchangeOrganisationRole);
                roles.Add(ViewAsOrganisationRole);
                roles.Add(DocumentExchangeAdminRole);

                return roles.ToArray();
            }
        }

        /// <summary>
        /// Gets the admin user role.
        /// </summary>
        public static string DocumentExchangeAdminRole
           => UserRole.DocumentExchangeAdmin.ToString();

        /// <summary>
        /// Gets the view as organisation user role.
        /// </summary>
        public static string ViewAsOrganisationRole
            => UserRole.ViewAsProvider.ToString();

        /// <summary>
        /// Gets the list of all roles that give access to the org data validation search.
        /// </summary>
        public static string[] DocumentExchangeInternalUserRoles
        {
            get
            {
                var roles = DocumentExchangeAgencyRoles.ToList();
                roles.Add(DocumentExchangeAgencyAdvancedUserRole);
                roles.Add(DocumentExchangeAdminRole);

                return roles.ToArray();
            }
        }

        /// <summary>
        /// Gets the list of all roles that give access to the org data validation search.
        /// </summary>
        public static string[] DocumentExchangeAdvancedOrAdminRoles
        {
            get
            {
                return new string[] { DocumentExchangeAgencyAdvancedUserRole, DocumentExchangeAdminRole };
            }
        }

        private static string[] DocumentExchangeAgencyRoles
        {
            get
            {
                var roles = DocumentExchangeAgencyTeamRoles.ToList();
                roles.Add(DocumentExchangeAgencyAdvancedUserRole);
                return roles.ToArray();
            }
        }

        private static string DocumentExchangeOrganisationRole
            => UserRole.DocumentExchangeUser.ToString();
    }
}