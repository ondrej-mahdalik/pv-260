using PV260.API.BL.Extensions;
using PV260.API.BL.Options;
using PV260.API.DAL.Extensions;
using PV260.API.DAL.Migrator;
using PV260.API.DAL.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<DalOptions>(builder.Configuration.GetSection("DalSettings"));
builder.Services.Configure<ReportOptions>(builder.Configuration.GetSection("ReportSettings"));
builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection("EmailSettings"));

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

builder.Services.AddDalServices();
builder.Services.AddDBlServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Services.AddScheduledTasks();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Set up the database
app.Services.GetRequiredService<IDbMigrator>().Migrate();

app.Run();