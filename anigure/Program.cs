using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using anigure.Data;
using anigure.Models;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Security.Claims;
using anigure.Abstractions;
using anigure.Extensions;
using anigure.Helpers;
using anigure.Implementations;
// using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

// Add services to the container.
var connectionString = configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

if (builder.Environment.IsDevelopment())
{
    services.AddDatabaseDeveloperPageExceptionFilter();
}

services
    .AddDefaultIdentity<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

services
    .AddIdentityServer()
    .AddApiAuthorization<ApplicationUser, ApplicationDbContext>()
    .AddProfileService<ProfileService>();

services
    .AddAuthentication()
    .AddIdentityServerJwt();

services.AddAuthorization();

//services.AddAuthorization(options =>
//{
//    options.AddPolicy("IsAdmin",
//        policy =>
//        {
//            // Even though we are using JwtClaimTypes in the ProfileService of the IdentityServer
//            // the actual user claims are converted to those in ClaimTypes so check for them here
//            policy.RequireClaim(ClaimTypes.Role, RoleHelpers.Admin);
//        });
//});

// services.AddCors(options =>
// {
//     options.AddPolicy("ClientPermission", policy =>
//     {
//         policy.AllowAnyHeader()
//             .AllowAnyMethod()
//             .SetIsOriginAllowed(_ => true)
//             .AllowCredentials();
//     });
// });

services.AddControllersWithViews();
services.AddRazorPages();

// In production, the React files will be served from this directory
services.AddSpaStaticFiles(c =>
{
    // c.RootPath = "ClientApp/build";
    // c.RootPath = Path.Join(builder.Environment.ContentRootPath, "ClientApp");
    c.RootPath = Path.Combine(Directory.GetCurrentDirectory(), "ClientApp");
});

services.AddServices();

services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Anigure"
    });

    // using System.Reflection;
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// services
//     .AddDataProtection()
//     .SetApplicationName("anigure");

var app = builder.Build();

InitializeDatabase(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSpaStaticFiles();

app.UseRouting();

// app.UseCors("ClientPermission");

app.UseAuthentication();
app.UseIdentityServer();
app.UseAuthorization();

// app.MapControllerRoute(name: "default",
//     pattern: "{controller}/{action=Index}/{id?}");
//
// app.MapRazorPages();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller}/{action=Index}/{id?}");
    endpoints.MapRazorPages();
});

app.UseSpa(spa =>
{
    // spa.Options.SourcePath = "ClientApp";
    // spa.Options.SourcePath = Path.Join(app.Environment.ContentRootPath, "ClientApp");
    spa.Options.SourcePath = Path.Combine(Directory.GetCurrentDirectory(), "ClientApp");

    if (app.Environment.IsDevelopment())
    {
        spa.UseReactDevelopmentServer(npmScript: "start");
    }
});

app.Run();

static async void InitializeDatabase(IHost app)
{
    using var serviceScope = app.Services.CreateScope();
    var services = serviceScope.ServiceProvider;
    await using var context = services.GetService<ApplicationDbContext>();

    ArgumentNullException.ThrowIfNull(context);

    if (!context.AllMigrationsApplied())
    {
        context.Database.Migrate();
    }

    var userManagementService = services.GetRequiredService<IUserManagementService>();
    var usersCount = await userManagementService.GetUsersCountAsync(RoleHelpers.Admin);

    if (usersCount != 0)
    {
        return;
    }

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    foreach (var roleName in RoleHelpers.RoleNames)
    {
        var identityRole = new IdentityRole(roleName);
        await roleManager.CreateAsync(identityRole);
    }

    var adminUser = new ApplicationUser
    {
        UserName = "admin@anigure.com",
        Email = "admin@anigure.com",
        EmailConfirmed = true
    };

    await userManagementService.AddUserAsync(
        adminUser,
        "P@ssw0rd",
        RoleHelpers.Admin);
}