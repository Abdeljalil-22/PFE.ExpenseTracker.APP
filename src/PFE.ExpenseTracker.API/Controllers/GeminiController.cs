using Microsoft.AspNetCore.Mvc;
using PFE.ExpenseTracker.Infrastructure.Services;
using System.Threading.Tasks;

namespace PFE.ExpenseTracker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GeminiController : ControllerBase
    {
        private readonly GeminiAIService _geminiService;

        public GeminiController(GeminiAIService geminiService)
        {
            _geminiService = geminiService;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] GeminiRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Prompt))
                return BadRequest("Prompt is required.");

            var result = await _geminiService.GetGeminiCompletionAsync(request.Prompt, request.SystemInstruction);
            return Ok(new { response = result });
        }
    }

    public class GeminiRequest
    {
        public string Prompt { get; set; }
        public string SystemInstruction { get; set; }
    }
}
