using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PV260.API.BL.Options;
using PV260.API.BL.Services;
using PV260.Common.Models;

namespace PV260.API.Infrastructure.Services;

/// <summary>
/// Implementation of IReportService based on ARK funds data.
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
        
        var latestRecords = latestReport?.Records.ToDictionary(record => record.Ticker) ?? [];
        var reportRecords = await ParseCsvData(csvData, latestRecords);
        
        return CreateReportDetail(reportRecords);
    }
    
    private async Task<List<ReportRecordModel>> ParseCsvData(string csvData,Dictionary<string, ReportRecordModel> latestRecords)
    {
        var reportRecords = new List<ReportRecordModel>();
        using var reader = new StringReader(csvData);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null
        });
        
        await csv.ReadAsync();
        csv.ReadHeader();
        
        var currentTickers = new HashSet<string>();

        while (await csv.ReadAsync())
        {
            if (IsEndOfRelevantData(csv))
            {
                break;
            }

            var ticker = csv.GetField<string>("ticker");
            if (ticker == null) continue;
            currentTickers.Add(ticker);
            var record = CreateReportRecord(csv, ticker, latestRecords);
            reportRecords.Add(record);
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
        if (!latestRecords.TryGetValue(ticker, out var latestRecord) || latestRecord.NumberOfShares <= 0)
            return 0;
        
        logger.LogDebug("Ticker: {Ticker}, Latest Shares: {LatestShares}, Current Shares: {CurrentShares}", ticker, latestRecord.NumberOfShares, numberOfShares);
        return (double)(numberOfShares - latestRecord.NumberOfShares) / latestRecord.NumberOfShares * 100;

    }
    
    private static bool IsEndOfRelevantData(CsvReader csv)
    {
        return csv.Context.Parser?.RawRecord.Contains("Investors") ?? false;
    }
    
    private static void AddMissingTickerRecords(Dictionary<string, ReportRecordModel> latestRecords, HashSet<string> currentTickers, List<ReportRecordModel> reportRecords)
    {
        reportRecords.AddRange(from latestTicker in latestRecords.Keys
            where !currentTickers.Contains(latestTicker)
            let latestRecord = latestRecords[latestTicker]
            select new ReportRecordModel
            {
                CompanyName = latestRecord.CompanyName,
                Ticker = latestTicker,
                NumberOfShares = 0,
                SharesChangePercentage = -100,
                Weight = 0
            });
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