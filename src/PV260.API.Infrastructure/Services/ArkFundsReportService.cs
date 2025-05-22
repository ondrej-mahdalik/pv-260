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
        var csvData = await httpClient.GetStringAsync(_reportOptions.ArkFundsCsvUrl);
        var latestRecords = latestReport?.Records.ToDictionary(r => r.Ticker)
                            ?? new Dictionary<string, ReportRecordModel>();
        var reportRecords = await ParseCsvDataAsync(csvData, latestRecords);
        return CreateReportDetail(reportRecords);
    }

    private async Task<List<ReportRecordModel>> ParseCsvDataAsync(string csvData, Dictionary<string, ReportRecordModel> latestRecords)
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
        csv.Context.RegisterClassMap<AkrFundsModelMap>();

        try
        {
            // Ensure header is read
            await csv.ReadAsync();
            csv.ReadHeader();

            // Read records asynchronously
            await foreach (var model in csv.GetRecordsAsync<AkrFundsModel>())
            {
                // Stop on Investors section (if row.Date contains marker)
                if (model.Date.Contains("Investors", StringComparison.OrdinalIgnoreCase))
                    break;

                // Track tickers
                var ticker = model.Ticker;
                if (string.IsNullOrWhiteSpace(ticker))
                    continue;

                // Parse numeric fields
                var sharesText = model.Shares.Replace(",", string.Empty);
                var numberOfShares = int.TryParse(sharesText, out var s) ? s : -1;
                var weightText = model.Weight.Replace("%", string.Empty).Trim();
                var weight = double.TryParse(weightText, NumberStyles.Any, CultureInfo.InvariantCulture, out var w) ? w : 0d;

                // Compute change percentage
                var changePercent = numberOfShares >= 0
                    ? CalculateSharesChangePercentage(ticker, numberOfShares, latestRecords)
                    : 0d;

                reportRecords.Add(new ReportRecordModel
                {
                    CompanyName = model.Company,
                    Ticker = ticker,
                    NumberOfShares = numberOfShares,
                    SharesChangePercentage = changePercent,
                    Weight = weight
                });
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error parsing CSV data");
            throw;
        }

        // Add any missing tickers with zero shares
        AddMissingTickerRecords(latestRecords, reportRecords.Select(r => r.Ticker).ToHashSet(), reportRecords);
        return reportRecords;
    }

    private double CalculateSharesChangePercentage(string ticker, int numberOfShares, Dictionary<string, ReportRecordModel> latestRecords)
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
        foreach (var kvp in latestRecords)
        {
            if (!currentTickers.Contains(kvp.Key))
            {
                reportRecords.Add(new ReportRecordModel
                {
                    CompanyName = kvp.Value.CompanyName,
                    Ticker = kvp.Key,
                    NumberOfShares = 0,
                    SharesChangePercentage = -100d,
                    Weight = 0d
                });
            }
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
}
