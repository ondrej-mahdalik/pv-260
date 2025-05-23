using Microsoft.EntityFrameworkCore;
using PV260.API.DAL.Entities;

namespace PV260.API.IntegrationTests.Seeds;

internal static class ReportEntitySeeds
{
    public static readonly ReportEntity Entity1 = new()
    {
        Id = new Guid("246E17CF-6594-4632-AC78-8514B8297297"),
        Name = "Test Report",
        CreatedAt = DateTime.UtcNow.AddDays(-3)
    };

    public static readonly ReportEntity Entity2 = new()
    {
        Id = new Guid("A81C2BF1-6F55-470E-B482-E96767067C6E"),
        Name = "Test Report 2",
        CreatedAt = DateTime.UtcNow.AddHours(-12)
    };

    public static readonly ReportEntity Entity3 = new()
    {
        Id = new Guid("F8D88F90-8858-40C8-B5B1-511F376FBDDD"),
        Name = "Test Report 3",
        CreatedAt = DateTime.UtcNow.AddMinutes(-15)
    };
    
    public static async Task SeedAsync(DbContext dbContext)
    {
        await dbContext.Set<ReportEntity>().AddRangeAsync(
            Entity1 with { Records = [] },
            Entity2 with { Records = [] },
            Entity3 with { Records = [] }
        );
    }
}