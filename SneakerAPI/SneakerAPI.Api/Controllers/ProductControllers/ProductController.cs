using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SneakerAPI.Core.Interfaces;
using SneakerAPI.Core.Models.Filters;
using SneakerAPI.Core.Models.ProductEntities;
using System;
using System.Linq;

namespace SneakerAPI.Api.Controllers
{   
    [ApiController]
    [Route("api/products")]
    public class ProductController : BaseController
    {
        private readonly IUnitOfWork _uow;
        private readonly int PageSize = 20;

        public ProductController(IUnitOfWork uow) : base(uow)
        {
            _uow = uow;
        }

        [HttpGet("status/{status}/page/{page:int?}")]
        public IActionResult GetProductsByStatus(int status, int page = 1)
        {
            try
            {
                var products = _uow.Product
                    .GetAll(x => x.Product__Status == status)
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();

                if (!products.Any())
                    return NotFound("No products found with the given status.");

                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetProduct(int id)
        {
            try
            {
                var product = _uow.Product.Get(id);
                if (product == null)
                    return NotFound("Product not found.");

                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("filter")]
        public IActionResult GetProductsByFilter([FromQuery] ProductFilter filter, int page = 1)
        {
            try
            {
                var products = _uow.Product
                    .GetFilteredProducts(filter)
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize)
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
                    .GetAll(x => productIds.Contains(x.Product__Id))
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize)
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