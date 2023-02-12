﻿using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Duende.IdentityServer.Models;

namespace ids_server.Data
{
    public class DataSeeder
    {
        public static void SeedIdentityServer(IServiceProvider serviceProvider)
        {
            Console.WriteLine("Seeding data for Identity server");

            var context = serviceProvider
                .GetRequiredService<ConfigurationDbContext>();

            DataSeeder.SeedData(context);
        }

        private static void SeedData(ConfigurationDbContext context)
        {
            if (!context.Clients.Any())
            {
                var clients = new List<Client> {
                    new Client
                    {
                        ClientId = "m2m.client",
                        AllowedGrantTypes = GrantTypes.ClientCredentials,
                        ClientSecrets = { new Secret("SuperSecretPassword".Sha256()) },
                        AllowedScopes = { "api.read" }
                    },
                    new Client
                    {
                        ClientId = "interactive",

                        AllowedGrantTypes = GrantTypes.Code,
                        RequireClientSecret = false,
                        RequirePkce = true,

                        RedirectUris = { "http://localhost:3000/signin-oidc" },
                        PostLogoutRedirectUris = { "http://localhost:3000" },

                        AllowedScopes = { "openid", "profile", "api.read" }
                    },
                };

                foreach (var client in clients)
                {
                    context.Clients.Add(client.ToEntity());
                }
                context.SaveChanges();
                Console.WriteLine($"Added {clients.Count()} clients");
            }
            else
            {
                Console.WriteLine("clients already added..");
            }

            if (!context.ApiResources.Any())
            {
                var apiResources = new List<ApiResource>() {
                    new ApiResource("api") {
                        Scopes = { "api.read" },
                    }
                };

                foreach (var apiRrc in apiResources)
                {
                    context.ApiResources.Add(apiRrc.ToEntity());
                }
                context.SaveChanges();
                Console.WriteLine($"Added {apiResources.Count()} api resources");
            }
            else
            {
                Console.WriteLine("api resources already added..");
            }


            if (!context.ApiScopes.Any())
            {
                var scopes = new List<ApiScope> {
                    new ApiScope("api.read", "Read Access to API"),
                    new ApiScope("api.write", "Write Access to API")
                };

                foreach (var scope in scopes)
                {
                    context.ApiScopes.Add(scope.ToEntity());
                }
                context.SaveChanges();
                Console.WriteLine($"Added {scopes.Count()} api scopes");
            }
            else
            {
                Console.WriteLine("api scopes already added..");
            }
        }
    }
}
