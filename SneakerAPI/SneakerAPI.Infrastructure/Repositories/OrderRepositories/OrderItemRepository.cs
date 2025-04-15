using Microsoft.EntityFrameworkCore;
using SneakerAPI.Core.DTOs;
using SneakerAPI.Core.Interfaces.OrderInterfaces;
using SneakerAPI.Core.Models.GHN;
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
    public async Task<GetOrderItemDTO> GetOrderItem(int orderItem_id){
        var query= from orderItem in db.OrderItems 
            join pcs in db.ProductColorSizes on orderItem.OrderItem__ProductColorSizeId equals pcs.ProductColorSize__Id
            join pc in db.ProductColors on pcs.ProductColorSize__ProductColorId equals pc.ProductColor__Id
            join size in db.Sizes on pcs.ProductColorSize__SizeId equals size.Size__Id 
            join file in db.Files on pc.ProductColor__Id equals file.ProductColorFile__ProductColorId into fileGroup
            join product in db.Products on pc.ProductColor__ProductId equals product.Product__Id
            join color in db.Colors on pc.ProductColor__ColorId equals color.Color__Id
            where orderItem.OrderItem__Id==orderItem_id 
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
        return await query.FirstOrDefaultAsync();
    }
    public async Task<List<GetOrderItemDTO>> GetOrderItems(int order_id){
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

    public async Task<List<ItemDto>> GetItemDTO(int order_id)
    {
            var query= from orderItem in db.OrderItems 
            join pcs in db.ProductColorSizes on orderItem.OrderItem__ProductColorSizeId equals pcs.ProductColorSize__Id
            join pc in db.ProductColors on pcs.ProductColorSize__ProductColorId equals pc.ProductColor__Id
            join size in db.Sizes on pcs.ProductColorSize__SizeId equals size.Size__Id 
            join product in db.Products on pc.ProductColor__ProductId equals product.Product__Id
            join color in db.Colors on pc.ProductColor__ColorId equals color.Color__Id
            where orderItem.OrderItem__OrderId==order_id 
                        select new ItemDto {
                        Name=product.Product__Name+" - "+color.Color__Name + " - "+size.Size__Value,
                        Price=(int)pc.ProductColor__Price,
                        Quantity=orderItem.OrderItem__Quantity,
                        Weight=500,
                        Length=25,
                        Width=10,
                        Height=10,

            };
        return await query.ToListAsync();
    }
}