using System.Security.Cryptography;
using Blog.Admin.Middlewares;
using Blog.Domain.Services.Client;
using Evrane.Core.ObjectStorage.S3;
using Evrane.Core.Security;
using Evrane.Core.Settings;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IJwtService, JwtService>();

builder.Services.AddAuthentication(options =>
    {
        // use JWT bearer scheme to deserialize and validate a JWT bearer token to construct the user's identity.
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        // use cookie authentication scheme to redirect the user to a login page.
        options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        // use cookie authentication scheme to redirect the user to a page indicating access was forbidden.
        options.DefaultForbidScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>()!;

        var pem = File.ReadAllText(jwtSettings.IssuerKeyPath);

        var rsa = new RSACryptoServiceProvider();
        rsa.ImportFromPem(pem);

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            // do not validate the lifetime
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudiences = new[]
            {
                jwtSettings.Audience,
            },
            IssuerSigningKey = new RsaSecurityKey(rsa)
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                if (ctx.Request.Cookies.ContainsKey("X-Access-Token"))
                {
                    ctx.Token = ctx.Request.Cookies["X-Access-Token"];
                }

                return Task.CompletedTask;
            }
        };
    })
    .AddCookie(options => { options.LoginPath = new PathString("/Account/Login"); });


// The typed client is registered as transient with DI container
builder.Services.AddHttpClient<BlogService>();
builder.Services.AddHttpClient<S3HttpClient>();
builder.Services.AddSingleton<AccessTokenInfoMiddleware>();
builder.Services.AddSingleton<DomainInfoMiddleware>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<AccessTokenInfoMiddleware>();
app.UseMiddleware<DomainInfoMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();