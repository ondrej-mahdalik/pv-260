using Microsoft.AspNetCore.Mvc;

namespace PV260.API.App.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthCheckController : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Health Check")]
    [EndpointDescription("A dumb endpoint to check if the API is running and responding")]
    public ActionResult<string> Get()
    {
        return Ok("Healthy");
    }
}