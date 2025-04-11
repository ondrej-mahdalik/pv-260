namespace PV260.API.DAL.UnitOfWork;

public interface IUnitOfWorkFactory
{
    /// <summary>
    /// Creates a new instance of <see cref="IUnitOfWork"/>.
    /// </summary>
    /// <returns>A new <see cref="IUnitOfWork"/> instance.</returns>
    IUnitOfWork Create();
}