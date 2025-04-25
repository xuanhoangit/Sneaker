using SneakerAPI.Infrastructure.Repositories;
using SneakerAPI.Core.Interfaces.UserInterfaces;
using SneakerAPI.Core.Models.UserEntities;
using SneakerAPI.Infrastructure.Data;

namespace SneakerAPI.Infrastructure.Repositories.UserRepositories
{
    public class AddressRepository : Repository<Address>, IAddressRepository
    {
        public AddressRepository(SneakerAPIDbContext context) : base(context)
        {
        }
        public int GetAddressStoreAt()
        {   
            var query = from address in _context.Addresses
                        join ci in _context.CustomerInfos
            on address.Address__CustomerInfo equals ci.CustomerInfo__Id
                        join ac in _context.Users
            on ci.CustomerInfo__AccountId equals ac.Id
                        where "admin@gmail.com" == ac.Email
                        select address.Address__Id;
            return query.FirstOrDefault();
        }
        public List<int> GetRandomAddressIdByAccountId(int accountId)
        {
            var query = from address in _context.Addresses
                        join ci in _context.CustomerInfos
            on address.Address__CustomerInfo equals ci.CustomerInfo__Id
                        join ac in _context.Users
            on ci.CustomerInfo__AccountId equals ac.Id
                        where accountId == ac.Id
                        select address.Address__Id;
            return query.ToList();
        }

    }
}