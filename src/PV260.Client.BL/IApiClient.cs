using PV260.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PV260.Client.BL
{
    public interface IApiClient
    {
        Task<IEnumerable<ReportListModel>> GetAllReportsAsync();
        Task<ReportListModel> GetReportByIdAsync(Guid id);
        Task<ReportListModel> GetLatestReportAsync();
        Task<ReportListModel> GenerateNewReportAsync();
        Task DeleteReportAsync(Guid id);
        Task DeleteAllReportsAsync();
        Task SendReportAsync(Guid id);
        Task<SettingsModel> GetSettingsAsync();
    }
}
