using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SneakerAPI.Core.DTOs;
using SneakerAPI.Core.Interfaces;
using SneakerAPI.Core.Models.Filters;
using SneakerAPI.Core.Models.ProductEntities;

namespace SneakerAPI.Api.Controllers
{   
    [ApiController]
    [Route("api/products")]
    // [ApiExplorerSettings(IgnoreApi =true)]
    public class ProductController : BaseController
    {
        private readonly IUnitOfWork _uow;
        private readonly int unitInAPage = 20;

        public ProductController(IUnitOfWork uow) : base(uow)
        {
            _uow = uow;
        }

      
        [HttpGet("best-sale/page/{page}")]
        public IActionResult GetBestSale([FromQuery]RangeDateTime range,int page=1){
            var query= _uow.Product.GetQuery().AsQueryable();
            if(range!=null){
                if(range.From.HasValue){
                   query= query.Where(x=>x.Product__CreatedDate>=range.From);
                }
                if(range.To.HasValue){
                   query= query.Where(x=>x.Product__CreatedDate<=range.To);
                }
            }
            var bestSale=query.Include(x=>x.ProductColors)
            .OrderByDescending(x=>x.Product__Sold)
            .Select(product=>new Product{
                        Product__Id=product.Product__Id,
                        Product__Name=product.Product__Name,
                        Product__Sold=product.Product__Sold,
                        Product__CreatedDate=product.Product__CreatedDate,
                        ProductColors=product.ProductColors
                    })
                    
                    .Skip((page - 1) * unitInAPage)
                    .Take(unitInAPage)
                    .ToList();
            return Ok(bestSale);
        }
        [HttpGet("new-arrival/page/{page}")]
        public IActionResult GetNewArrival([FromQuery]RangeDateTime range,int page=1){
            var query= _uow.Product.GetQuery().AsQueryable();
            if(range!=null){
                if(range.From.HasValue){
                   query= query.Where(x=>x.Product__CreatedDate>=range.From);
                }
                if(range.To.HasValue){
                   query= query.Where(x=>x.Product__CreatedDate<=range.To);
                }
            }
            var newArrival=query.Include(x=>x.ProductColors)
            .Select(product=>new Product{
                        Product__Id=product.Product__Id,
                        Product__Name=product.Product__Name,
                        Product__Sold=product.Product__Sold,
                        Product__CreatedDate=product.Product__CreatedDate,
                        ProductColors=product.ProductColors
                    })
                    .Skip((page - 1) * unitInAPage)
                    .Take(unitInAPage)
                    .ToList();
            return Ok(newArrival);
        }
        [HttpGet("{id}")]
        public IActionResult GetProduct(int id)
        {
            try
            {   
                if (id <= 0)
                    return NotFound("Product not found.");
                var product = _uow.Product.GetFirstOrDefault(x=>x.Product__Id==id && x.Product__Status==(int)Status.Released,"ProductColors");
                if (product == null)
                    return NotFound("Product not found.");
                var dto= new GetProductDTO{
                        Product__Id=product.Product__Id,
                        Product__Name=product.Product__Name,
                        Product__Sold=product.Product__Sold,
                        ProductColors =product.ProductColors.Select(pc=>new GetProductColorDTO{
                            ProductColor__Name=pc.ProductColor__Name,
                            ProductColor__Description=pc.ProductColor__Description,
                            ProductColor__ColorId=pc.ProductColor__ColorId,
                            ProductColor__Price=pc.ProductColor__Price,
                            ProductColor__Id=pc.ProductColor__Id,
                            ProductColor__Sold=pc.ProductColor__Sold,
                            ProductColor__ProductId =pc.ProductColor__ProductId,
                        }).ToList()

                };


                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        //pass
        [HttpGet("filter")]
        public IActionResult GetProductsByFilter([FromQuery] ProductFilter filter, int page = 1)
        {
            try
            {   
                var query=_uow.Product.GetFilteredProducts(filter);
                if(filter.OrderByDatetime==true){
                    query=query.OrderBy(x=>x.Product__CreatedDate);
                }else{
                    query=query.OrderByDescending(x=>x.Product__CreatedDate);
                }
                var products = query
                    .Include(p=>p.ProductColors)
                    .Where(x=>x.Product__Status==(int)Status.Released)
                    .Select(product=>new Product{
                        Product__Id = product.Product__Id,
                        Product__Name = product.Product__Name,
                        ProductColors = product.ProductColors,
                        Product__Sold = product.Product__Sold
                        // ,
                        // Product__CreatedDate = product.Product__CreatedDate
                    })
                    .Skip((page - 1) * unitInAPage)
                    .Take(unitInAPage)
                    .ToList();

                if (!products.Any())
                    return NotFound("No products found with the given filters.");

                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("category/{categoryId}/page/{page:int?}")]
        public IActionResult GetProductsByCategory(int categoryId, int page = 1)
        {
            try
            {
                var category = _uow.Category.Get(categoryId);
                if (category == null)
                    return NotFound("Category does not exist.");

                var productIds = _uow.ProductCategory
                    .GetAll(x => x.ProductCategory__CategoryId == categoryId)
                    .Select(x => x.ProductCategory__ProductId)
                    .ToList();

                if (!productIds.Any())
                    return NotFound("No products found for this category.");

                var products = _uow.Product
                    .GetAll(x => productIds.Contains(x.Product__Id) && x.Product__Status==(int)Status.Released,"ProductColors")
                    .Skip((page - 1) * unitInAPage)
                    .Take(unitInAPage)
                    .ToList();

                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}