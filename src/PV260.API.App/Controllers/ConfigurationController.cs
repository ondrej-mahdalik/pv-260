using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PV260.Common.Models;

namespace PV260.API.App.Controllers;

[ApiController]
[Route("[controller]")]
public class ConfigurationController(IOptions<SettingsModel> settings) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Get application settings")]
    [EndpointDescription("Returns the application settings")]
    public ActionResult<SettingsModel> GetSettings()
    {
        return Ok(settings.Value);
    }
}