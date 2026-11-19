using System.Text.Json.Serialization;
using Api.Features.Registrations;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Features.Registrations.CreateRegistration;
using Domain.Common;
using FluentValidation;
using Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Database (SQLite file: carshow.db)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

// Make the file path absolute so carshow.db is always created inside the Api folder,
// no matter which folder the command is run from.
var sqliteBuilder = new SqliteConnectionStringBuilder(connectionString);
sqliteBuilder.DataSource = Path.Combine(builder.Environment.ContentRootPath, sqliteBuilder.DataSource);

builder.Services.AddDbContext<IAppDbContext, AppDbContext>(options =>
    options.UseSqlite(sqliteBuilder.ConnectionString));

// JSON: enums are sent and received as strings ("Competitor"), not numbers
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false));
});

// CORS: allow the React dev server
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

// Validators
builder.Services.AddValidatorsFromAssembly(typeof(IAppDbContext).Assembly);

// Handlers
builder.Services.AddScoped<ICommandHandler<CreateRegistrationCommand, Result<CreateRegistrationResponse>>, CreateRegistrationCommandHandler>();
// builder.Services.AddScoped<IQueryHandler<TQuery, Result<TResponse>>, TQueryHandler>();

// API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Info.Title = "Vertical Slice API Template by Elvin Manuel R. Luces, MIT";
        document.Info.Description = "A simple API demonstrating Vertical Slice Architecture";
        return Task.CompletedTask;
    });
});

var app = builder.Build();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // dbContext.<Entities>.AddRange(...);

    dbContext.SaveChanges();
}

app.UseCors("Frontend");

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// Root endpoint
app.MapGet("/", () => "Welcome to the Vertical Slice API! Go to /scalar/v1 for API documentation.");

// Map endpoints
app.MapRegistrationEndpoints();

app.Run();
