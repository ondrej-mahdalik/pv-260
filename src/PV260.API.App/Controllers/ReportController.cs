using Microsoft.AspNetCore.Mvc;
using PV260.Common.Models;
using PV260.API.App.Services;

namespace PV260.API.App.Controllers;

[ApiController]
[Route("[controller]")]
public class ReportController : ControllerBase
{
    private readonly ReportService _reportService;

    public ReportController(ReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReportModel>>> GetAllReports()
    {
        var reports = await _reportService.GetAllReportsAsync();
        return Ok(reports);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ReportModel>> GetReportById(int id)
    {
        var report = await _reportService.GetReportByIdAsync(id);
        if (report == null)
            return NotFound();

        return Ok(report);
    }

    [HttpGet("latest")]
    public async Task<ActionResult<ReportModel>> GetLatestReport()
    {
        var reports = await _reportService.GetAllReportsAsync();
        var latestReport = reports.OrderByDescending(r => r.CreatedAt).FirstOrDefault();

        if (latestReport == null)
            return NotFound();

        return Ok(latestReport);
    }

    [HttpPost]
    public async Task<ActionResult<ReportModel>> CreateReport(ReportModel report)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var createdReport = await _reportService.CreateReportAsync(report);
        return CreatedAtAction(nameof(GetReportById), new { id = createdReport.Id }, createdReport);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ReportModel>> UpdateReport(int id, ReportModel report)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updatedReport = await _reportService.UpdateReportAsync(id, report);
        if (updatedReport == null)
            return NotFound();

        return Ok(updatedReport);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteReport(int id)
    {
        var result = await _reportService.DeleteReportAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}