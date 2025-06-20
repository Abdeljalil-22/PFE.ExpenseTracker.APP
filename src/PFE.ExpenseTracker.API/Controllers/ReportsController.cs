// using System;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using PFE.ExpenseTracker.Application.Common.Interfaces;
// using System.Security.Claims;

// namespace PFE.ExpenseTracker.API.Controllers
// {
//     [Authorize]
//     [ApiController]
//     [Route("api/[controller]")]
//     [Produces("application/json")]
//     [Tags("Reports")]
//     public class ReportsController : ControllerBase
//     {
//         private readonly IReportService _reportService;

//         public ReportsController(IReportService reportService)
//         {
//             _reportService = reportService;
//         }

//         /// <summary>
//         /// Generate an expense report for a specific date range
//         /// </summary>
//         /// <param name="startDate">Start date for the report</param>
//         /// <param name="endDate">End date for the report</param>
//         /// <param name="format">Report format (PDF or Excel)</param>
//         /// <returns>The generated report file</returns>
//         [HttpGet("expenses")]
//         [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
//         public async Task<IActionResult> GenerateExpenseReport(
//             [FromQuery] DateTime startDate,
//             [FromQuery] DateTime endDate,
//             [FromQuery] string format = "PDF")
//         {
//             var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//             var reportBytes = await _reportService.GenerateExpenseReportAsync(
//                 Guid.Parse(userId),
//                 startDate,
//                 endDate,
//                 format);

//             var contentType = format.Equals("PDF", StringComparison.OrdinalIgnoreCase)
//                 ? "application/pdf"
//                 : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

//             var fileName = format.Equals("PDF", StringComparison.OrdinalIgnoreCase)
//                 ? "ExpenseReport.pdf"
//                 : "ExpenseReport.xlsx";

//             return File(reportBytes, contentType, fileName);
//         }

//         /// <summary>
//         /// Generate a budget report
//         /// </summary>
//         /// <param name="startDate">Start date for the report</param>
//         /// <param name="endDate">End date for the report</param>
//         /// <param name="format">Report format (PDF or Excel)</param>
//         /// <returns>The generated report file</returns>
//         [HttpGet("budgets")]
//         [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
//         public async Task<IActionResult> GenerateBudgetReport(
//             [FromQuery] DateTime startDate,
//             [FromQuery] DateTime endDate,
//             [FromQuery] string format = "PDF")
//         {
//             var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//             var reportBytes = await _reportService.GenerateBudgetReportAsync(
//                 Guid.Parse(userId),
//                 startDate,
//                 endDate,
//                 format);

//             var contentType = format.Equals("PDF", StringComparison.OrdinalIgnoreCase)
//                 ? "application/pdf"
//                 : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

//             var fileName = format.Equals("PDF", StringComparison.OrdinalIgnoreCase)
//                 ? "BudgetReport.pdf"
//                 : "BudgetReport.xlsx";

//             return File(reportBytes, contentType, fileName);
//         }

//         /// <summary>
//         /// Generate an annual summary report
//         /// </summary>
//         /// <param name="year">The year for the report</param>
//         /// <param name="format">Report format (PDF or Excel)</param>
//         /// <returns>The generated report file</returns>
//         [HttpGet("annual-summary")]
//         [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
//         public async Task<IActionResult> GenerateAnnualSummaryReport(
//             [FromQuery] int year,
//             [FromQuery] string format = "PDF")
//         {
//             var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//             var reportBytes = await _reportService.GenerateAnnualSummaryReportAsync(
//                 Guid.Parse(userId),
//                 year,
//                 format);

//             var contentType = format.Equals("PDF", StringComparison.OrdinalIgnoreCase)
//                 ? "application/pdf"
//                 : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

//             var fileName = format.Equals("PDF", StringComparison.OrdinalIgnoreCase)
//                 ? $"AnnualSummary_{year}.pdf"
//                 : $"AnnualSummary_{year}.xlsx";

//             return File(reportBytes, contentType, fileName);
//         }
//     }
// }
