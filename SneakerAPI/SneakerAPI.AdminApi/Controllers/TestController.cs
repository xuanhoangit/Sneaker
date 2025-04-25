

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    public TestController()
    {

    }
    [HttpGet]
    [Authorize]
    public IActionResult TEst()
    {
        return Ok("HAHAHAHAHA");
    }
}