using Microsoft.AspNetCore.Mvc;
using PV260.Common.Models;

namespace PV260.API.App.Controllers;

[ApiController]
[Route("[controller]")]
public class ReportController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReportModel>>> GetAllReports()
    {
        return Ok(new List<ReportModel>());
    }
    
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ReportModel>> GetReportById(Guid id)
    {
        return Ok(new ReportModel());
    }
    
    [HttpGet("latest")]
    public async Task<ActionResult<ReportModel>> GetLatestReport()
    {
        return Ok(new ReportModel());
    }
    
    [HttpPost("generate")]
    public async Task<ActionResult<ReportModel>> GenerateNewReport()
    {
        return Ok(new ReportModel());
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteReport(Guid id)
    {
        return NoContent();
    }
    
    [HttpDelete("all")]
    public async Task<ActionResult> DeleteAllReports()
    {
        return NoContent();
    }
    
    [HttpPost("{id:guid}/send")]
    public async Task<ActionResult> SendReport(Guid id)
    {
        return Ok();
    }
}