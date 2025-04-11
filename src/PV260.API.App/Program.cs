using PV260.API.BL.Extensions;
using PV260.API.DAL.Extensions;
using PV260.API.DAL.Migrator;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets<Program>();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

builder.Services.AddDBlServices();
builder.Services.AddDalServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Services.AddScheduledTasks(app.Configuration);

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Set up the database
app.Services.GetRequiredService<IDbMigrator>().Migrate();

app.Run();