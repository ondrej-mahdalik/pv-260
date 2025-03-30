using Microsoft.AspNetCore.Mvc;

namespace PV260.API.App.Controllers;

[ApiController]
[Route("[controller]")]
public class EmailController : ControllerBase
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
}