using Microsoft.AspNetCore.Mvc;

namespace OsintEyeWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelloController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Hello World!");
        }
    }
} 