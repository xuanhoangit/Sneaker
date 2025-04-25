using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SneakerAPI.Core.DTOs;
using SneakerAPI.Core.Interfaces;
using SneakerAPI.Core.Models.ProductEntities;

namespace SneakerAPI.AdminApi.Controllers.ProductControllers
{   
    [ApiController]
    [Route("api/product-colors")]
    
    public class ProductColorController : BaseController
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public ProductColorController(IUnitOfWork uow,IMapper mapper):base(uow)
        {   
            _uow = uow;
            _mapper = mapper;
        }   
       [HttpGet("product-color")]
        public IActionResult GetById([FromQuery]int id){
            try
            {   
                if(id<=0)
                    return BadRequest("Stupid query");
                var productColor=_uow.ProductColor.Get(id);
                return productColor!=null?Ok(productColor):NotFound("Not result found");  
            }
            catch (System.Exception e)
            {
                
                return StatusCode(50000000,"An error occur while getting product color"+e.Message);
            }
        }
        [HttpGet("product/{product_id}")]
        public async Task<IActionResult> GetByProductId(int product_id){
            try
            {   
                if(product_id<=0)
                    return BadRequest("Stupid query");
                var productColors=await _uow.ProductColor.GetAllAsync(x=>x.ProductColor__ProductId==product_id);
                if(productColors.Count()==0 || productColors==null)
                return NotFound("No result found");
                return Ok(productColors);
            }
            catch (System.Exception e)
            {
                
                return StatusCode(50000000,"An error occur while getting product color"+e.Message);
            }
        }
        [HttpPost("create")]
        public async Task<IActionResult> Create ([FromBody]ProductColorDTO ProductColorDTO){
            try
            {
                if(!ModelState.IsValid){
                    return BadRequest(new {result=false,message="Add product color failed"});
                }
                var productColor=_mapper.Map<ProductColor>(ProductColorDTO);
                var isSuccess =await _uow.ProductColor.AddAsync(productColor);
                if(!isSuccess)
                    return BadRequest(new {result=isSuccess,message="Add product color failed"});
                return Ok(new {result=isSuccess,message="Added product color successfully"});
            }
            catch (System.Exception e)
            {
                
                return BadRequest(e);
            }
        }
        [HttpPut("update")]
        public IActionResult Update ([FromBody]ProductColorDTO productColorDto){
            try
            {
                // if(!ModelState.IsValid){
                //     return BadRequest(new {result=false,message="Update product color failed"});
                // }
                if (productColorDto.ProductColor__Id == 0)
                {
                    return NotFound("Product Color not found");
                }
                var productColor=_mapper.Map<ProductColor>(productColorDto);
            
                var isSuccess = _uow.ProductColor.Update(productColor);
                return isSuccess? Ok(new {result=isSuccess,message="Updated product color successfully"}):
                BadRequest(new {result=isSuccess,message="Update product color failed"});
            }
            catch (System.Exception e)
            {
                 return BadRequest(e);
            }
        }
        [HttpPatch("publish")]
        public IActionResult Publish([FromBody] List<int> pc_ids)
        {
            try
            {    
                if (pc_ids == null || !pc_ids.Any())
                    return BadRequest("No product color provided");

                var failed = new List<string>();

                foreach (var id in pc_ids)
                {
                    var pc = _uow.ProductColor.FirstOrDefault(x => x.ProductColor__ColorId == id && x.ProductColor__Status == (int)Status.Unreleased);

                    if (pc == null)
                    {
                        failed.Add($"No result found at {id}");
                        continue;
                    }

                    pc.ProductColor__Status = (int)Status.Released;
                    var isSuccess = _uow.ProductColor.Update(pc);

                    if (!isSuccess)
                        failed.Add($"Failed to publish color {id}");
                }

                return !failed.Any()
                    ? Ok(new { message = "Published product color successfully" })
                    : BadRequest(new { errors = failed });
            }
            catch (System.Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
        [HttpPatch("stop-publishing")]
        public IActionResult StopPublishing([FromBody] List<int> pc_ids)
        {
            try
            {    
                if (pc_ids == null || !pc_ids.Any())
                    return BadRequest("No product color provided");

                var failed = new List<string>();

                foreach (var id in pc_ids)
                {
                    var pc = _uow.ProductColor.FirstOrDefault(x => x.ProductColor__ColorId == id && x.ProductColor__Status == (int)Status.Released);

                    if (pc == null)
                    {
                        failed.Add($"No result found at {id}");
                        continue;
                    }

                    pc.ProductColor__Status = (int)Status.Unreleased;
                    var isSuccess = _uow.ProductColor.Update(pc);

                    if (!isSuccess)
                        failed.Add($"Failed to publish color {id}");
                }

                return !failed.Any()
                    ? Ok(new { message = "Published product color successfully" })
                    : BadRequest(new { errors = failed });
            }
            catch (System.Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
       [HttpPatch("delete")]
        public IActionResult Delete ([FromBody]List<int> pc_ids){
           try
           {    
                if(!pc_ids.Any()|| pc_ids==null)
                    return BadRequest("No product color provided");
                var failed=new List<string>();
                foreach (var id in pc_ids)
                {
                    var pc=_uow.ProductColor.FirstOrDefault(x=>x.ProductColor__Id==id&& x.ProductColor__Status!=(int)Status.Deleted);
                    if(pc==null)
                        return NotFound("No result found");
                    pc.ProductColor__Status=(int)Status.Deleted;
                    var isSuccess = _uow.ProductColor.Update(pc);
                    if(isSuccess)
                        failed.Add($"Failed to delete color {pc.Color.Color__Name}");
                }
                    return !failed.Any()? Ok(new {message="Deleted product color successfully"}):
                    BadRequest(failed);
           }
           catch (System.Exception e)
           {
            
                return BadRequest(e);
           }
        }
    }
}