using PV260.API.DAL.Entities;
using PV260.Common.Models;

namespace PV260.API.BL.Mappers;

/// <inheritdoc />
public class ReportRecordMapper : IMapper<ReportRecordEntity, ReportRecordModel, ReportRecordModel>
{
    /// <inheritdoc />
    public ReportRecordModel ToListModel(ReportRecordEntity entity)
    {
        return ToDetailModel(entity);
    }

    /// <inheritdoc />
    public ReportRecordModel ToDetailModel(ReportRecordEntity entity)
    {
        return new ReportRecordModel
        {
            Id = entity.Id,
            CompanyName = entity.CompanyName,
            Ticker = entity.Ticker,
            Weight = entity.Weight,
            NumberOfShares = entity.NumberOfShares,
            SharesChangePercentage = entity.SharesChangePercentage
        };
    }

    /// <inheritdoc />
    public ReportRecordEntity ToEntity(ReportRecordModel detailModel)
    {
        return new ReportRecordEntity
        {
            Id = detailModel.Id,
            CompanyName = detailModel.CompanyName,
            Ticker = detailModel.Ticker,
            Weight = detailModel.Weight,
            NumberOfShares = detailModel.NumberOfShares,
            SharesChangePercentage = detailModel.SharesChangePercentage
        };
    }
}