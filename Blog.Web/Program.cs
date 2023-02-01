using Blog.Domain.Services.Client;
using Blog.Web.Services;
using Evrane.Core.Security;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddSingleton<IJwtService, JwtService>();
builder.Services.AddHttpClient<BlogService>();
builder.Services.AddSingleton<IDomainService, DomainService>();

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

app.MapRazorPages();

app.Run();