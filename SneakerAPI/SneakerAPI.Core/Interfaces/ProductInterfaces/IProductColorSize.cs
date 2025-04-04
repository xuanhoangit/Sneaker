using SneakerAPI.Core.Models;
using SneakerAPI.Core.Models.OrderEntities;
using SneakerAPI.Core.Models.ProductEntities;

namespace SneakerAPI.Core.Interfaces.ProductInterfaces;
public interface IProductColorSizeRepository : IRepository<ProductColorSize>
{
        Task<object> OrderLock(int pcs_id,int quantity);
        Task<bool> UpdateQuantity(IEnumerable<OrderItem> orderItems);
        Task<bool> UpdateQuantity(int pcs_id, int quantity);
}