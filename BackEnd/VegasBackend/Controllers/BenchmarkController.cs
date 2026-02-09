using Microsoft.AspNetCore.Mvc;
using VegasBackend.ServerLogic.ChessAI;

namespace VegasBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BenchmarkController : ControllerBase
    {
        private readonly ILogger<BenchmarkController> _logger;

        public BenchmarkController(ILogger<BenchmarkController> logger)
        {
            _logger = logger;
        }

        [HttpGet("/bench")]
        public IActionResult Bench()
        {
            _logger.LogInformation("Running AI benchmark...");
            AIService.Benchmark();
            return Ok(new { success = true, message = "Benchmark complete - check console output" });
        }
    }
}
