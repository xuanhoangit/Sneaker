using Azure;
using Microsoft.AspNetCore.Mvc;
using SneakerAPI.Core.Interfaces;

namespace SneakerAPI.AdminApi.Controllers.ProductControllers
{   
    [Route("api/categories")]
    public class CategoryController : BaseController
    {
        private readonly IUnitOfWork _uow;
        private readonly int unitInAPage=20;
        

        public CategoryController(IUnitOfWork uow):base(uow)
        {
            _uow = uow;
        }
        [HttpGet("{page:int?}")]
        public IActionResult GetCategories(int page=1){
            var categories=_uow.Category.GetAll().Skip(unitInAPage*(page-1)).Take(unitInAPage);
            return Ok(categories);
        }
    }
}