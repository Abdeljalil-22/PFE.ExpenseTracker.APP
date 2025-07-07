using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace PFE.ExpenseTracker.MCP.Controllers
{
    [ApiController]
    [Route("api/mcp-server")]
    public class McpServerController : ControllerBase
    {
        private readonly ExpenseTrackerMcpServer _mcpServer;
        private readonly ILogger<McpServerController> _logger;

        public McpServerController(ExpenseTrackerMcpServer mcpServer, ILogger<McpServerController> logger)
        {
            _mcpServer = mcpServer;
            _logger = logger;
        }

        [HttpPost("call")]
        public async Task<IActionResult> Call([FromBody] McpRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Method))
                return BadRequest(new { error = "Invalid MCP request" });

            McpResponse response;
            switch (request.Method.ToLowerInvariant())
            {
                case "initialize":
                    response = await _mcpServer.InitializeAsync(request);
                    break;
                case "listtools":
                    response = await _mcpServer.ListToolsAsync(request);
                    break;
                case "calltool":
                    response = await _mcpServer.CallToolAsync(request);
                    break;
                case "listresources":
                    response = await _mcpServer.ListResourcesAsync(request);
                    break;
                case "readresource":
                    response = await _mcpServer.ReadResourceAsync(request);
                    break;
                case "listprompts":
                    response = await _mcpServer.ListPromptsAsync(request);
                    break;
                case "getprompt":
                    response = await _mcpServer.GetPromptAsync(request);
                    break;
                default:
                    response = new McpResponse
                    {
                        Id = request.Id,
                        Error = new McpError { Code = -32601, Message = "Method not found", Data = request.Method }
                    };
                    break;
            }
            return Ok(response);
        }
    }
}
