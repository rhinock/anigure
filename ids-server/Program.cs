using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ids_server.Data;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

var connectStr = builder.Configuration.GetConnectionString("DefaultConnection");

var migrationAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlite(connectStr, opt => opt.MigrationsAssembly(migrationAssembly));
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddIdentityServer()
    .AddConfigurationStore(options =>
    {
        options.ConfigureDbContext = builder => builder.UseSqlite(connectStr, opt => opt.MigrationsAssembly(migrationAssembly));
    })
    .AddOperationalStore(options =>
    {
        options.ConfigureDbContext = builder => builder.UseSqlite(connectStr, opt => opt.MigrationsAssembly(migrationAssembly));
    })
    .AddAspNetIdentity<IdentityUser>();

builder.Services.AddCors();

builder.Services.AddControllersWithViews();

var app = builder.Build();

var serviceScope = app.Services.CreateScope();
var conf = serviceScope.ServiceProvider.GetService<IConfiguration>();
if (conf.GetValue("SeedData", true))
    DataSeeder.SeedIdentityServer(serviceScope.ServiceProvider);

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