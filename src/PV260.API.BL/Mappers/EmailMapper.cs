using PV260.API.DAL.Entities;
using PV260.Common.Models;

namespace PV260.API.BL.Mappers;

public class EmailMapper : IMapper<EmailEntity, EmailRecipientModel, EmailRecipientModel>
{
    public EmailRecipientModel ToListModel(EmailEntity entity)
    {
        return new EmailRecipientModel
        {
            EmailAddress = entity.EmailAddress,
            CreatedAt = entity.CreatedAt
        };
    }

    public EmailRecipientModel ToDetailModel(EmailEntity entity)
    {
        return new EmailRecipientModel
        {
            EmailAddress = entity.EmailAddress,
            CreatedAt = entity.CreatedAt
        };
    }

    public EmailEntity ToEntity(EmailRecipientModel detailRecipientModel)
    {
        return new EmailEntity
        {
            EmailAddress = detailRecipientModel.EmailAddress,
            CreatedAt = detailRecipientModel.CreatedAt
        };
    }
}