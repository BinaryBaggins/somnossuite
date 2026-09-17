using SomnosSuite.Application;
using SomnosSuite.Infrastructure;
using SomnosSuite.Persistence;
using SomnosSuite.Presentation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services
    .AddApplication() // Application Layer (CQRS, MediatR, etc.
    .AddInfrastructure() // Infrastructure Layer (Repositories, EF Core, etc.)
    .AddPersistence(builder.Configuration) // Persistence Layer (DbContext, Migrations, etc.)
    .AddPresentation(); // Presentation Layer (Controllers, DTOs, etc.)

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapControllers();

app.Run();
