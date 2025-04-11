using Microsoft.EntityFrameworkCore;
using PV260.API.DAL.Entities;
using PV260.Common.Models;

namespace PV260.API.DAL;

/// <summary>
/// Represents the main database context for the application.
/// </summary>
/// <param name="options">The options to configure the database context.</param>
public class MainDbContext(DbContextOptions<MainDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Gets or sets the DbSet for email entities.
    /// </summary>
    public DbSet<EmailEntity> Emails { get; set; }

    /// <summary>
    /// Gets or sets the DbSet for report entities.
    /// </summary>
    public DbSet<ReportEntity> Reports { get; set; }

    /// <summary>
    /// Gets or sets the DbSet for report record models.
    /// </summary>
    public DbSet<ReportRecordModel> ReportRecords { get; set; }

    /// <summary>
    /// Configures the model relationships and constraints.
    /// </summary>
    /// <param name="modelBuilder">The builder used to configure the model.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configures the relationship between ReportEntity and its records.
        modelBuilder.Entity<ReportEntity>()
            .HasMany(x => x.Records)
            .WithOne(x => x.Report)
            .OnDelete(DeleteBehavior.Cascade);
    }
}