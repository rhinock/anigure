using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdentityServer()
    .AddInMemoryApiScopes(new List<ApiScope>
    {
        new("api.read", "Read Access To API")
    })
    .AddInMemoryApiResources(new List<ApiResource>
    {
        new("api")
        {
            Scopes = { "api.read" }
        }
    })
    .AddInMemoryClients(new List<Client>
    {
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

            RedirectUris = { "http://localhost:3000/signin-oidc" },
            PostLogoutRedirectUris = { "http://localhost:3000" },

            AllowedScopes = { "openid", "profile", "api.read" }
        }
    })
    .AddInMemoryIdentityResources(new List<IdentityResource>
    {
        new IdentityResources.OpenId(),
        new IdentityResources.Profile()
    })
    .AddTestUsers(new List<TestUser>
    {
        new TestUser
        {
            SubjectId = "Alice",
            Username = "alice",
            Password = "alice"
        }
    });

builder.Services.AddCors();

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();

app.UseCors(config => config
    .AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod());

app.UseRouting();

app.UseIdentityServer();

app.UseAuthorization();

app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());

app.Run();