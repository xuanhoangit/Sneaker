using Microsoft.EntityFrameworkCore;
using SneakerAPI.Core.DTOs;
using SneakerAPI.Core.Interfaces.OrderInterfaces;
using SneakerAPI.Core.Models.OrderEntities;
using SneakerAPI.Core.Models.ProductEntities;
using SneakerAPI.Infrastructure.Data;

namespace SneakerAPI.Infrastructure.Repositories.OrderRepositories
{
    public class CartItemRepository: Repository<CartItem>,ICartItemRepository
    {
        private readonly SneakerAPIDbContext db;

        public CartItemRepository(SneakerAPIDbContext _db):base(_db)
        {
            db = _db;
        }

        public async Task<List<GetCartItemDTO>> GetCartItem(int account_id,int[]? cartItem_Ids=null){
            var query= from cart in db.CartItems 
            join pcs in db.ProductColorSizes on cart.CartItem__ProductColorSizeId equals pcs.ProductColorSize__Id
            join pc in db.ProductColors on pcs.ProductColorSize__ProductColorId equals pc.ProductColor__Id
            join size in db.Sizes on pcs.ProductColorSize__SizeId equals size.Size__Id 
            join file in db.Files on pc.ProductColor__Id equals file.ProductColorFile__ProductColorId into fileGroup
            join product in db.Products on pc.ProductColor__ProductId equals product.Product__Id
            join color in db.Colors on pc.ProductColor__ColorId equals color.Color__Id
            where cart.CartItem__CreatedByAccountId==account_id 
            select new GetCartItemDTO {
                        CartItem__Id = cart.CartItem__Id,
                        Color=color.Color__Name,
                        Product__Name=product.Product__Name,
                        CartItem__Quantity=cart.CartItem__Quantity,
                        CartItem__ProductColorSizeId=pcs.ProductColorSize__Id,
                        CartItem__CreatedByAccountId = cart.CartItem__CreatedByAccountId,
                        ProductColor = pc,
                        Size =size,
                        Images = fileGroup.ToList(),
                        CartItem__IsSale=pc.ProductColor__Status==(int)Status.Released,
                        CartItem__Message=pc.ProductColor__Status!=(int)Status.Released?"Product has been discontinued":""
            };
            if(cartItem_Ids!=null){
                 query=query.Where(x=>cartItem_Ids.Contains(x.CartItem__Id));
            }
        return await query.ToListAsync();
        }
    }
}