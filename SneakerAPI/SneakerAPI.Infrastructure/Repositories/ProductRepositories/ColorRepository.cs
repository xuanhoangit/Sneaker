using SneakerAPI.Infrastructure.Data;
using SneakerAPI.Core.Interfaces.ProductInterfaces;
using SneakerAPI.Core.Models.ProductEntities;

namespace SneakerAPI.Infrastructure.Repositories.ProductRepositories;
public class ColorRepository :Repository<Color> ,IColorRepository
{
    public ColorRepository(SneakerAPIDbContext db):base(db)
    {
        
    }

}