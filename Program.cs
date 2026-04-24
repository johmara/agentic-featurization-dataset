using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using ReferenceManager.Data;
using ReferenceManager.Endpoints;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.ConfigureHttpJsonOptions(o =>
    o.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// &begin[Database]
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
// &end[Database]

var app = builder.Build();

// &begin[ApiDocs]
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => options.WithTitle("Reference Manager API"));
}
// &end[ApiDocs]

app.UseHttpsRedirection();

// &begin[Database]
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    await DbSeeder.SeedAsync(db);
}
// &end[Database]

var v1 = app.MapGroup("/api/v1"); // &line[Versioning]
v1.MapPaperEndpoints();
v1.MapCollectionEndpoints();

app.Run();

public partial class Program { }
