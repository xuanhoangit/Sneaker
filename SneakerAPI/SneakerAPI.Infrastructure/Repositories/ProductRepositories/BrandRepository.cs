using SneakerAPI.Core.Interfaces.ProductInterfaces;
using SneakerAPI.Core.Models.ProductEntities;
using SneakerAPI.Infrastructure.Data;

namespace SneakerAPI.Infrastructure.Repositories.ProductRepositories;
public class BrandRepository :Repository<Brand> ,IBrandRepository
{
    public BrandRepository(SneakerAPIDbContext db):base(db)
    {
        
    }

}