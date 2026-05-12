using Microsoft.EntityFrameworkCore;
using SubSentry.BLL.Interfaces;
using SubSentry.BLL.Services;
using SubSentry.DAL.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 1. Get the Connection String from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Register the DbContext in the N-Tier Architecture
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    // This stops the infinite loop by ignoring the back-references
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

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

app.UseDefaultFiles(); // Looks for index.html

app.UseStaticFiles();  // Serves files from wwwroot

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    DbSeeder.Seed(context);
}

app.Run();
