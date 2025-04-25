using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SneakerAPI.Core.DTOs;
using SneakerAPI.Core.Interfaces;
using SneakerAPI.Core.Models.ProductEntities;

namespace SneakerAPI.AdminApi.Controllers.ProductControllers;

[Route("api/sizes")]
[Authorize]
public class SizeController : BaseController
{
    private readonly IUnitOfWork _uow;

    public SizeController(IUnitOfWork uow) : base(uow)
    {
        _uow = uow;
    }
    [Authorize(Roles = RolesName.ProductManager)]
    [HttpPost("create")]
    public IActionResult Create([FromBody]Size size)
    {
        var result = _uow.Size.Add(size);
        if (!result)
        {
            return BadRequest("Create size failed");
        }
        var uri=new Uri($"{Request.Scheme}://{Request.Host}/api/sizes/{size.Size__Id}"); 
        return Created(uri,new
        {
            message="Create size successfully"
        });
    }
    [HttpGet("product-color/{pc_id}")]
    public IActionResult GetSizesByProductColor(int pc_id)
    {
        var sizeIds = _uow.ProductColorSize.Find(x => x.ProductColorSize__ProductColorId == pc_id).Select(x => x.ProductColorSize__SizeId);
        var sizes = _uow.Size.GetAll(x => sizeIds.Contains(x.Size__Id)).ToList();
        return Ok(sizes);
    }
}