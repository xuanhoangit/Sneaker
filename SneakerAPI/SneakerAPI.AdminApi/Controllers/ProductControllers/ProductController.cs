using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SneakerAPI.Core.DTOs;
using SneakerAPI.Core.Interfaces;
using SneakerAPI.Core.Models.Filters;
using SneakerAPI.Core.Models.ProductEntities;

namespace SneakerAPI.AdminApi.Controllers.ActionProductControllers
{   
    [ApiController]
    [Route("api/products")]
    [Authorize(Roles=$"{RolesName.Staff},{RolesName.Manager},{RolesName.Admin}")]
    public class GetProductDataController :BaseController
    {
        private readonly IUnitOfWork _uow;
        private readonly int unitInAPage=20;
        
        public GetProductDataController(IUnitOfWork uow):base(uow)
        {
            _uow = uow;
        }
        [HttpGet("{status}/page/{page}")]
        public IActionResult GetProductsByStatus(int status,int page=1){
            try
            {   
                if(status==(int)Status.Deleted){
                    return BadRequest("Status is not valid");
                }
                var products=
                 _uow.Product.GetAll(x=>x.Product__Status==status,"ProductColors").Skip((page-1)*unitInAPage).Take(unitInAPage);
                if(products==null)
                return NotFound("No results found");
                return Ok(products);
            }
            catch (System.Exception e)
            {
                return BadRequest(e);
            }
        }
        [HttpGet("{id}")]
        public IActionResult GetProduct(int id){
            try
            {
       
                var product = _uow.Product.FirstOrDefault(x=>x.Product__Id==id && x.Product__Status !=(int)Status.Deleted);
                if(product==null)
                    return NotFound("Product not found");
                 return Ok(product);
                 
            }
            catch (System.Exception e)
            {
                
                return BadRequest(e);
            }
        }

        [HttpGet("filter")]
        public IActionResult GetProductsByFilter([FromQuery] ProductFilter filter,int page=1){
            try
            {   
                var products= _uow.Product.GetFilteredProducts(filter).Where(x=>x.Product__Status!=(int)Status.Deleted).Skip((page-1)*unitInAPage).Take(unitInAPage).ToList();
                if(products==null)
                return NotFound("No results found");
                return Ok(products);
            }
            catch (System.Exception e)
            {
                
                return BadRequest(e);
            }
        }
        
        [HttpGet("category/{category_id}/page/{page}")]
        public IActionResult GetProductsByCategory(int category_id,int page=1){
            try
            {
                var category= _uow.Category.Get(category_id);
                if(category==null){
                    return NotFound("Category is not exists");
                }
                var ids= _uow.ProductCategory.GetAll(x=>x.ProductCategory__CategoryId==category_id).Select(x=>x.ProductCategory__ProductId).ToList();
                System.Console.WriteLine(ids);
                if(ids==null || ids.Count()<=0){
                    return NotFound("No results found");
                }
                var products=_uow.Product.GetAll(x=>ids.Contains(x.Product__Id) && x.Product__Status !=(int)Status.Deleted).Skip((page-1)*unitInAPage).Take(unitInAPage);
                return Ok(products);
            }
            catch (System.Exception e)
            {
                
                return BadRequest(e);
            }
        }
    }

    [ApiController]
    [Route("api/products")]
    [Authorize(Roles=RolesName.Manager)]
    public class ActionProductController : BaseController
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public ActionProductController(IUnitOfWork uow,IMapper mapper):base(uow)
        {   
            _uow = uow;
            _mapper = mapper;
        }

        [HttpPatch("publish")]
        public IActionResult ReleaseProducts([FromBody] List<int> ids){
            try
            {
                if (ids == null || !ids.Any())
                    return BadRequest(new { Message = "No product provided" });

                var failed=new List<string>();
                foreach (var id in ids)
                {   
                    var p=_uow.Product.FirstOrDefault(x=>x.Product__Id==id && x.Product__Status==(int)Status.Unreleased);
                    if(p==null){
                        failed.Add($"No results found for product with ID = {id}");
                        continue;
                    }
                   
                        p.Product__Status=(int)Status.Released;
                        var result=_uow.Product.Update(p);
                        if(!result)
                        failed.Add($"Fail to publish product {p.Product__Name}");

                    
                }
                return !failed.Any()?Ok(new {Message="Published products successfully"}):Ok(failed);
            }
            catch (System.Exception e )
            {
                
                return BadRequest(e);
            }
        }
        [HttpPatch("stop-publishing")]
        public IActionResult StopPublishing([FromBody]List<int> ids){
            try
            {
            if (ids == null || !ids.Any())
                return BadRequest(new { Message = "No product provided" });

            var failed=new List<string>();
            foreach (var id in ids)
            {   
                var p=_uow.Product.GetFirstOrDefault(x=>x.Product__Id==id && x.Product__Status==(int)Status.Released,"ProductColors");
                if(p==null)
                {
                    failed.Add($"No results found for product with ID = {id}");
                    continue;
                }
                
                    p.Product__Status=(int)Status.Unreleased;
                    var result=_uow.Product.Update(p);
                    if(result){
                        foreach (var pc in p.ProductColors)
                        {
                            pc.ProductColor__Status=(int)Status.Unreleased;
                            _uow.ProductColor.Update(pc);
                        }
                    }
                    if(!result)
                    failed.Add($"Fail to stop publishing product {p.Product__Name}");
                   
            }
            return  !failed.Any()?Ok(new {Message="Stop publishing products successfully"}):Ok(failed);
                        
            }
            catch (System.Exception e)
            {
                
                return BadRequest(e);
            }
        }
        [HttpPost("create")]
        public IActionResult Create([FromBody]ProductDTO productDto){
            try
            {
                if(!ModelState.IsValid){
                    return BadRequest(new {result=false,message="add product failed"});
                }
                var isExist=_uow.Product.FirstOrDefault(x=>x.Product__Name==productDto.Product__Name );
                if(isExist!=null){
                    if(isExist.Product__Status!= (int)Status.Deleted){
                        return BadRequest("Product is already exist");
                    }
                    isExist.Product__Status=(int)Status.Unreleased;
                    
                    return Ok(new {result=_uow.Product.Update(isExist),message="added product successfully"});
                }
                var product=_mapper.Map<Product>(productDto);
                var isSuccess=_uow.Product.Add(product);
                return isSuccess?Ok(new {result=isSuccess,message="added product successfully"})
                :BadRequest(new {result=isSuccess,message="add product failed"});
            }
            catch (System.Exception e)
            {
                
                return BadRequest(e);
            }
        }
        [HttpPut("update")]
        public IActionResult Update([FromBody]ProductDTO productDto){
            try
            {
                if(!ModelState.IsValid){
                    return BadRequest(new {result=false,message="update product failed"});
                }
                var product=_mapper.Map<Product>(productDto);
                product.Product__UpdatedDate=DateTime.UtcNow;
                var isSuccess=_uow.Product.Update(product);
                return isSuccess?Ok(new {result=isSuccess,message="updated product successfully"})
                :BadRequest(new {result=isSuccess,message="update product failed"});
            }
            catch (System.Exception e)
            {
                
                return BadRequest(e);
            }
        }
        [HttpPatch("delete/{id}")]
        public IActionResult Detele(int id){
            try
            {
                var product=_uow.Product.GetFirstOrDefault(x=>x.Product__Id==id,"ProductColors");
                //changeStatus
                product.Product__Status=(int)Status.Deleted;
                var isSuccess=_uow.Product.Update(product);
                if(isSuccess){
                   
                        foreach (var pc in product.ProductColors)
                        {
                            pc.ProductColor__Status=(int)Status.Deleted;
                            _uow.ProductColor.Update(pc);
                        }
                        return Ok(new {result=isSuccess,message="deleted product successfully"});
                }
                return BadRequest(new {result=isSuccess,message="delete product failed"});
            }
            catch (System.Exception e)
            {
                
                return BadRequest(e);
            }
        }

    }
}