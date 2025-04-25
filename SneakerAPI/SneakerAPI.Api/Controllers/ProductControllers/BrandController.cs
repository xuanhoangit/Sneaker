using Microsoft.AspNetCore.Mvc;
using SneakerAPI.Core.Interfaces;

namespace SneakerAPI.Api.Controllers.ProductControllers;
[Route("api/brands")]
public class BrandController : BaseController
{
    private readonly IUnitOfWork uow;
    private readonly int  unitInAPage=20;

    public BrandController(IUnitOfWork uow) : base(uow)
    {
        this.uow = uow;
    }
    [HttpGet("search/{searchString}/page/{page}")]
    public IActionResult GetBrands(string searchString = "", int page = 1)
    {
        var brands = uow.Brand.GetAll(x => x.Brand__Name.Contains(searchString)).Skip(unitInAPage * (page - 1)).Take(unitInAPage);
        return brands.Count() > 0 ?
        Ok(brands) : NotFound("NotFound");
    }
}