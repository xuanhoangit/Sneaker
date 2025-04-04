using SneakerAPI.Infrastructure.Repositories;
using SneakerAPI.Core.Interfaces.UserInterfaces;
using SneakerAPI.Core.Models.UserEntities;
using SneakerAPI.Infrastructure.Data;

namespace SneakerAPI.Infrastructure.Repositories.UserRepositories
{
    public class CustomerInfoRepository : Repository<CustomerInfo>, ICustomerInfoRepository
    {
        public CustomerInfoRepository(SneakerAPIDbContext context) : base(context)
        {
        }
    }
}