using Microsoft.EntityFrameworkCore;
using PV260.API.BL.Mappers;
using PV260.API.DAL.Entities;
using PV260.API.DAL.UnitOfWork;
using PV260.Common.Models;

namespace PV260.API.BL.Facades;

/// <inheritdoc cref="IReportFacade"/>
public class ReportFacade(
    IMapper<ReportRecordEntity, ReportRecordModel, ReportRecordModel> reportRecordMapper,
    IMapper<ReportEntity, ReportListModel, ReportDetailModel> reportMapper,
    IUnitOfWorkFactory unitOfWorkFactory)
    : CrudFacadeBase<ReportListModel, ReportDetailModel, ReportEntity>(reportMapper, unitOfWorkFactory), IReportFacade
{
    private readonly IMapper<ReportRecordEntity, ReportRecordModel, ReportRecordModel> _reportRecordMapper =
        reportRecordMapper;
    private readonly IMapper<ReportEntity, ReportListModel, ReportDetailModel> _reportMapper = reportMapper;

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
    
    // TODO Implement report generation and old report cleanup methods here
    // Use methods provided by the base class for CRUD operations

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
}