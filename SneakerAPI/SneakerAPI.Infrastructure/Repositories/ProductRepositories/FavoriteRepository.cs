using Microsoft.EntityFrameworkCore;
using SneakerAPI.Core.DTOs;
using SneakerAPI.Core.Interfaces.ProductInterfaces;
using SneakerAPI.Core.Models.ProductEntities;
using SneakerAPI.Infrastructure.Data;

namespace SneakerAPI.Infrastructure.Repositories.ProductRepositories
{
    public class FavoriteRepository : Repository<Favorite>, IFavoriteRepository
    {
        private readonly SneakerAPIDbContext db;

        public FavoriteRepository(SneakerAPIDbContext db) : base(db)
        {
            this.db = db;
        }

        public async Task<List<GetFavoriteDTO>> GetFavorites(int account_id){
            var query= from favor in db.Favorites 
            // join pcs in db.ProductColorSizes on favor.CartItem__ProductColorSizeId equals pcs.ProductColorSize__Id
            join pc in db.ProductColors on favor.Favorite__ProductColorId equals pc.ProductColor__Id
            // join size in db.Sizes on pcs.ProductColorSize__SizeId equals size.Size__Id 
            join file in db.Files on pc.ProductColor__Id equals file.ProductColorFile__ProductColorId into fileGroup
            join product in db.Products on pc.ProductColor__ProductId equals product.Product__Id
            join color in db.Colors on pc.ProductColor__ColorId equals color.Color__Id
            where favor.Favorite__AccountId==account_id 
            select new GetFavoriteDTO {
                        Color=color.Color__Name,
                        Product__Name=product.Product__Name,
                        Favorite__CreatedByAccountId = favor.Favorite__AccountId,
                        ProductColor = pc,
                        Images = fileGroup.ToList(),
                        Favorite__IsSale=pc.ProductColor__Status==(int)Status.Released,
                        Favorite__Message=pc.ProductColor__Status!=(int)Status.Released?"Product has been discontinued":""
            };
        return await query.ToListAsync();
        }
 
    }
}