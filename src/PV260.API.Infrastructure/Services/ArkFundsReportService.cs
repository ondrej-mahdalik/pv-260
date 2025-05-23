using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PV260.API.Infrastructure.Models;
using PV260.API.BL.Options;
using PV260.API.BL.Services;
using PV260.Common.Models;

namespace PV260.API.Infrastructure.Services;

/// <summary>
/// Implementation of IReportService based on ARK funds data, using CsvHelper class mapping.
/// </summary>
public class ArkFundsReportService(
    HttpClient httpClient,
    IOptions<ReportOptions> reportOptions,
    ILogger<ArkFundsReportService> logger) : IReportService
{
    private readonly ReportOptions _reportOptions = reportOptions.Value;

    /// <inheritdoc />
    public async Task<ReportDetailModel> GenerateNewReportAsync(ReportDetailModel? latestReport)
    {
        try
        {
            var csvData = await httpClient.GetStringAsync(_reportOptions.ArkFundsCsvUrl);
            var latestRecords = latestReport?.Records.ToDictionary(r => r.Ticker)
                                ?? new Dictionary<string, ReportRecordModel>();
            var reportRecords = await ParseCsvDataAsync(csvData, latestRecords);
            return CreateReportDetail(reportRecords);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error generating new report");
            throw;
        }
    }

    private async Task<List<ReportRecordModel>> ParseCsvDataAsync(string csvData,
        Dictionary<string, ReportRecordModel> latestRecords)
    {
        var reportRecords = new List<ReportRecordModel>();
        using var reader = new StringReader(csvData);
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null,
            BadDataFound = context => logger.LogWarning("Bad data {RawRecord}", context.RawRecord)
        };
        
        using var csv = new CsvReader(reader, config);
        await foreach (var model in csv.GetRecordsAsync<AkrFundsModel>())
        {
            // Stop on Investors section (if row.Date contains marker)
            if (model.Date?.Contains("Investors", StringComparison.OrdinalIgnoreCase) ?? false)
                break;

            // Track tickers
            if (string.IsNullOrWhiteSpace(model.Ticker))
                continue;

            // Compute change percentage
            var changePercent = model.NumberOfShares >= 0
                ? CalculateSharesChangePercentage(model.Ticker, model.NumberOfShares, latestRecords)
                : 0d;

            reportRecords.Add(new ReportRecordModel
            {
                CompanyName = model.Company ?? string.Empty,
                Ticker = model.Ticker,
                NumberOfShares = model.NumberOfShares,
                SharesChangePercentage = changePercent,
                Weight = model.Weight
            });
        }

        // Add any missing tickers with zero shares
        AddMissingTickerRecords(latestRecords, reportRecords.Select(r => r.Ticker).ToHashSet(), reportRecords);
        return reportRecords;
    }

    private double CalculateSharesChangePercentage(string ticker, int numberOfShares,
        Dictionary<string, ReportRecordModel> latestRecords)
    {
        if (!latestRecords.TryGetValue(ticker, out var latestRecord) || latestRecord.NumberOfShares <= 0)
            return 0d;

        logger.LogDebug("Ticker: {Ticker}, Latest Shares: {LatestShares}, Current Shares: {CurrentShares}",
            ticker, latestRecord.NumberOfShares, numberOfShares);
        return (double)(numberOfShares - latestRecord.NumberOfShares)
            / latestRecord.NumberOfShares * 100d;
    }

    private static void AddMissingTickerRecords(
        Dictionary<string, ReportRecordModel> latestRecords,
        HashSet<string> currentTickers,
        List<ReportRecordModel> reportRecords)
    {
        reportRecords.AddRange(from kvp in latestRecords
            where !currentTickers.Contains(kvp.Key)
            select new ReportRecordModel
            {
                CompanyName = kvp.Value.CompanyName,
                Ticker = kvp.Key,
                NumberOfShares = 0,
                SharesChangePercentage = -100d,
                Weight = 0d
            });
    }

    private static ReportDetailModel CreateReportDetail(List<ReportRecordModel> reportRecords)
    {
        return new ReportDetailModel
        {
            Name = $"ARK Innovation ETF Report - {DateTime.UtcNow:yyyy-MM-dd}",
            CreatedAt = DateTime.UtcNow,
            Records = reportRecords
        };
    }
}