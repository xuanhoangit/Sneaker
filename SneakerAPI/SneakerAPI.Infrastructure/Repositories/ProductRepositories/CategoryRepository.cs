using SneakerAPI.Core.Interfaces;
using SneakerAPI.Core.Interfaces.ProductInterfaces;
using SneakerAPI.Core.Models;
using SneakerAPI.Core.Models.ProductEntities;
using SneakerAPI.Infrastructure.Data;

namespace SneakerAPI.Infrastructure.Repositories.ProductRepositories;
public class CategoryRepository :Repository<Category> ,ICategoryRepository
{
    public CategoryRepository(SneakerAPIDbContext db):base(db)
    {
        
    }

}