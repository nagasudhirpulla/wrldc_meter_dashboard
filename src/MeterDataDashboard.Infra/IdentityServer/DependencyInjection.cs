using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;

namespace MeterDataDashboard.Infra.IdentityServer
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddIdentityServerInfra(this IServiceCollection services, IConfiguration configuration)
        {
            // setup identity server for electron dashboard scada archive data access
            // https://demo.identityserver.io/.well-known/openid-configuration
            // https://medium.com/all-technology-feeds/testing-your-asp-net-core-webapi-secured-with-identityserver4-in-postman-97eee976aa16
            services.AddIdentityServer()
                .AddInMemoryApiResources(new List<ApiResource>
                {
                    new ApiResource("scada_archive", "SCADA Archive Data Access API")
                })
                .AddInMemoryClients(new List<Client>
                {
                    new Client
                    {
                        ClientId = configuration["ElectronDashboard:ClientID"],
                        // no interactive user, use the clientid/secret for authentication
                        AllowedGrantTypes = GrantTypes.ClientCredentials,
                        // secret for authentication
                        ClientSecrets =
                        {
                            new Secret(configuration["ElectronDashboard:ClientSecret"].Sha256())
                        },
                        // scopes that client has access to
                        AllowedScopes = { "scada_archive" }
                    }
                })
                .AddDeveloperSigningCredential();

            services.AddAuthentication()
                    .AddIdentityServerJwt();

            return services;
        }
    }
}
