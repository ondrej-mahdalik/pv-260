using Microsoft.EntityFrameworkCore;
using PV260.API.BL.Facades;
using PV260.API.BL.Mappers;
using PV260.API.DAL;
using PV260.API.DAL.Entities;
using PV260.API.DAL.UnitOfWork;
using PV260.API.Tests.Factories;
using PV260.Common.Models;

namespace PV260.API.Tests.FacadeTests;

public abstract class FacadeTestBase : IAsyncLifetime
{
    protected FacadeTestBase()
    {
        DbContextFactory = new DbContextSqliteFactory();
        UnitOfWorkFactory = new UnitOfWorkFactory(DbContextFactory);
        
        ReportRecordMapper = new ReportRecordMapper();
        ReportMapper = new ReportMapper(ReportRecordMapper);
        EmailMapper = new EmailMapper();


        ReportFacadeSut = new ReportFacade(ReportRecordMapper, ReportMapper, unitOfWorkFactory: UnitOfWorkFactory);
        EmailFacadeSut = new EmailFacade(EmailMapper, UnitOfWorkFactory);
    }

    protected IDbContextFactory<MainDbContext> DbContextFactory { get; }
    protected UnitOfWorkFactory UnitOfWorkFactory { get; }
    protected ReportRecordMapper ReportRecordMapper { get; }
    protected IMapper<ReportEntity, ReportListModel, ReportDetailModel> ReportMapper { get; }
    protected IMapper<EmailEntity, EmailRecipientModel, EmailRecipientModel> EmailMapper { get; }
    
    
    protected IEmailFacade EmailFacadeSut { get; }
    protected IReportFacade ReportFacadeSut { get; }
    
    
    public async Task InitializeAsync()
    {
        await using var dbContext = await DbContextFactory.CreateDbContextAsync();
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}