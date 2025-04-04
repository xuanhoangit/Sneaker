using Microsoft.AspNetCore.Mvc;
[Route("api/test")]
public class TestController: ControllerBase
{   
    [HttpGet]
    public IActionResult HAHA(){
        return Ok("HAHAHAHAHA");
    }
}