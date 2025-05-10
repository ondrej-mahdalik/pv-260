using Microsoft.AspNetCore.Mvc;
using PV260.API.BL.Facades;
using PV260.Common.Models;

namespace PV260.API.App.Controllers;

[ApiController]
[Route("[controller]")]
public class EmailController(IEmailFacade emailFacade) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Get all email recipients")]
    [EndpointDescription("Returns a list of all email recipients")]
    public async Task<ActionResult<PaginatedResponse<EmailRecipientModel>>> GetAllEmails([FromQuery] PaginationCursor paginationCursor)
    {
        return Ok(await emailFacade.GetAllEmailRecipientsAsync(paginationCursor));
    }

    [HttpPost]
    [EndpointSummary("Add a new email recipient")]
    [EndpointDescription("Adds a new email recipient to the list")]
    public async Task<ActionResult> AddEmail([FromBody] EmailRecipientModel emailRecipient)
    {
        await emailFacade.AddEmailRecipientAsync(emailRecipient);
        return Ok();
    }
    
    [HttpDelete]
    [EndpointSummary("Delete an email recipient")]
    [EndpointDescription("Deletes an email recipient from the list")]
    public async Task<ActionResult> DeleteEmail([FromBody] string email)
    {
        await emailFacade.DeleteEmailRecipientAsync(email);
        return NoContent();
    }
    
    [HttpDelete("all")]
    [EndpointSummary("Delete all email recipients")]
    [EndpointDescription("Deletes all email recipients from the list")]
    public async Task<ActionResult> DeleteAllEmails()
    {
        await emailFacade.DeleteAllEmailRecipientsAsync();
        return NoContent();
    }
}