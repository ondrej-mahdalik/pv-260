using Microsoft.AspNetCore.Mvc;

namespace PV260.API.App.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthCheckController : ControllerBase
{
    [HttpGet]
    public ActionResult<string> Get()
    {
        return Ok("Healthy");
    }
}