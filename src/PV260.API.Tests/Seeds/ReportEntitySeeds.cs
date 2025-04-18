using PV260.API.DAL;
using PV260.API.DAL.Entities;

namespace PV260.API.Tests.Seeds;

internal static class ReportEntitySeeds
{
    public static readonly List<ReportEntity> SeededReportEntities =
    [
        new()
        {
            Name = "Test Report",
            CreatedAt = new DateTime(2025, 4, 12, 11, 13, 57),
            Records =
            [
                new()
                {
                    CompanyName = "Test Company",
                    Ticker = "Test Ticker",
                    Weight = 3.5,
                    NumberOfShares = 300,
                    SharesChangePercentage = 0.3
                },
                new()
                {
                    CompanyName = "Test Company 2",
                    Ticker = "Test Ticker 2",
                    Weight = 2.1,
                    NumberOfShares = 210,
                    SharesChangePercentage = 0.6
                }
            ]
        },
        new()
        {
            Name = "Test Report 2",
            CreatedAt = new DateTime(2025, 4, 13, 2, 12, 22),
            Records =
            [
                new()
                {
                    CompanyName = "Test Company 3",
                    Ticker = "Test Ticker 3",
                    Weight = 1.5,
                    NumberOfShares = 150,
                    SharesChangePercentage = 0.1
                }
            ]
        },
        new()
        {
            Name = "Test Report 3",
            CreatedAt = new DateTime(2025, 4, 18, 16, 43, 22),
            Records = []
        },
    ];
    
    public static async Task SeedAsync(MainDbContext dbContext)
    {
        await dbContext.Reports.AddRangeAsync(SeededReportEntities);
        await dbContext.SaveChangesAsync();
    }
}