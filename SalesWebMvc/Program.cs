using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using SalesWebMvc.Data;
using System;
using System.Text;
using SalesWebMvc.Services;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow.PointsToAnalysis;

var builder = WebApplication.CreateBuilder(args);
var connectionStr = builder.Configuration.GetConnectionString("SalesWebMvcContext");

builder.Services.AddDbContext<SalesWebMvcContext>(options =>
options.UseMySql(connectionStr, ServerVersion.AutoDetect(connectionStr), builder => builder.MigrationsAssembly("SalesWebMvc")));

builder.Services.AddScoped<SeedingService>();
builder.Services.AddScoped<SellerService>();
builder.Services.AddScoped<DepartmentService>();

var ptBR = new CultureInfo("pt-BR");
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("pt-BR"),
    SupportedCultures = new List<CultureInfo> { ptBR },
    SupportedUICultures = new List<CultureInfo> { ptBR }
};



// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseRequestLocalization(localizationOptions);

// Configure the HTTP request pipeline.

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();

    var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<SalesWebMvcContext>();

    context.GetService<SeedingService>().Seed();

}

app.UseHttpsRedirection();
app.UseStaticFiles();


app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
