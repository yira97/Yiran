using System.Globalization;
using Blog.Domain.Services.Client;
using Blog.Web.Services;
using Evrane.Core.Security;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMvc().AddViewLocalization();

// Add services to the container.
builder.Services
    .AddRazorPages(options => { options.Conventions.Add(new CultureTemplatePageRouteModelConvention()); });

builder.Services.AddSingleton<IJwtService, JwtService>();
builder.Services.AddSingleton<IDomainService, DomainService>();
builder.Services.AddSingleton<CommonLocalizationService>();


builder.Services.AddHttpClient<BlogService>();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("en"),
        new CultureInfo("ja"),
        new CultureInfo("zh"),
    };
    options.DefaultRequestCulture = new RequestCulture("en");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
    options.RequestCultureProviders.Insert(0, new RouteDataRequestCultureProvider { Options = options });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();

var localizationOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>()!.Value;
app.UseRequestLocalization(localizationOptions);

app.MapRazorPages();

app.Run();