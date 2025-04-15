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
            join p in db.Products on favor.Favorite__ProductId equals p.Product__Id
            join pc in db.ProductColors on p.Product__Id equals pc.ProductColor__ProductId
            // join size in db.Sizes on pcs.ProductColorSize__SizeId equals size.Size__Id 
            join file in db.Files on pc.ProductColor__Id equals file.ProductColorFile__ProductColorId 
            // join product in db.Products on pc.ProductColor__ProductId equals product.Product__Id
            join color in db.Colors on pc.ProductColor__ColorId equals color.Color__Id
            where favor.Favorite__AccountId==account_id && file.ProductColorFile__Main && favor.Favorite__IsDeleted==false 
            select new GetFavoriteDTO {
                        Favorite__ProductId=p.Product__Id,
                        Favorite__Id=favor.Favorite__Id,
                        Color=color.Color__Name,
                        Product__Name=p.Product__Name,
                        Favorite__CreatedByAccountId = favor.Favorite__AccountId,
                        Image = new ProductColorFile{
                            ProductColorFile__Id=file.ProductColorFile__Id,
                            ProductColorFile__Main=file.ProductColorFile__Main,
                            ProductColorFile__ProductColorId=file.ProductColorFile__ProductColorId,
                            ProductColorFile__Name=file.ProductColorFile__Name,
                            ProductColor=pc
                        },
                        Favorite__IsSale=p.Product__Status==(int)Status.Released,
                        Favorite__Message=p.Product__Status!=(int)Status.Released?"Product has been discontinued":""
            };
        return await query.ToListAsync();
        }
 
    }
}