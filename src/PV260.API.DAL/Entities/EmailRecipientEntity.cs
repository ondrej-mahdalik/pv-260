namespace PV260.API.DAL.Entities;

public record EmailRecipientEntity : EntityBase
{
    public required string EmailAddress { get; set; }
    public required DateTime CreatedAt { get; set; }
}