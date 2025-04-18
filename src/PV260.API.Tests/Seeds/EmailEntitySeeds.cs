using PV260.API.DAL;
using PV260.API.DAL.Entities;

namespace PV260.API.Tests.Seeds;

internal static class EmailEntitySeeds
{
    public static readonly List<EmailEntity> SeededEmailEntities =
    [
        new()
        {
            CreatedAt = new DateTime(2025, 4, 16, 5, 46, 01),
            EmailAddress = "test@test.com"
        },
        new()
        {
            CreatedAt = new DateTime(2025, 4, 16, 5, 51, 23),
            EmailAddress = "test2@test.com"
        }
    ];
    
    public static async Task SeedAsync(MainDbContext dbContext)
    {
        await dbContext.Emails.AddRangeAsync(SeededEmailEntities);
        await dbContext.SaveChangesAsync();
    }
}