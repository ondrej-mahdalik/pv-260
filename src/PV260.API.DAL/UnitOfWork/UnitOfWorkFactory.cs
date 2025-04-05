using Microsoft.EntityFrameworkCore;

namespace PV260.API.DAL.UnitOfWork;

/// <summary>
/// Factory class for creating instances of <see cref="IUnitOfWork"/>.
/// </summary>
/// <param name="dbContextFactory">The factory for creating instances of <see cref="MainDbContext"/>.</param>
public class UnitOfWorkFactory(IDbContextFactory<MainDbContext> dbContextFactory) : IUnitOfWorkFactory
{
    /// <inheritdoc />
    public IUnitOfWork Create()
    {
        return new UnitOfWork(dbContextFactory.CreateDbContext());
    }
}