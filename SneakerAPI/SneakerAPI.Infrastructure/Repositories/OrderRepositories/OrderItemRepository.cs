using Microsoft.EntityFrameworkCore;
using SneakerAPI.Core.DTOs;
using SneakerAPI.Core.Interfaces.OrderInterfaces;
using SneakerAPI.Core.Models.OrderEntities;
using SneakerAPI.Infrastructure.Data;

namespace SneakerAPI.Infrastructure.Repositories.OrderRepositories;
public class OrderItemRepository :Repository<OrderItem> ,IOrderItemRepository
{
    private readonly SneakerAPIDbContext db;

    public OrderItemRepository(SneakerAPIDbContext db):base(db)
    {
        this.db = db;
    }
    public async Task<List<GetOrderItemDTO>> GetOrderItem(int order_id){
            var query= from orderItem in db.OrderItems 
            join pcs in db.ProductColorSizes on orderItem.OrderItem__ProductColorSizeId equals pcs.ProductColorSize__Id
            join pc in db.ProductColors on pcs.ProductColorSize__ProductColorId equals pc.ProductColor__Id
            join size in db.Sizes on pcs.ProductColorSize__SizeId equals size.Size__Id 
            join file in db.Files on pc.ProductColor__Id equals file.ProductColorFile__ProductColorId into fileGroup
            join product in db.Products on pc.ProductColor__ProductId equals product.Product__Id
            join color in db.Colors on pc.ProductColor__ColorId equals color.Color__Id
            where orderItem.OrderItem__OrderId==order_id 
                        select new GetOrderItemDTO {
                        OrderItem__Id = orderItem.OrderItem__Id,
                        Color=color.Color__Name,
                        Product__Name=product.Product__Name,
                        ProductColor = pc,
                        Size =size,
                        Images = fileGroup.ToList(),
                        OrderItem__IsSale=pc.ProductColor__Status==(int)Status.Released,
                        OrderItem__Message=pc.ProductColor__Status!=(int)Status.Released?"Product has been discontinued":""
            };
        return await query.ToListAsync();
        }
}