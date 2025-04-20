using PV260.API.DAL.Entities;
using PV260.Common.Models;

namespace PV260.API.BL.Mappers;

/// <inheritdoc />
/// <param name="reportRecordMapper">The mapper for <see cref="ReportRecordEntity"/> to <see cref="ReportRecordModel"/>.</param>
public class ReportMapper(IMapper<ReportRecordEntity, ReportRecordModel, ReportRecordModel> reportRecordMapper)
    : IMapper<ReportEntity, ReportListModel, ReportDetailModel>
{
    /// <inheritdoc />
    public ReportListModel ToListModel(ReportEntity entity)
    {
        return new ReportListModel
        {
            Id = entity.Id,
            Name = entity.Name,
            CreatedAt = entity.CreatedAt
        };
    }

    /// <inheritdoc />
    public ReportDetailModel ToDetailModel(ReportEntity entity)
    {
        var records = entity.Records
            .Select(reportRecordMapper.ToListModel)
            .ToList();

        return new ReportDetailModel
        {
            Id = entity.Id,
            Name = entity.Name,
            CreatedAt = entity.CreatedAt,
            Records = records
        };
    }

    /// <inheritdoc />
    public ReportEntity ToEntity(ReportDetailModel detailModel)
    {
        return new ReportEntity
        {
            Id = detailModel.Id,
            Name = detailModel.Name,
            CreatedAt = detailModel.CreatedAt,
            Records = detailModel.Records.Select(reportRecordMapper.ToEntity).ToList()
        };
    }
}