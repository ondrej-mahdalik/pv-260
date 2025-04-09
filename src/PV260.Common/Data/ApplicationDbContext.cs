using Microsoft.EntityFrameworkCore;
using PV260.Common.Models;

namespace PV260.Common.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<ReportModel> Reports { get; set; }
    public DbSet<ReportRecordModel> ReportRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ReportModel>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasMany(e => e.Records)
                  .WithOne(e => e.Report)
                  .HasForeignKey(e => e.ReportId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ReportRecordModel>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CompanyName).IsRequired();
            entity.Property(e => e.Ticker).IsRequired();
            entity.Property(e => e.NumberOfShares).IsRequired();
            entity.Property(e => e.SharesChangePercentage).HasPrecision(18, 4);
            entity.Property(e => e.Weight).HasPrecision(18, 4);
        });
    }
}