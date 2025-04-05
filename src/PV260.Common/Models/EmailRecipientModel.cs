using System.ComponentModel.DataAnnotations;

namespace PV260.Common.Models;

public record EmailRecipientModel
{
    [Required]
    [EmailAddress]
    public required string EmailAddress { get; set; }
    
    public required DateTime CreatedAt { get; set; }
}