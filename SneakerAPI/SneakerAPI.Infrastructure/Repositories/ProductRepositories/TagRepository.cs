
using SneakerAPI.Core.Interfaces.ProductInterfaces;
using SneakerAPI.Core.Models.ProductEntities;
using SneakerAPI.Infrastructure.Data;

namespace SneakerAPI.Infrastructure.Repositories.ProductRepositories;
public class TagRepository :Repository<Tag> ,ITagRepository
{
    public TagRepository(SneakerAPIDbContext db):base(db)
    {
        
    }

}