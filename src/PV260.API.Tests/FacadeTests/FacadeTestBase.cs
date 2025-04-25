using Microsoft.Extensions.Logging.Abstractions;
using PV260.API.BL.Facades;
using PV260.API.BL.Mappers;
using PV260.API.DAL.Entities;
using PV260.API.DAL.UnitOfWork;
using PV260.API.Tests.Factories;
using PV260.API.Tests.Seeds;
using PV260.Common.Models;
using Testcontainers.MsSql;
using Microsoft.Extensions.Options;
using PV260.API.BL.Options;

namespace PV260.API.Tests.FacadeTests;

public abstract class FacadeTestBase : IAsyncLifetime
{
    private readonly MsSqlContainer _container = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-CU18-ubuntu-22.04")
        .Build();
    private DbContextContainerFactory? _dbContextFactory;

    protected FacadeTestBase()
    {
        ReportRecordMapper = new ReportRecordMapper();
        ReportMapper = new ReportMapper(ReportRecordMapper);
        EmailRecipientMapper = new EmailMapper();
    }

    protected UnitOfWorkFactory UnitOfWorkFactory { get; private set; } = null!;
    protected ReportRecordMapper ReportRecordMapper { get; }
    protected IMapper<ReportEntity, ReportListModel, ReportDetailModel> ReportMapper { get; }
    protected IMapper<EmailRecipientEntity, EmailRecipientModel, EmailRecipientModel> EmailRecipientMapper { get; }

    protected IEmailFacade EmailFacadeSut { get; private set; } = null!;
    protected IReportFacade ReportFacadeSut { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        _dbContextFactory = new DbContextContainerFactory(_container.GetConnectionString());
        UnitOfWorkFactory = new UnitOfWorkFactory(_dbContextFactory);

        var reportOptions = Options.Create(new ReportOptions
        {
            ReportDaysToKeep = 30,
            ReportGenerationCron = "0 0 * * *",
            OldReportCleanupCron = "0 0 * * 0",
            SendEmailOnReportGeneration = false,
            ArkFundsCsvUrl = "https://assets.ark-funds.com/fund-documents/funds-etf-csv/ARK_INNOVATION_ETF_ARKK_HOLDINGS.csv"
        });

        ReportFacadeSut = new ReportFacade(ReportMapper, UnitOfWorkFactory, reportOptions, new NullLogger<ReportFacade>());
        EmailFacadeSut = new EmailFacade(EmailRecipientMapper, UnitOfWorkFactory);

        await using var dbContext = _dbContextFactory.CreateDbContext();
        await dbContext.Database.EnsureCreatedAsync();
        
        // Seed data
        await EmailEntitySeeds.SeedAsync(dbContext);
        await ReportEntitySeeds.SeedAsync(dbContext);
        await ReportRecordEntitySeeds.SeedAsync(dbContext);
        await dbContext.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}