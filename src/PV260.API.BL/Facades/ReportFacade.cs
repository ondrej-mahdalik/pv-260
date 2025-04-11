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
    // TODO Implement report generation and old report cleanup methods here
    // Use methods provided by the base class for CRUD operations

    public override async Task SaveAsync(ReportDetailModel model)
    {
        // Save the report using the base class method
        await base.SaveAsync(model);

        // Save the report records
        await using var unitOfWork = UnitOfWorkFactory.Create();
        var reportRecords = _reportRecordMapper.ToEntity(model.Records);


    }
}