using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SneakerAPI.Core.Interfaces;

namespace SneakerAPI.AdminApi.Controllers.ProductControllers
{   
    [Route("api/brands")]
    public class BrandController : BaseController
    {
        private readonly IUnitOfWork _uow;
        private readonly int unitInAPage=20;
        public BrandController(IUnitOfWork uow):base(uow)
        {
            _uow = uow;
        }
        [HttpGet("{id}")]
        public IActionResult GetBrandById(int id){
            var brand=_uow.Brand.Get(id);
            return brand!=null?
            Ok(brand):NotFound("NotFound");
        }
        [HttpGet("search/{searchString}/page/{page}")]
        public IActionResult GetBrands(string searchString="",int page=1){
            var brands=_uow.Brand.GetAll(x=> x.Brand__Name.Contains(searchString)).Skip(unitInAPage*(page-1)).Take(unitInAPage);
            return brands.Count()>0?
            Ok(brands):NotFound("NotFound");
        }
    }
}