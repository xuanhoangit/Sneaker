using SneakerAPI.Core.DTOs;
using SneakerAPI.Core.Models;
using SneakerAPI.Core.Models.Filters;
using SneakerAPI.Core.Models.ProductEntities;

namespace SneakerAPI.Core.Interfaces.ProductInterfaces;
public interface IProductRepository : IRepository<Product>
{       

        IQueryable<Product> GetFilteredProducts(ProductFilter filter);
        
}