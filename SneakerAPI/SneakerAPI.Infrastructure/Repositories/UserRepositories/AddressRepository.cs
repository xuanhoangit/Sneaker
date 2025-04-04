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
    }
}