using System.Security.Cryptography;
using Blog.Api.Data;
using Blog.Api.Entities;
using Blog.Domain.Enums;
using Blog.Api.Services;
using Evrane.Core.ObjectStorage;
using Evrane.Core.ObjectStorage.ServerlessImageHandlerSolution;
using Evrane.Core.Security;
using Evrane.Core.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var logger = LoggerFactory.Create(config => { config.AddConsole(); }).CreateLogger("Main");

var builder = WebApplication.CreateBuilder(args);

var connectionSettings = builder.Configuration.GetConnectionSettings();
logger.LogInformation("{Info}", connectionSettings.DebugInfo);

builder.Services
    .AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseNpgsql(connectionSettings.Postgres.ConnectionString);
    });

builder.Services.AddIdentityCore<ApplicationUserEntity>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedAccount = false;

        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireDigit = false;
        options.Password.RequireUppercase = false;
    })
    .AddRoles<ApplicationRoleEntity>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>()!;

        var pem = File.ReadAllText(jwtSettings.IssuerKeyPath);

        var rsa = new RSACryptoServiceProvider();
        rsa.ImportFromPem(pem);
        
        var p = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidAudiences = new[]
            {
                jwtSettings.Audience,
            },
            IssuerSigningKey = new RsaSecurityKey(rsa)
        };
        
        if (jwtSettings.Issuers.Count > 0)
        {
            p.ValidIssuers = jwtSettings.Issuers;
        }
        else
        {
            p.ValidIssuer = jwtSettings.Issuer;
        }

        options.TokenValidationParameters = p;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Policy.RequireAdmin,
        policyBuilder => { policyBuilder.RequireClaim(Claim.Role, new[] { Role.Admin }); });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddSingleton<IRsaCryptographyTool, RsaCryptographyTool>();
builder.Services.AddSingleton<IJwtService, JwtService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<IObjectStorageService, ServerlessImageHandlerSolution>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();