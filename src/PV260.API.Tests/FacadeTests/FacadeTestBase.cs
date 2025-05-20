using Microsoft.Extensions.Logging.Abstractions;
using PV260.API.BL.Mappers;
using PV260.API.DAL.Entities;
using PV260.API.DAL.UnitOfWork;
using PV260.API.Tests.Factories;
using PV260.API.Tests.Seeds;
using PV260.Common.Models;
using Testcontainers.MsSql;
using Microsoft.Extensions.Options;
using Moq;
using PV260.API.BL.Facades;
using PV260.API.BL.Options;
using PV260.API.BL.Services;
using PV260.API.Infrastructure.Services;
using PV260.API.Presentation.Facades;

namespace PV260.API.Tests.FacadeTests;

public abstract class FacadeTestBase : IAsyncLifetime
{
    private readonly MsSqlContainer _container = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-CU18-ubuntu-22.04")
        .Build();

    private DbContextContainerFactory? _dbContextFactory;
    
    private readonly IOptions<ReportOptions> _reportOptions = Options.Create(new ReportOptions
    {
        ReportDaysToKeep = 30,
        ReportGenerationCron = "0 0 * * *",
        OldReportCleanupCron = "0 0 * * 0",
        SendEmailOnReportGeneration = false,
        ArkFundsCsvUrl = "https://assets.ark-funds.com/fund-documents/funds-etf-csv/ARK_INNOVATION_ETF_ARKK_HOLDINGS.csv"
    });
    
    private readonly IOptions<EmailOptions> _emailOptions = Options.Create(new EmailOptions
    {
        ReportEmailSubjectTemplate = "A new report - {ReportName}",
        ReportEmailBodyTemplate = "Report {ReportName} generated at {ReportTimestamp}. \n\n{ReportRecords}",
        ReportEmailSenderEmail = "test@test.com",
        ReportEmailSenderName = "Test Sender",
        IntegrationApiKey = "test-api-key"
    });
    

    protected FacadeTestBase()
    {
        var reportRecordMapper = new ReportRecordMapper();
        ReportMapper = new ReportMapper(reportRecordMapper);
        EmailRecipientMapper = new EmailMapper();
    }

    protected UnitOfWorkFactory UnitOfWorkFactory { get; private set; } = null!;
    
    protected IMapper<ReportEntity, ReportListModel, ReportDetailModel> ReportMapper { get; }
    protected IMapper<EmailRecipientEntity, EmailRecipientModel, EmailRecipientModel> EmailRecipientMapper { get; }
    protected Mock<IEmailService> EmailServiceMock { get; } = new();

    protected IReportService ReportService { get; private set; } = null!;

    protected IEmailFacade EmailRecipientFacadeSut { get; private set; } = null!;
    protected IReportFacade ReportFacadeSut { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        _dbContextFactory = new DbContextContainerFactory(_container.GetConnectionString());
        UnitOfWorkFactory = new UnitOfWorkFactory(_dbContextFactory);
        
        ReportService = new ArkFundsReportService(new HttpClient(), _reportOptions,
            new NullLogger<ArkFundsReportService>());
        
        EmailRecipientFacadeSut = new EmailRecipientFacade(EmailRecipientMapper, UnitOfWorkFactory);
        ReportFacadeSut = new ReportFacade(ReportMapper, UnitOfWorkFactory, _reportOptions,
            _emailOptions, EmailRecipientFacadeSut, EmailServiceMock.Object, ReportService,
            new NullLogger<ReportFacade>());

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