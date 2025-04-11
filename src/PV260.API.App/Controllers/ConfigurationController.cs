using Microsoft.AspNetCore.Mvc;
using PV260.Common.Models;

namespace PV260.API.App.Controllers;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
// TODO Remove once methods are implemented

[ApiController]
[Route("[controller]")]
public class ConfigurationController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<SettingsModel>> GetSettings()
    {
        return Ok(new SettingsModel("0 0 * * *", 30, true));
    }
}