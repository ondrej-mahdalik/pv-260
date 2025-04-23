using PV260.Client.BL;
using PV260.Common.Models;

namespace PV260.Client.Mock;

public class ApiClientMock : IApiClient
{
    private readonly List<ReportDetailModel> _reports =
    [
        new()
        {
            Id = Guid.NewGuid(),
            Name = "Q1 2025 Performance",
            CreatedAt = new DateTime(2025, 4, 1),
            Records =
            [
                new ReportRecordModel
                {
                    Id = Guid.NewGuid(),
                    CompanyName = "Contoso Ltd",
                    Ticker = "CTSO",
                    NumberOfShares = 1500,
                    SharesChangePercentage = 3.5,
                    Weight = 12.4
                },
                new ReportRecordModel
                {
                    Id = Guid.NewGuid(),
                    CompanyName = "Fabrikam Inc",
                    Ticker = "FBKM",
                    NumberOfShares = 2300,
                    SharesChangePercentage = -1.2,
                    Weight = 8.9
                }
            ]
        },
        new()
        {
            Id = Guid.NewGuid(),
            Name = "Q4 2024 Overview",
            CreatedAt = new DateTime(2025, 1, 15),
            Records =
            [
                new ReportRecordModel
                {
                    Id = Guid.NewGuid(),
                    CompanyName = "Northwind Traders",
                    Ticker = "NWTR",
                    NumberOfShares = 1800,
                    SharesChangePercentage = 0.75,
                    Weight = 10.2
                },
                new ReportRecordModel
                {
                    Id = Guid.NewGuid(),
                    CompanyName = "Lucerne Publishing",
                    Ticker = "LCPB",
                    NumberOfShares = 1200,
                    SharesChangePercentage = 2.0,
                    Weight = 7.3
                }
            ]
        },
        new()
        {
            Id = Guid.NewGuid(),
            Name = "Annual Summary 2024",
            CreatedAt = new DateTime(2025, 1, 5),
            Records =
            [
                new ReportRecordModel
                {
                    Id = Guid.NewGuid(),
                    CompanyName = "Adventure Works",
                    Ticker = "ADWK",
                    NumberOfShares = 3100,
                    SharesChangePercentage = 4.2,
                    Weight = 15.6
                },
                new ReportRecordModel
                {
                    Id = Guid.NewGuid(),
                    CompanyName = "Tailspin Toys",
                    Ticker = "TLSP",
                    NumberOfShares = 950,
                    SharesChangePercentage = -0.5,
                    Weight = 5.1
                }
            ]
        },
        new()
        {
            Id = Guid.NewGuid(),
            Name = "Mid-Year Insights 2024",
            CreatedAt = new DateTime(2024, 7, 1),
            Records =
            [
                new ReportRecordModel
                {
                    Id = Guid.NewGuid(),
                    CompanyName = "Woodgrove Bank",
                    Ticker = "WDBG",
                    NumberOfShares = 2000,
                    SharesChangePercentage = 1.1,
                    Weight = 11.0
                },
                new ReportRecordModel
                {
                    Id = Guid.NewGuid(),
                    CompanyName = "Alpine Ski House",
                    Ticker = "ALSK",
                    NumberOfShares = 1100,
                    SharesChangePercentage = 0.3,
                    Weight = 6.2
                },
                new ReportRecordModel
                {
                    Id = Guid.NewGuid(),
                    CompanyName = "Blue Yonder Airlines",
                    Ticker = "BYAL",
                    NumberOfShares = 1700,
                    SharesChangePercentage = 2.9,
                    Weight = 9.8
                }
            ]
        }
    ];

    public Task<IEnumerable<ReportListModel>> GetAllReportsAsync()
    {
        var listModels = _reports.Select(r => new ReportListModel
        {
            Id = r.Id,
            CreatedAt = r.CreatedAt,
            Name = r.Name
        }).ToList();

        return Task.FromResult<IEnumerable<ReportListModel>>(listModels);
    }

    public Task<ReportDetailModel?> GetReportByIdAsync(Guid id)
    {
        var report = _reports.FirstOrDefault(r => r.Id == id);
        return Task.FromResult(report);
    }

    public Task<ReportDetailModel?> GetLatestReportAsync()
    {
        var report = _reports.OrderByDescending(r => r.CreatedAt).FirstOrDefault();
        return Task.FromResult(report);
    }

    public Task<ReportDetailModel> GenerateNewReportAsync()
    {
        var newReport = new ReportDetailModel { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, Name = "New Report" };
        _reports.Add(newReport);
        return Task.FromResult(newReport);
    }

    public Task DeleteReportAsync(Guid id)
    {
        var report = _reports.FirstOrDefault(r => r.Id == id);
        if (report != null)
        {
            _reports.Remove(report);
        }

        return Task.CompletedTask;
    }

    public Task DeleteAllReportsAsync()
    {
        _reports.Clear();
        return Task.CompletedTask;
    }

    public Task SendReportAsync(Guid id)
    {
        return Task.CompletedTask;
    }
}