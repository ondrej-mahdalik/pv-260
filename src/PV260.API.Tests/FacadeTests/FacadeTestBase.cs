using Microsoft.EntityFrameworkCore;
using PV260.API.BL.Facades;
using PV260.API.BL.Mappers;
using PV260.API.DAL;
using PV260.API.DAL.Entities;
using PV260.API.DAL.UnitOfWork;
using PV260.API.Tests.Factories;
using PV260.API.Tests.Seeds;
using PV260.Common.Models;
using Testcontainers.MsSql;

namespace PV260.API.Tests.FacadeTests;

public abstract class FacadeTestBase : IAsyncLifetime
{
    private readonly MsSqlContainer _container = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();
    private IDbContextFactory<MainDbContext>? _dbContextFactory;

    protected FacadeTestBase()
    {
        ReportRecordMapper = new ReportRecordMapper();
        ReportMapper = new ReportMapper(ReportRecordMapper);
        EmailMapper = new EmailMapper();
    }

    protected UnitOfWorkFactory UnitOfWorkFactory { get; private set; } = null!;
    protected ReportRecordMapper ReportRecordMapper { get; }
    protected IMapper<ReportEntity, ReportListModel, ReportDetailModel> ReportMapper { get; }
    protected IMapper<EmailEntity, EmailRecipientModel, EmailRecipientModel> EmailMapper { get; }

    protected IEmailFacade EmailFacadeSut { get; private set; } = null!;
    protected IReportFacade ReportFacadeSut { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        _dbContextFactory = new DbContextContainerFactory(_container.GetConnectionString());
        UnitOfWorkFactory = new UnitOfWorkFactory(_dbContextFactory);

        ReportFacadeSut = new ReportFacade(ReportRecordMapper, ReportMapper, UnitOfWorkFactory);
        EmailFacadeSut = new EmailFacade(EmailMapper, UnitOfWorkFactory);

        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await dbContext.Database.EnsureCreatedAsync();
        
        // Seed data
        await EmailEntitySeeds.SeedAsync(dbContext);
        await ReportEntitySeeds.SeedAsync(dbContext);
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
    
    protected List<EmailRecipientModel> EmailRecipientModelSeeds =>
        EmailEntitySeeds.SeededEmailEntities.Select(x => EmailMapper.ToDetailModel(x)).ToList();
    
    protected List<ReportDetailModel> ReportDetailModelSeeds =>
        ReportEntitySeeds.SeededReportEntities.Select(x => ReportMapper.ToDetailModel(x)).ToList();
    
    protected List<ReportListModel> ReportListModelSeeds =>
        ReportEntitySeeds.SeededReportEntities.Select(x => ReportMapper.ToListModel(x)).ToList();
}