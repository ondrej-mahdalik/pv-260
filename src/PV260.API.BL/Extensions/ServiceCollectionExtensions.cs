using Microsoft.Extensions.DependencyInjection;
using PV260.API.BL.Mappers;
using PV260.API.DAL.Entities;
using PV260.Common.Models;

namespace PV260.API.BL.Extensions;

/// <summary>
/// Provides extension methods for registering business logic services in the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds business logic services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="serviceCollection">The service collection to which the services will be added.</param>
    public static IServiceCollection AddDBlServices(this IServiceCollection serviceCollection)
    {
        // Mappers
        serviceCollection.AddSingleton<IMapper<ReportEntity, ReportListModel, ReportDetailModel>, ReportMapper>();
        serviceCollection.AddSingleton<IMapper<ReportRecordEntity, ReportRecordModel, ReportRecordModel>, ReportRecordMapper>();
        serviceCollection.AddSingleton<IMapper<EmailRecipientEntity, EmailRecipientModel, EmailRecipientModel>, EmailMapper>();

        return serviceCollection;
    }
}