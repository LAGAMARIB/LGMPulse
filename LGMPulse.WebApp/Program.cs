using LGMPulse.Connections;
using LGMPulse.Connections.Helpers;
using LGMPulse.WebApp.Filters;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Globalization;
using LGMPulse.AppServices;
using LGMPulse.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor(); // Necessário para acessar a sessão no controller

// Exceptions Filter
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.AddService<LGMExceptionFilter>();
});
builder.Services.AddScoped<LGMExceptionFilter>();

// Add dependency injection services
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddScoped<SessionHelper>(); // Muito importante manter AddScoped
builder.Services.AddScoped<WebAPIHelper>();  // Também scoped por injetar SessionHelper

// Add dependency injection core and infra
builder.Services.AddAppServices();
builder.Services.AddInfraServices();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});


// Configurar opções de localização
var defaultCulture = new CultureInfo("pt-BR");
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(defaultCulture),
    SupportedCultures = new List<CultureInfo> { defaultCulture },
    SupportedUICultures = new List<CultureInfo> { defaultCulture }
};

var app = builder.Build();

// Antes de UseHttpsRedirection ou qualquer middleware que dependa de HTTPS
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
    KnownNetworks = { },
    KnownProxies = { }
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Impedir cache de arquivos estáticos críticos
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        var fileName = ctx.File.Name.ToLowerInvariant();

        if (fileName == "sw.js" || fileName == "manifest.json")
        {
            ctx.Context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
        }
        else
        {
            ctx.Context.Response.Headers["Cache-Control"] = "public, max-age=31536000, immutable";
        }
    }
});

app.UseRouting();
app.UseSession();
app.UseRequestLocalization(localizationOptions);
app.UseAuthorization();

// Initialize ConnectionSettings  
ConnectionSettings.Instance.Initialize(builder.Configuration);


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
