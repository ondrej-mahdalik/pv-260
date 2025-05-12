using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PV260.API.BL.Facades;
using PV260.Common.Models;

namespace PV260.API.Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class ReportController(IReportFacade reportFacade) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Get all reports")]
    [EndpointDescription("Returns a list of all reports")]
    public async Task<ActionResult<PaginatedResponse<ReportListModel>>> GetAllReports([FromQuery] PaginationCursor paginationCursor)
    {
        return Ok(await reportFacade.GetAsync(paginationCursor));
    }

    [HttpGet("{id:guid}")]
    [EndpointSummary("Get report by ID")]
    [EndpointDescription("Returns a detail of a report by its ID")]
    public async Task<ActionResult<ReportDetailModel>> GetReportById(Guid id)
    {
        var report = await reportFacade.GetAsync(id);
        if (report is null)
            return NotFound();

        return Ok(report);
    }

    [HttpGet("latest")]
    [EndpointSummary("Get latest report")]
    [EndpointDescription("Returns details of the latest report, if available")]
    public async Task<ActionResult<ReportDetailModel>> GetLatestReport()
    {
        var report = await reportFacade.GetLatestAsync();
        if (report is null)
            return NotFound();

        return Ok(report);
    }

    [HttpPost("generate")]
    [EndpointSummary("Generate a new report")]
    [EndpointDescription("Generates a new report and returns its details")]
    public async Task<ActionResult<ReportDetailModel>> GenerateNewReport()
    {
        var report = await reportFacade.GenerateReportAsync();
        return Ok(report);
    }

    [HttpDelete("{id:guid}")]
    [EndpointSummary("Delete report by ID")]
    [EndpointDescription("Deletes a report by its ID. If the report is not found, nothing happens.")]
    public async Task<ActionResult> DeleteReport(Guid id)
    {
        await reportFacade.DeleteAsync(id);
        return NoContent();
    }

    [HttpDelete("all")]
    [EndpointSummary("Delete all reports")]
    [EndpointDescription("Deletes all reports")]
    public async Task<ActionResult> DeleteAllReports()
    {
        await reportFacade.DeleteAllAsync();
        return NoContent();
    }

    [HttpPost("{id:guid}/send")]
    [EndpointSummary("Send email to recipients with report")]
    [EndpointDescription("Sends an email to all recipients with the report details")]
    public async Task<ActionResult> SendReport(Guid id)
    {
        await reportFacade.SendReportViaEmailAsync(id);
        return Ok();
    }
}