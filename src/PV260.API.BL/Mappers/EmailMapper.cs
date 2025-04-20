using PV260.API.DAL.Entities;
using PV260.Common.Models;

namespace PV260.API.BL.Mappers;

public class EmailMapper : IMapper<EmailRecipientEntity, EmailRecipientModel, EmailRecipientModel>
{
    public EmailRecipientModel ToListModel(EmailRecipientEntity recipientEntity)
    {
        return new EmailRecipientModel
        {
            EmailAddress = recipientEntity.EmailAddress,
            CreatedAt = recipientEntity.CreatedAt
        };
    }

    public EmailRecipientModel ToDetailModel(EmailRecipientEntity recipientEntity)
    {
        return new EmailRecipientModel
        {
            EmailAddress = recipientEntity.EmailAddress,
            CreatedAt = recipientEntity.CreatedAt
        };
    }

    public EmailRecipientEntity ToEntity(EmailRecipientModel detailRecipientModel)
    {
        return new EmailRecipientEntity
        {
            EmailAddress = detailRecipientModel.EmailAddress,
            CreatedAt = detailRecipientModel.CreatedAt
        };
    }
}