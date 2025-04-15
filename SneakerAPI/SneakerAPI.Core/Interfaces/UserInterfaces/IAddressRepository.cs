
using SneakerAPI.Core.Models.UserEntities;

namespace SneakerAPI.Core.Interfaces.UserInterfaces
{
    public interface IAddressRepository : IRepository<Address>
    {
        List<int> GetRandomAddressIdByAccountId(int accountId);
    }
}