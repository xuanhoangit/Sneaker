
using SneakerAPI.Core.Models.UserEntities;

namespace SneakerAPI.Core.Interfaces.UserInterfaces
{
    public interface IAddressRepository : IRepository<Address>
    {
        int GetAddressStoreAt();
        List<int> GetRandomAddressIdByAccountId(int accountId);
    }
}