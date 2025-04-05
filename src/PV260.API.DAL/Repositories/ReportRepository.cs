using PV260.API.DAL.Entities;

namespace PV260.API.DAL.Repositories;

public sealed class ReportRepository(MainDbContext context) : RepositoryBase<ReportEntity>(context);