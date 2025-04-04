using SneakerAPI.Core.Interfaces.ProductInterfaces;
using SneakerAPI.Core.Models.ProductEntities;
using SneakerAPI.Infrastructure.Data;

namespace SneakerAPI.Infrastructure.Repositories.ProductRepositories;
public class ProductCategoryRepository :Repository<ProductCategory> ,IProductCategoryRepository
{
    public ProductCategoryRepository(SneakerAPIDbContext db):base(db)
    {
        
    }

}