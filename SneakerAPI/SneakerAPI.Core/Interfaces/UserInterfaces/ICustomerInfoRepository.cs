
using SneakerAPI.Core.Models.UserEntities;

namespace SneakerAPI.Core.Interfaces.UserInterfaces
{
    public interface ICustomerInfoRepository : IRepository<CustomerInfo>
    {
        Task< CreateOrderRequest> GetBuyer(int order_id);
    }
}