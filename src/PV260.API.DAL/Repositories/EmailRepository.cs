using PV260.API.DAL.Entities;

namespace PV260.API.DAL.Repositories;

public sealed class EmailRepository(MainDbContext context) : RepositoryBase<EmailEntity>(context);