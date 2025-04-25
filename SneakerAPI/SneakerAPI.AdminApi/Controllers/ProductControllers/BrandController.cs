using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SneakerAPI.Core.DTOs;
using SneakerAPI.Core.Interfaces;
using SneakerAPI.Core.Libraries;
using SneakerAPI.Core.Models.ProductEntities;

namespace SneakerAPI.AdminApi.Controllers.ProductControllers
{
    [Route("api/brands")]
    [Authorize(Roles=RolesName.ProductManager)]
    public class BrandController : BaseController
    {
        private readonly IUnitOfWork _uow;
        private readonly int unitInAPage = 20;
         private readonly string localStorage="uploads/logos";
        private readonly string uploadFilePath=Path.Combine(Directory.GetCurrentDirectory(),"wwwroot/uploads/logos");
        public BrandController(IUnitOfWork uow) : base(uow)
        {
            _uow = uow;
        }
        [HttpGet("haha")]
        public IActionResult HAHA()
        {
            return Ok("hahaha");
        }
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] AddBrandDTO brandDto)
        {

            try
            {

                var brand = new Brand
                {
                    Brand__Description = brandDto.Brand__Description,
                    Brand__IsActive = brandDto.Brand__IsActive,
                    Brand__Name = brandDto.Brand__Name,
                    
                };
                if (brandDto.FileLogo != null)
                {
                    brand.Brand__Logo = Guid.NewGuid().ToString() + ".jpg";
                }
                var result = _uow.Brand.Add(brand);
                if (!result)
                {
                    return BadRequest("Create size failed");
                }

                await HandleFile.Upload(brandDto.FileLogo, Path.Combine(uploadFilePath, brand.Brand__Logo));
                brand.Brand__Logo = $"{Request.Scheme}://{Request.Host}/{localStorage}/{brand.Brand__Logo}";
                var uri = new Uri($"{Request.Scheme}://{Request.Host}/api/brands/{brand.Brand__Id}");
                return Created(uri, new { Message = "created brand successfully.", Data = brand });
                  
            }
            catch (System.Exception e)
            {
                
                return StatusCode(500,$"An error occur create brand : {e}");
            }
        }
        [HttpGet("{id}")]
        public IActionResult GetBrandById(int id)
        {
            var brand = _uow.Brand.Get(id);
            return brand != null ?
            Ok(brand) : NotFound("NotFound");
        }
        [HttpGet("search/{searchString}/page/{page}")]
        public IActionResult GetBrands(string searchString = "", int page = 1)
        {
            var brands = _uow.Brand.GetAll(x => x.Brand__Name.Contains(searchString)).Skip(unitInAPage * (page - 1)).Take(unitInAPage);
            return brands.Count() > 0 ?
            Ok(brands) : NotFound("NotFound");
        }
    }
}