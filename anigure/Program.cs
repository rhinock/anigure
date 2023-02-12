using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using anigure.Data;
using anigure.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

// builder.Services.AddDbContext<ApplicationDbContext>(options =>
// options.UseSqlite(connectionString));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

var authorityUrl = builder.Configuration.GetSection("Authority").Get<string>() ??
                   throw new InvalidOperationException("'authorityUrl' not found.");

// builder.Services.AddIdentityServer()
builder.Services.AddIdentityServer(options => 
    options.IssuerUri = authorityUrl)
    .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

builder.Services.AddAuthentication()
    .AddIdentityServerJwt();

// builder.Services.AddCors();
// builder.Services.AddCors(options =>
// {
//     options.AddDefaultPolicy(builder =>
//     {
//         builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().SetIsOriginAllowed(_ => true);
//     });
// });
builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientPermission", policy =>
    {
        policy.AllowAnyHeader()
            .AllowAnyMethod()
            .SetIsOriginAllowed(_ => true)
            .AllowCredentials();
    });
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.Configure<JwtBearerOptions>(
    "IdentityServerJwtBearer",
    o => o.Authority = authorityUrl);
// where 44494 is the port for the spa proxy.

builder.Services.AddSwaggerGen(options =>
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

// builder.Services.AddDataProtection()
//     .PersistKeysToFileSystem(new DirectoryInfo(Environment.CurrentDirectory))
//     .SetDefaultKeyLifetime(TimeSpan.FromDays(36500));

builder.Services.AddDataProtection().SetApplicationName("anigure");

var app = builder.Build();

using (var scope = app.Services.CreateScope())
using (var context = scope.ServiceProvider.GetService<ApplicationDbContext>())
    context?.Database.Migrate();

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

// var options = new ForwardedHeadersOptions
// {
//     ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
// };
// options.KnownNetworks.Clear();
// options.KnownProxies.Clear();
// app.UseForwardedHeaders(options); // https://stackoverflow.com/questions/43267113/how-to-configure-usecookieauthentication-behind-a-load-balancer

app.UseCors("ClientPermission");
// app.UseCors();
// app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
// app.UseCors(x => x
//     .AllowAnyHeader()
//     .AllowAnyMethod()
//     .AllowCredentials()
//     .WithOrigins(
//         "http://localhost:13768",
//         "https://localhost:44382",
//         "https://localhost:7146",
//         "http://localhost:5100",
//         authorityUrl));

app.UseAuthentication();
app.UseIdentityServer();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");
app.MapRazorPages();

app.MapFallbackToFile("index.html");

app.Run();