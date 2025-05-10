using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PV260.API.BL.Mappers;
using PV260.API.BL.Options;
using PV260.API.DAL.Entities;
using PV260.API.DAL.UnitOfWork;
using PV260.Common.Models;
using System.Globalization;
using CsvHelper;
using Microsoft.Extensions.Logging;
using PV260.API.BL.Services;

namespace PV260.API.BL.Facades;

/// <inheritdoc cref="IReportFacade"/>
public class ReportFacade(
    IMapper<ReportEntity, ReportListModel, ReportDetailModel> reportMapper,
    IUnitOfWorkFactory unitOfWorkFactory,
    IOptions<ReportOptions> reportOptions,
    IOptions<EmailOptions> emailOptions,
    IEmailFacade emailFacade,
    IEmailService emailService,
    ILogger<ReportFacade> logger)
    : CrudFacadeBase<ReportListModel, ReportDetailModel, ReportEntity>(reportMapper, unitOfWorkFactory), IReportFacade
{
    private const string ReportNamePlaceholder = "{ReportName}";
    private const string ReportTimestampPlaceholder = "{ReportTimestamp}";
    private const string ReportRecordsPlaceholder = "{ReportRecords}";
    
    private readonly IMapper<ReportEntity, ReportListModel, ReportDetailModel> _reportMapper = reportMapper;
    private readonly IEmailFacade _emailFacade = emailFacade;
    private readonly IEmailService _emailService = emailService;
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
            Items = Mapper.ToListModel(currentPage),
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
    
    /// <summary>
    /// Gets the last generated report
    /// </summary>
    /// <returns>The last generated report</returns>
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

        await uow.CommitAsync();
    }

    /// <summary>
    /// Deletes reports that are older than the configured retention period.
    /// </summary>
    /// <returns>The number of reports deleted.</returns>
    public async Task<int> DeleteOldReportsAsync()
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-_reportOptions.ReportDaysToKeep);

        await using var uow = UnitOfWorkFactory.Create();
        var reports = await uow.GetRepository<ReportEntity>()
            .Get()
            .Where(r => r.CreatedAt < cutoffDate)
            .ToListAsync();

        foreach (var report in reports)
        {
            uow.GetRepository<ReportEntity>().Delete(report);
        }

        await uow.CommitAsync();
        return reports.Count;
    }

    /// <summary>
    /// Generates new report by fetching data from the specified CSV URL.
    /// </summary>
    /// <returns>New report</returns>
    public async Task<ReportDetailModel> GenerateReportAsync()
    {
        logger.LogDebug("Generating new report. Source: {ArkFundsCsvUrl}", _reportOptions.ArkFundsCsvUrl);
        
        using var httpClient = new HttpClient();
        var csvData = await httpClient.GetStringAsync(_reportOptions.ArkFundsCsvUrl);

        var reportRecords = await ParseCsvData(csvData);

        var reportDetail = CreateReportDetail(reportRecords);

        await SaveAsync(reportDetail);
        return reportDetail;
    }

    /// <inheritdoc />
    public async Task SendReportViaEmailAsync(Guid reportId)
    {
        var report = await GetAsync(reportId);
        if (report is null)
            throw new InvalidOperationException($"Report with ID {reportId} was not found.");
        
        var recipients = await _emailFacade.GetAllEmailRecipientsAsync();
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

        await _emailService.SendEmailAsync(recipients.Select(recipient => recipient.EmailAddress).ToList(), emailSubject, emailBody);
    }

    #region Report Generation

    private async Task<List<ReportRecordModel>> ParseCsvData(string csvData)
    {
        var reportRecords = new List<ReportRecordModel>();
        using var reader = new StringReader(csvData);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        await csv.ReadAsync();
        csv.ReadHeader();

        var latestReport = await GetLatestAsync();
        var latestRecords = latestReport?.Records?.ToDictionary(r => r.Ticker) ?? new Dictionary<string, ReportRecordModel>();

        var currentTickers = new HashSet<string>();

        while (await csv.ReadAsync())
        {
            if (IsEndOfRelevantData(csv))
            {
                break;
            }

            var ticker = csv.GetField<string>("ticker");
            if (ticker != null)
            {
                currentTickers.Add(ticker);
                var record = CreateReportRecord(csv, ticker, latestRecords);
                reportRecords.Add(record);
            }
        }

        AddMissingTickerRecords(latestRecords, currentTickers, reportRecords);

        return reportRecords;
    }

    private ReportRecordModel CreateReportRecord(CsvReader csv, string ticker, Dictionary<string, ReportRecordModel> latestRecords)
    {
        var sharesField = csv.GetField<string>("shares") ?? "-1";
        var numberOfShares = int.Parse(sharesField.Replace(",", ""));
        var sharesChangePercentage = numberOfShares == -1 ? 0 : CalculateSharesChangePercentage(ticker, numberOfShares, latestRecords);

        return new ReportRecordModel
        {
            CompanyName = csv.GetField<string>("company") ?? "Missing Company name",
            Ticker = ticker,
            NumberOfShares = numberOfShares,
            SharesChangePercentage = sharesChangePercentage,
            Weight = double.Parse((csv.GetField<string>("weight (%)") ?? "").Replace("%", "").Trim())
        };
    }

    private double CalculateSharesChangePercentage(string ticker, int numberOfShares, Dictionary<string, ReportRecordModel> latestRecords)
    {
        if (latestRecords.TryGetValue(ticker, out var latestRecord) && latestRecord.NumberOfShares > 0)
        {
            logger.LogDebug("Ticker: {Ticker}, Latest Shares: {LatestShares}, Current Shares: {CurrentShares}", ticker, latestRecord.NumberOfShares, numberOfShares);
            return (double)(numberOfShares - latestRecord.NumberOfShares) / latestRecord.NumberOfShares * 100;
        }
        
        return 0;
    }
    
    private static bool IsEndOfRelevantData(CsvReader csv)
    {
        return csv.Context.Parser?.RawRecord.Contains("Investors") ?? false;
    }

    private static void AddMissingTickerRecords(Dictionary<string, ReportRecordModel> latestRecords, HashSet<string> currentTickers, List<ReportRecordModel> reportRecords)
    {
        foreach (var latestTicker in latestRecords.Keys)
        {
            if (currentTickers.Contains(latestTicker))
            {
                continue;
            }

            var latestRecord = latestRecords[latestTicker];
            var record = new ReportRecordModel
            {
                CompanyName = latestRecord.CompanyName,
                Ticker = latestTicker,
                NumberOfShares = 0,
                SharesChangePercentage = -100,
                Weight = 0
            };
            reportRecords.Add(record);
        }
    }

    private ReportDetailModel CreateReportDetail(List<ReportRecordModel> reportRecords)
    {
        return new ReportDetailModel
        {
            Name = $"ARK Innovation ETF Report - {DateTime.UtcNow:yyyy-MM-dd}",
            CreatedAt = DateTime.UtcNow,
            Records = reportRecords
        };
    }
    
    #endregion
}