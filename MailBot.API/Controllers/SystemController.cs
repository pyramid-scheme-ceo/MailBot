using Microsoft.AspNetCore.Mvc;

namespace MailBot.API.Controllers
{
    [ApiController]
    [Route("api/system")]
    public class SystemController : ControllerBase
    {
        [HttpGet("version")]
        public IActionResult Version()
        {
            return Ok(typeof(SystemController).Assembly.GetName().Version);
        }
    }
}