using System.Security.Cryptography;
using Blog.Api.Data;
using Blog.Api.Entities;
using Blog.Domain.Enums;
using Blog.Api.Services;
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
        options.User.RequireUniqueEmail = false;
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireNonAlphanumeric = false;
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

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudiences = new[]
            {
                jwtSettings.Audience,
            },
            IssuerSigningKey = new RsaSecurityKey(rsa)
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(nameof(Policy.RequireAdmin),
        policyBuilder => { policyBuilder.RequireRole(Role.Admin.ToString()); });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddSingleton<IRsaCryptographyTool, RsaCryptographyTool>();
builder.Services.AddSingleton<IJwtService, JwtService>();

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