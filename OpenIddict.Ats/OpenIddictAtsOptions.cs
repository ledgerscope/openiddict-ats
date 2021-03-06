/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating to this project.
 */

using Ats.Driver;
using Microsoft.Azure.Cosmos.Table;
using OpenIddict.Ats.Models;

namespace OpenIddict.Ats
{
    /// <summary>
    /// Provides various settings needed to configure the OpenIddict ATS integration.
    /// </summary>
    public class OpenIddictAtsOptions
    {
        /// <summary>
        /// Gets or sets the name of the applications collection (by default, openiddict.applications).
        /// </summary>
        public string ApplicationsCollectionName { get; set; } = "OpenIddictApplications";

        /// <summary>
        /// Gets or sets the name of the authorizations collection (by default, openiddict.authorizations).
        /// </summary>
        public string AuthorizationsCollectionName { get; set; } = "OpenIddictAuthorizations";

        /// <summary>
        /// Gets or sets the <see cref="ICloudTableClient"/> used by the OpenIddict stores.
        /// If no value is explicitly set, the database is resolved from the DI container.
        /// </summary>
        public ICloudTableClient? Database { get; set; }

        /// <summary>
        /// Gets or sets the name of the scopes collection (by default, openiddict.scopes).
        /// </summary>
        public string ScopesCollectionName { get; set; } = "OpenIddictScopes";

        /// <summary>
        /// Gets or sets the name of the tokens collection (by default, openiddict.tokens).
        /// </summary>
        public string TokensCollectionName { get; set; } = "OpenIddictTokens";
    }
}
