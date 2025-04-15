
using SneakerAPI.Core.DTOs;
using SneakerAPI.Core.Models.GHN;
using SneakerAPI.Core.Models.OrderEntities;

namespace SneakerAPI.Core.Interfaces.OrderInterfaces
{
    public interface IOrderItemRepository : IRepository<OrderItem>
    {
        Task<List<GetOrderItemDTO>> GetOrderItems(int order_id);
        Task<List<ItemDto>> GetItemDTO(int order_id);
        Task<GetOrderItemDTO> GetOrderItem(int orderItem_id);
    }
}