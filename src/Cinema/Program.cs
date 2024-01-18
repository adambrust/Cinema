using Carter;
using Cinema.Features.Common;
using Cinema.Features.Users;
using Cinema.Persistance;
using Cinema.Services;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var assembly = Assembly.GetExecutingAssembly();
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.CustomSchemaIds(x => x.FullName));

builder.Services.AddDbContext<CinemaDbContext>(
    options => options.UseInMemoryDatabase("CinemaDB"),
    ServiceLifetime.Singleton);

builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
builder.Services.AddValidatorsFromAssembly(assembly);
builder.Services.AddCarter();

builder.Services.AddIdentityCore<User>()
    .AddEntityFrameworkStores<CinemaDbContext>()
    .AddApiEndpoints();
builder.Services.AddAuthentication()
    .AddBearerToken(IdentityConstants.BearerScheme);
builder.Services.AddAuthorizationBuilder();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapIdentityApi<User>();
app.MapCarter();

app.Run();
