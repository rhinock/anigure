using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using anigure.Data;
using anigure.Models;
using Microsoft.OpenApi.Models;
using System.Reflection;
using anigure.Abstractions;
using anigure.Extensions;
using anigure.Helpers;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

// Add services to the container.
var connectionString = configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

services.AddDatabaseDeveloperPageExceptionFilter();

services
    .AddDefaultIdentity<ApplicationUser>(options =>
        options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

services
    .AddIdentityServer()
    .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

services
    .AddAuthentication()
    .AddIdentityServerJwt();

services.AddCors(options =>
{
    options.AddPolicy("ClientPermission", policy =>
    {
        policy.AllowAnyHeader()
            .AllowAnyMethod()
            .SetIsOriginAllowed(_ => true)
            .AllowCredentials();
    });
});

services.AddControllersWithViews();
services.AddRazorPages();

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

services
    .AddDataProtection()
    .SetApplicationName("anigure");

services.AddServices();

var app = builder.Build();

InitializeDatabase(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.UseMigrationsEndPoint();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCors("ClientPermission");

app.UseAuthentication();
app.UseIdentityServer();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");
app.MapRazorPages();

app.MapFallbackToFile("index.html");

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