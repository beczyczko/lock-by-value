using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace RaceCondition.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private static readonly object Lock = new object();

        private readonly ILogger<ValuesController> _logger;

        public ValuesController(ILogger<ValuesController> logger)
        {
            _logger = logger;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<string> Get()
        {
            return "start";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromForm] string value, [FromForm] string callNo)
        {
            // we could get user id from HttpContext and use it in lock
            using (new LockByValue(value))
            {
                _logger.LogCritical($"Enter endpoint: {value}, {callNo}");
                _logger.LogCritical($"Processing: {value}, {callNo}");
                _logger.LogCritical($"Leave endpoint: {value}, {callNo}");
            }

            // bad aproach - you block code for everyone
            //            lock (Lock)
            //            {
            //                _logger.LogCritical($"Enter endpoint: {value}, {callNo}");
            //                _logger.LogCritical($"Processing: {value}, {callNo}");
            //                _logger.LogCritical($"Leave endpoint: {value}, {callNo}");
            //            }
        }
    }

}
