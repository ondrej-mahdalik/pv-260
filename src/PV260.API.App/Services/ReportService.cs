using Microsoft.EntityFrameworkCore;
using PV260.Common.Data;
using PV260.Common.Models;

namespace PV260.API.App.Services;

public class ReportService
{
    private readonly ApplicationDbContext _context;

    public ReportService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ReportModel>> GetAllReportsAsync()
    {
        return await _context.Reports
            .Include(r => r.Records)
            .ToListAsync();
    }

    public async Task<ReportModel?> GetReportByIdAsync(int id)
    {
        return await _context.Reports
            .Include(r => r.Records)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<ReportModel> CreateReportAsync(ReportModel report)
    {
        report.CreatedAt = DateTime.UtcNow;

        _context.Reports.Add(report);
        await _context.SaveChangesAsync();
        return report;
    }

    public async Task<ReportModel?> UpdateReportAsync(int id, ReportModel updatedReport)
    {
        var report = await _context.Reports
            .Include(r => r.Records)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (report == null)
            return null;

        report.Name = updatedReport.Name;

        // Update existing records and add new ones
        foreach (var updatedRecord in updatedReport.Records)
        {
            var existingRecord = report.Records.FirstOrDefault(r => r.Id == updatedRecord.Id);
            if (existingRecord != null)
            {
                existingRecord.CompanyName = updatedRecord.CompanyName;
                existingRecord.Ticker = updatedRecord.Ticker;
                existingRecord.NumberOfShares = updatedRecord.NumberOfShares;
                existingRecord.SharesChangePercentage = updatedRecord.SharesChangePercentage;
                existingRecord.Weight = updatedRecord.Weight;
            }
            else
            {
                report.Records.Add(updatedRecord);
            }
        }

        await _context.SaveChangesAsync();
        return report;
    }

    public async Task<bool> DeleteReportAsync(int id)
    {
        var report = await _context.Reports.FindAsync(id);
        if (report == null)
            return false;

        _context.Reports.Remove(report);
        await _context.SaveChangesAsync();
        return true;
    }
}