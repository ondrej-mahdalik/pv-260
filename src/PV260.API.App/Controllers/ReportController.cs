using Microsoft.AspNetCore.Mvc;
using PV260.API.BL.Facades;
using PV260.Common.Models;
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
                               // TODO Remove once methods are implemented

namespace PV260.API.App.Controllers;

[ApiController]
[Route("[controller]")]
public class ReportController(ReportFacade reportFacade) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReportListModel>>> GetAllReports()
    {
        return Ok(new List<ReportListModel>());
    }
    
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ReportDetailModel>> GetReportById(Guid id)
    {
        return Ok(new ReportDetailModel { Name = string.Empty, CreatedAt = DateTime.Now });
    }
    
    [HttpGet("latest")]
    public async Task<ActionResult<ReportDetailModel>> GetLatestReport()
    {
        return Ok(new ReportDetailModel { Name = string.Empty, CreatedAt = DateTime.Now });
    }
    
    [HttpPost("generate")]
    public async Task<ActionResult<ReportDetailModel>> GenerateNewReport()
    {
        return Ok(new ReportDetailModel { Name = string.Empty, CreatedAt = DateTime.Now });
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