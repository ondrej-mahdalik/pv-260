using PV260.API.DAL.Entities;
using PV260.Common.Models;

namespace PV260.API.BL.Facades;

/// <summary>
/// A facade for managing reports, including report generation, CRUD operations and cleanup logic.
/// </summary>
public interface IReportFacade : ICrudFacade<ReportListModel, ReportDetailModel, ReportEntity>;