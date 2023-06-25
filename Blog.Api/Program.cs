using System.Security.Cryptography;
using Blog.Api.Data;
using Blog.Domain.Enums;
using Blog.Api.Services;
using Evrane.Core.ObjectStorage;
using Evrane.Core.ObjectStorage.ServerlessImageHandlerSolution;
using Evrane.Core.Security;
using Evrane.Core.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddSingleton<IObjectStorageService, ServerlessImageHandlerSolution>();

var authority = builder.Configuration["AuthSettings:Authority"];
var audience = builder.Configuration["AuthSettings:Audience"];
logger.LogInformation("authority: {authority}, audience: {audience}", authority, audience);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(jwtBearerOptions =>
    {
        jwtBearerOptions.Authority = authority;
        jwtBearerOptions.Audience = audience;

        jwtBearerOptions.TokenValidationParameters.ValidateIssuer = true;
        jwtBearerOptions.TokenValidationParameters.ValidateAudience = true;
        jwtBearerOptions.TokenValidationParameters.ValidateIssuerSigningKey = true;
        jwtBearerOptions.TokenValidationParameters.ValidateLifetime = true;

        // 如果不设置，role 将会被自动映射成 http://schemas.microsoft.com/ws/2008/06/identity/claims/role
         jwtBearerOptions.MapInboundClaims = false;
        jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = "name",
            RoleClaimType = Claims.UserRole
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Policies.RequireAdminRole, policy => policy.RequireClaim(Claims.UserRole, UserRole.Admin));
});

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