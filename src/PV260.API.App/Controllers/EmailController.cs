using Microsoft.AspNetCore.Mvc;
using PV260.API.BL.Facades;

namespace PV260.API.App.Controllers;

[ApiController]
[Route("[controller]")]
public class EmailController(IEmailFacade emailFacade) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<string>>> GetAllEmails()
    {
        return Ok(new List<string>());
    }

    [HttpPost]
    public async Task<ActionResult> AddEmail([FromBody] string email)
    {
        return Ok();
    }
    
    [HttpDelete]
    public async Task<ActionResult> DeleteEmail()
    {
        return NoContent();
    }
    
    [HttpDelete("all")]
    public async Task<ActionResult> DeleteAllEmails()
    {
        return NoContent();
    }

        
    //TODO rename
    [HttpPost("testSendSingle")]
    public async Task<ActionResult> SendTestEmailSingle()
    {
        await emailFacade.TestEmailSingleAsync();
        return Ok();
    }

    //TODO rename 
    [HttpPost("testSendMultiple")]
    public async Task<ActionResult> SendTestEmailMultiple()
    {
        await emailFacade.TestEmailAsync();
        return Ok();
    }

}