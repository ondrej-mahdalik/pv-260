using Microsoft.EntityFrameworkCore;
using PV260.API.DAL.Entities;

namespace PV260.API.IntegrationTests.Seeds;

internal static class ReportRecordEntitySeeds
{
    public static readonly ReportRecordEntity Entity1 = new()
    {
        Id = new Guid("939AB018-3641-4E18-8076-2084163914A7"),
        CompanyName = "Test Company",
        Ticker = "Test Ticker",
        Weight = 3.5,
        NumberOfShares = 300,
        SharesChangePercentage = 0.3,
        ReportId = ReportEntitySeeds.Entity1.Id,
        Report = ReportEntitySeeds.Entity1
    };

    public static readonly ReportRecordEntity Entity2 = new()
    {
        Id = new Guid("6C337768-D95F-47EA-B34F-DE023D79424B"),
        CompanyName = "Test Company 2",
        Ticker = "Test Ticker 2",
        Weight = 2.1,
        NumberOfShares = 210,
        SharesChangePercentage = 0.6,
        ReportId = ReportEntitySeeds.Entity1.Id,
        Report = ReportEntitySeeds.Entity1
    };

    public static readonly ReportRecordEntity Entity3 = new()
    {
        Id = new Guid("F514E2C1-0BBD-450C-9878-46427D6A6D21"),
        CompanyName = "Test Company 3",
        Ticker = "Test Ticker 3",
        Weight = 1.5,
        NumberOfShares = 150,
        SharesChangePercentage = 0.1,
        ReportId = ReportEntitySeeds.Entity2.Id,
        Report = ReportEntitySeeds.Entity2
    };

    public static async Task SeedAsync(DbContext dbContext)
    {
        await dbContext.Set<ReportRecordEntity>().AddRangeAsync(
            Entity1 with {Report = null},
            Entity2 with {Report = null},
            Entity3 with {Report = null}
        );
    }
}