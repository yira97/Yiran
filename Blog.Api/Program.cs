using System.Security.Cryptography;
using System.Text.Json;
using Blog.Api.Data;
using Blog.Domain.Enums;
using Blog.Api.Services;
using Evrane.Core.ObjectStorage;
using Evrane.Core.ObjectStorage.ServerlessImageHandlerSolution;
using Evrane.Core.Security;
using Evrane.Core.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
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

// 当模型验证失败时，返回自定义的错误信息
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        context.HttpContext.Response.Headers["X-ERROR-CODE"] = "Format.ModelInvalid";
        
        var responseObj = new
        {
            path = context.HttpContext.Request.Path.ToString(),
            method = context.HttpContext.Request.Method,
            controller = (context.ActionDescriptor as ControllerActionDescriptor)?.ControllerName,
            errors = context.ModelState.Keys.Select(k =>
            {
                return new
                {
                    field = k,
                    Messages = context.ModelState[k]?.Errors.Select(e => e.ErrorMessage)
                };
            })
        };
        
        logger.LogInformation("InvalidModelStateResponseFactory: {responseObj}", JsonSerializer.Serialize(responseObj));
        
        return new BadRequestObjectResult(responseObj);
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