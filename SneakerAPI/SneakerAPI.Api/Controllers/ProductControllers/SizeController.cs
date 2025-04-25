using Microsoft.AspNetCore.Mvc;
using SneakerAPI.Core.Interfaces;

namespace SneakerAPI.Api.Controllers.ProductControllers;
[Route("api/sizes")]
public class SizeController : BaseController
{
    private readonly IUnitOfWork _uow;

    public SizeController(IUnitOfWork uow) : base(uow)
    {
        _uow = uow;
    }
    [HttpGet("product-color/{pc_id}")]
    public IActionResult GetSizesByProductColor(int pc_id)
    {
        var sizeIds = _uow.ProductColorSize.Find(x => x.ProductColorSize__ProductColorId == pc_id).Select(x => x.ProductColorSize__SizeId);
        var sizes = _uow.Size.GetAll(x => sizeIds.Contains(x.Size__Id)).ToList();
        return Ok(sizes);
    }
}