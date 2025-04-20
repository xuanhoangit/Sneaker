using Microsoft.AspNetCore.Mvc;
[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    public TestController()
    {

    }
    [HttpGet]
    public IActionResult HEHE()
    {
        return Ok("HEHEHEHEHEHEE");
    }
}