using SneakerAPI.Core.Interfaces.ProductInterfaces;
using SneakerAPI.Core.Models.ProductEntities;
using SneakerAPI.Infrastructure.Data;

namespace SneakerAPI.Infrastructure.Repositories.ProductRepositories;
public class ProductTagRepository :Repository<ProductTag> ,IProductTagRepository
{
    public ProductTagRepository(SneakerAPIDbContext db):base(db)
    {
        
    }

}