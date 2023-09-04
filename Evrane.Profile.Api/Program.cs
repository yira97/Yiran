using Evrane.Core.Settings;
using Evrane.Profile.Api.Data;
using Microsoft.EntityFrameworkCore;

var logger = LoggerFactory.Create(config => { config.AddConsole(); }).CreateLogger("Main");


var builder = WebApplication.CreateBuilder(args);

var connectionSettings = builder.Configuration.GetConnectionSettings();
logger.LogInformation("{Info}", connectionSettings.DebugInfo);

builder.Services
    .AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseNpgsql(connectionSettings.Postgres.ConnectionString);
    });

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();