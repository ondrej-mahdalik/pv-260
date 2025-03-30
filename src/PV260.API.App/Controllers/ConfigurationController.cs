using Microsoft.AspNetCore.Mvc;
using PV260.Common.Models;

namespace PV260.API.App.Controllers;

[ApiController]
[Route("[controller]")]
public class ConfigurationController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<SettingsModel>> GetSettings()
    {
        return Ok(new SettingsModel("0 0 * * *", 30, true));
    }
    
    [HttpPost]
    public async Task<ActionResult<SettingsModel>> UpdateSettings([FromBody] SettingsModel settings)
    {
        // Here you would typically update the settings in a database or configuration file
        return Ok(settings);
    }
}