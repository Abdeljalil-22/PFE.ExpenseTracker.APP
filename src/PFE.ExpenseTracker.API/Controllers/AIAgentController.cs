namespace PFE.ExpenseTracker.API.Controllers;


using PFE.ExpenseTracker.AIAgent.Models;
using PFE.ExpenseTracker.AIAgent.Services; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AIAgentController : ControllerBase
{
 
    private readonly ILogger<AIAgentController> _logger;
    private readonly  ExpenseTracker.AIAgent.Services.AIAgent _aiAgent; 

    public AIAgentController(

        ILogger<AIAgentController> logger , AIAgent aiAgent) 
    {
        _aiAgent = aiAgent;
        _logger = logger;
    }

    [HttpPost("process")]
    public async Task<IActionResult> ProcessRequest([FromBody] McpProcessRequest request)
    {
        var errors = new List<ValidationError>();
        if (string.IsNullOrWhiteSpace(request.Prompt))
            errors.Add(new ValidationError { Field = "Prompt", Error = "Prompt is required." });
        if (string.IsNullOrWhiteSpace(request.UserId))
            errors.Add(new ValidationError { Field = "UserId", Error = "UserId is required." });
        if (errors.Count > 0)
            return BadRequest(new { Success = false, Errors = errors });

        ArgumentNullException.ThrowIfNull(request.UserId); 

        try
        {
            var aIAgent= await _aiAgent.ProcessAsync(request);

            if (aIAgent == null)
            {
                return BadRequest(new McpProcessResponse
                {
                    Success = false,
                    Error = "Could not interpret the request (empty action)",
                    Data = new Dictionary<string, object> { ["raw"] = request.Prompt }
                });
            }
            // if (aIAgent.Response != null)
            // {
            //     return BadRequest(new McpProcessResponse
            //     {
            //         Success = false,
            //         Response = aIAgent.Response,
            //         Error = aIAgent.Error,
            //         Data = aIAgent.Data,
            //         History = aIAgent.History
            //     });
            // }
            // if(aIAgent.Success == false)
            // {
            //     return BadRequest(new McpProcessResponse
            //     {
            //         Success = false,
            //         Error = aIAgent.Error,
            //         Response = aIAgent.Response,
            //         Data = aIAgent.Data,
            //         History = aIAgent.History
            //     });
            // }

            return Ok(aIAgent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing MCP request");
            return StatusCode(500, new McpProcessResponse
            {
                Success = false,
                Error = ex.Message
            });
        }
    }
}
