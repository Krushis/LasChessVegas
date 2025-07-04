using Microsoft.AspNetCore.Mvc;

namespace VegasBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MainController : ControllerBase
    {
        private readonly ILogger<MainController> _logger;

        public MainController(ILogger<MainController> logger)
        {
            _logger = logger;
        }

        public IAsyncResult GetLegalMoves()
        {

        }
        
    }
}
