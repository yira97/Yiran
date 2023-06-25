using System.Globalization;
using Blog.Admin;
using Blog.Admin.Middlewares;
using Blog.Admin.Services;
using Blog.Domain.Enums;
using Blog.Domain.Services.Client;
using Evrane.Core.ObjectStorage.S3;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

var logger = LoggerFactory.Create(config => { config.AddConsole(); }).CreateLogger("Main");

var builder = WebApplication.CreateBuilder(args);
// 显示 OIDC 相关的日志 (PII)
IdentityModelEventSource.ShowPII = true;

// 解除文件上传大小限制
builder.Services.Configure<KestrelServerOptions>(kestrelServerOptions =>
{
    // 15MB
    kestrelServerOptions.Limits.MaxRequestBodySize =  15_000_000;
});

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (type, factory) =>
            factory.Create(typeof(CommonResources));
    });

builder.Services.AddLocalization(opt => { opt.ResourcesPath = "Resources"; });
builder.Services.AddHttpClient<BlogService>()
    .AddUserAccessTokenHandler();
builder.Services.AddHttpClient<S3HttpClient>();
builder.Services.AddSingleton<DomainInfoMiddleware>();
builder.Services.AddSingleton<CultureInfoMiddleware>();
builder.Services.AddSingleton<CommonLocalizationService>();

builder.Services.AddAuthentication(authenticationOptions =>
    {
        authenticationOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        authenticationOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, cookieAuthenticationOptions =>
    {
        cookieAuthenticationOptions.Cookie.Name = "web";
        // automatically revoke refresh token at signout time
        cookieAuthenticationOptions.Events.OnSigningOut = async e => { await e.HttpContext.RevokeRefreshTokenAsync(); };
    })
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, openIdConnectOptions =>
    {
        openIdConnectOptions.Authority = builder.Configuration["AuthSettings:Authority"];
        logger.LogInformation("authority: {authority}", openIdConnectOptions.Authority);
        
        openIdConnectOptions.ClientId = builder.Configuration["AuthSettings:ClientId"];
        openIdConnectOptions.ClientSecret = builder.Configuration["AuthSettings:ClientSecret"];
        logger.LogInformation("clientId: {clientId}", openIdConnectOptions.ClientId);
        logger.LogInformation("clientSecret.length: {clientSecretLength}", openIdConnectOptions.ClientSecret?.Length ?? 0);
        
        openIdConnectOptions.ResponseType = "code";
        openIdConnectOptions.ResponseMode = "query";
        
        openIdConnectOptions.Scope.Clear();

        // OIDC related scopes
        openIdConnectOptions.Scope.Add("openid");
        openIdConnectOptions.Scope.Add("profile");
        openIdConnectOptions.Scope.Add("email");
        
        // API scopes
        openIdConnectOptions.Scope.Add("role");
        openIdConnectOptions.Scope.Add("https://identityserver.evrane.com/api");
        openIdConnectOptions.Scope.Add("https://blog.evrane.com/api");
        openIdConnectOptions.Scope.Add("https://blog.evrane.com/admin");
        
        // requests a refresh token
        openIdConnectOptions.Scope.Add("offline_access");
        
        openIdConnectOptions.GetClaimsFromUserInfoEndpoint = true;
        // 如果不设置，role 将会被自动映射成 http://schemas.microsoft.com/ws/2008/06/identity/claims/role
        openIdConnectOptions.MapInboundClaims = false;
        
        // important! this store the access and refresh token in the authentication session
        // this is needed to the standard token store to manage the artefacts
        openIdConnectOptions.SaveTokens = true;
        
        openIdConnectOptions.Events.OnRedirectToIdentityProvider = async context =>
        {
            context.ProtocolMessage.RedirectUri = "https://admin.blog.local.evrane.com:8100/signin-oidc";
        };
        openIdConnectOptions.Events.OnRedirectToIdentityProviderForSignOut = async context =>
        {
            context.ProtocolMessage.RedirectUri = "https://admin.blog.local.evrane.com:8100/signout-callback-oidc";
        };
        openIdConnectOptions.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = "name",
            RoleClaimType = Claims.UserRole
        };
    });

builder.Services.AddOpenIdConnectAccessTokenManagement();

builder.Services.AddAuthorization(authorizationOptions =>
{
    authorizationOptions.AddPolicy(Policies.RequireAdminRole, policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole(UserRole.Admin);
    });
    
    authorizationOptions.AddPolicy(Policies.RequireEvraneBlogApiUserScope, policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "https://blog.evrane.com/api");
    });

    authorizationOptions.AddPolicy(Policies.RequireEvraneBlogApiAdminScope, policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "https://blog.evrane.com/admin");
    });
});

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
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

    // 信任所有的网络和代理
    options.KnownNetworks.Clear(); 
    options.KnownProxies.Clear();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    // app.UseHsts();
}

if (builder.Configuration.GetValue<string>("Host:Type") == "Proxied")
{
    logger.LogInformation("Using Proxied Host");
    app.UseForwardedHeaders(); 
}

app.UseStaticFiles();

app.UseRouting();

var options = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(options!.Value);

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<CultureInfoMiddleware>();
app.UseDomainInfoMiddleware();

app.MapControllerRoute(
    name: "areaDefault",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();