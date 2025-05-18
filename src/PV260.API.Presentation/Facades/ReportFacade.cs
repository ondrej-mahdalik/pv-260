using System.Globalization;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PV260.API.BL.Facades;
using PV260.API.BL.Mappers;
using PV260.API.BL.Options;
using PV260.API.BL.Services;
using PV260.API.DAL.Entities;
using PV260.API.DAL.UnitOfWork;
using PV260.Common.Models;

namespace PV260.API.Presentation.Facades;

/// <inheritdoc cref="IReportFacade"/>
public class ReportFacade(
    IMapper<ReportEntity, ReportListModel, ReportDetailModel> reportMapper,
    IUnitOfWorkFactory unitOfWorkFactory,
    IOptions<ReportOptions> reportOptions,
    IOptions<EmailOptions> emailOptions,
    IEmailFacade emailFacade,
    IEmailService emailService,
    IReportService reportService,
    ILogger<ReportFacade> logger)
    : CrudFacadeBase<ReportListModel, ReportDetailModel, ReportEntity>(reportMapper, unitOfWorkFactory), IReportFacade
{
    private const string ReportNamePlaceholder = "{ReportName}";
    private const string ReportTimestampPlaceholder = "{ReportTimestamp}";
    private const string ReportRecordsPlaceholder = "{ReportRecords}";
    
    private readonly IMapper<ReportEntity, ReportListModel, ReportDetailModel> _reportMapper = reportMapper;
    private readonly ReportOptions _reportOptions = reportOptions.Value;
    private readonly EmailOptions _emailOptions = emailOptions.Value;

    public async Task<PaginatedResponse<ReportListModel>> GetAsync(PaginationCursor paginationCursor)
    {
        await using var uow = UnitOfWorkFactory.Create();
        var repository = uow.GetRepository<ReportEntity>();

        const int nextPageOffset = 1;

        var isFirstPage = paginationCursor.LastCreatedAt is null && paginationCursor.LastId is null;

        var reportEntities = isFirstPage
            ? await repository
                .Get()
                .OrderByDescending(report => report.CreatedAt)
                .ThenBy(report => report.Id)
                .Take(paginationCursor.PageSize + nextPageOffset)
                .ToListAsync()
            : await repository
                .Get()
                .OrderByDescending(report => report.CreatedAt)
                .ThenBy(report => report.Id)
                .Where(report =>
                    report.CreatedAt < paginationCursor.LastCreatedAt ||
                    (report.CreatedAt == paginationCursor.LastCreatedAt && report.Id > paginationCursor.LastId))
                .Take(paginationCursor.PageSize + nextPageOffset)
                .ToListAsync();

        var hasNextPage = reportEntities.Count > paginationCursor.PageSize;

        var currentPage = reportEntities.Take(paginationCursor.PageSize).ToList();

        var last = currentPage.LastOrDefault();

        return new PaginatedResponse<ReportListModel>
        {
            Items = Mapper.ToListModel(currentPage).ToList(),
            PageSize = paginationCursor.PageSize,
            TotalCount = await repository.Get().CountAsync(),
            NextCursor = hasNextPage && last is not null
                ? new PaginationCursor
                {
                    LastCreatedAt = last.CreatedAt,
                    LastId = last.Id,
                    PageSize = paginationCursor.PageSize
                }
                : null
        };
    }

    /// <inheritdoc />
    public override async Task SaveAsync(ReportDetailModel model)
    {
        await using var unitOfWork = UnitOfWorkFactory.Create();
        var reportRepository = unitOfWork.GetRepository<ReportEntity>();
        var reportRecordRepository = unitOfWork.GetRepository<ReportRecordEntity>();

        // Delete existing report records
        var existingReportRecords = await reportRecordRepository.Get().Where(x => x.ReportId == model.Id).ToListAsync();
        reportRecordRepository.DeleteRange(existingReportRecords);

        // Save the report
        var report = _reportMapper.ToEntity(model);
        await reportRepository.AddOrUpdateAsync(report);

        await unitOfWork.CommitAsync();
    }
    
    /// <inheritdoc />
    public async Task<ReportDetailModel?> GetLatestAsync()
    {
        await using var uow = UnitOfWorkFactory.Create();
        var report = await uow.GetRepository<ReportEntity>()
            .Get()
            .OrderByDescending(r => r.CreatedAt)
            .FirstOrDefaultAsync();

        return report is null ? null : _reportMapper.ToDetailModel(report);
    }

    /// <inheritdoc />
    public async Task DeleteAllAsync()
    {
        await using var uow = UnitOfWorkFactory.Create();
        var reports = await uow.GetRepository<ReportEntity>()
            .Get()
            .ToListAsync();

        foreach (var report in reports)
            uow.GetRepository<ReportEntity>().Delete(report);
        
        logger.LogInformation("Deleted all ({Count}) reports", reports.Count);

        await uow.CommitAsync();
    }

    /// <inheritdoc />
    public async Task<int> DeleteOldReportsAsync()
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-_reportOptions.ReportDaysToKeep);

        await using var uow = UnitOfWorkFactory.Create();
        var reports = await uow.GetRepository<ReportEntity>()
            .Get()
            .Where(r => r.CreatedAt < cutoffDate)
            .ToListAsync();
        
        uow.GetRepository<ReportEntity>().DeleteRange(reports);
        await uow.CommitAsync();
        
        logger.LogInformation("Deleted {Count} old reports older than {CutoffDate}", reports.Count, cutoffDate);
        
        return reports.Count;
    }

    /// <inheritdoc />
    public async Task<ReportDetailModel> GenerateReportAsync()
    {
        var latestReport = await GetLatestAsync();
        var report = await reportService.GenerateNewReportAsync(latestReport);
        await SaveAsync(report);
        
        logger.LogInformation("Generated new report with ID {ReportId}", report.Id);
        
        if (_reportOptions.SendEmailOnReportGeneration)
        {
            await SendReportViaEmailAsync(report.Id);
            logger.LogInformation("Sent report with ID {ReportId} via email", report.Id);
        }
        
        return report;
    }

    /// <inheritdoc />
    public async Task SendReportViaEmailAsync(Guid reportId)
    {
        var report = await GetAsync(reportId);
        if (report is null)
            throw new InvalidOperationException($"Report with ID {reportId} was not found.");
        
        var recipients = await emailFacade.GetAllEmailRecipientsAsync();
        if (recipients.Count == 0)
            return;

        var recordsText = report.Records.Aggregate(string.Empty,
            (current, record) =>
                current +
                $"{record.CompanyName} ({record.Ticker}): weight {record.Weight}, {record.NumberOfShares} shares, {record.SharesChangePercentage}% change\n");

        var emailSubject = _emailOptions.ReportEmailSubjectTemplate
            .Replace(ReportNamePlaceholder, report.Name)
            .Replace(ReportTimestampPlaceholder, report.CreatedAt.ToString("g"));
        
        var emailBody = _emailOptions.ReportEmailBodyTemplate
            .Replace(ReportNamePlaceholder, report.Name)
            .Replace(ReportTimestampPlaceholder, report.CreatedAt.ToString("g"))
            .Replace(ReportRecordsPlaceholder, recordsText);

        await emailService.SendEmailAsync(recipients.Select(recipient => recipient.EmailAddress).ToList(), emailSubject, emailBody);
    }
}