using PV260.Common.Models;

namespace PV260.API.BL.Facades;

/// <summary>
/// A facade for managing email operations. Includes CRUD operations for email recipients list.
/// And methods for sending emails.
/// </summary>
public interface IEmailFacade
{
    /// <summary>
    /// Retrieves all email recipients.
    /// </summary>
    /// <returns>A list of email addresses.</returns>
    Task<IList<EmailRecipientModel>> GetAllEmailRecipientsAsync();

    /// <summary>
    /// Adds a new email recipient to the storage.
    /// </summary>
    /// <param name="emailRecipientModel">The email to add.</param>
    Task AddEmailRecipientAsync(EmailRecipientModel emailRecipientModel);

    /// <summary>
    /// Deletes an email from the storage.
    /// </summary>
    /// <param name="emailAddress">The email address of the recipient to delete.</param>
    Task DeleteEmailRecipientAsync(string emailAddress);
    
    /// <summary>
    /// Deletes all email recipients from the storage.
    /// </summary>
    Task DeleteAllEmailRecipientsAsync();
}