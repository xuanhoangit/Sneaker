using Microsoft.AspNetCore.Mvc;
using SneakerAPI.Api.Controllers;
using SneakerAPI.Core.Interfaces;
namespace SneakerAPI.Api.Controllers.ProductControllers;
[Route("api/files")]
[ApiController]
public class ProductColorFileController: BaseController
{
    private readonly IUnitOfWork _uow;

    public ProductColorFileController(IUnitOfWork uow):base(uow)
    {
        _uow = uow;
    }
    [HttpGet("main-image")]
    public async Task<IActionResult> GetMainImage(int pc_id){
        var image=await _uow.ProductColorFile.FirstOrDefaultAsync(x=>x.ProductColorFile__ProductColorId==pc_id && x.ProductColorFile__Main);
        if(image==null){
            return NotFound();
        }
        return Ok(image);
    }
    [HttpGet("images")]
    public IActionResult GetImages(int pc_id){
        var images=  _uow.ProductColorFile.GetAll(x=>x.ProductColorFile__ProductColorId==pc_id);
         if(images==null && !images.Any()){
            return NotFound();
        }
        return Ok(images);
    }
}