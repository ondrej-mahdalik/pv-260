using Microsoft.EntityFrameworkCore;
using PV260.API.DAL.Entities;

namespace PV260.API.Tests.Seeds;

internal static class EmailEntitySeeds
{
    public static readonly EmailRecipientEntity Entity1 = new()
    {
        Id = new Guid("DFD3FE56-BE05-4280-BC24-92713E7539CB"),
        CreatedAt = DateTime.UtcNow.AddDays(-2),
        EmailAddress = "test@test.com"
    };

    public static readonly EmailRecipientEntity Entity2 = new()
    {
        Id = new Guid("A276F8D4-9224-4C50-9A0D-9467C39FD8B3"),
        CreatedAt = DateTime.UtcNow.AddHours(-5),
        EmailAddress = "test2@test.com"
    };
    
    
    public static async Task SeedAsync(DbContext dbContext)
    {
        await dbContext.Set<EmailRecipientEntity>().AddRangeAsync(
            Entity1 with { },
            Entity2 with { }
        );
        await dbContext.SaveChangesAsync();
    }
}