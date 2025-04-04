using SneakerAPI.Core.Interfaces.ProductInterfaces;
using SneakerAPI.Core.Models.ProductEntities;
using SneakerAPI.Infrastructure.Data;

namespace SneakerAPI.Infrastructure.Repositories.ProductRepositories;
public class ProductColorFileRepository :Repository<ProductColorFile> ,IProductColorFileRepository
{
    public ProductColorFileRepository(SneakerAPIDbContext db):base(db)
    {
        
    }

}