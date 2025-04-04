
using SneakerAPI.Core.Interfaces.ProductInterfaces;
using SneakerAPI.Core.Models.ProductEntities;
using SneakerAPI.Infrastructure.Data;

namespace SneakerAPI.Infrastructure.Repositories.ProductRepositories;
public class SizeRepository :Repository<Size> ,ISizeRepository
{
    public SizeRepository(SneakerAPIDbContext db):base(db)
    {
        
    }

}