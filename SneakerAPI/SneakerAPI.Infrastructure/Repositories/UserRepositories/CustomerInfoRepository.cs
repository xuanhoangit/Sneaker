using SneakerAPI.Infrastructure.Repositories;
using SneakerAPI.Core.Interfaces.UserInterfaces;
using SneakerAPI.Core.Models.UserEntities;
using SneakerAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace SneakerAPI.Infrastructure.Repositories.UserRepositories
{
    public class CustomerInfoRepository : Repository<CustomerInfo>, ICustomerInfoRepository
    {
        public CustomerInfoRepository(SneakerAPIDbContext context) : base(context)
        {
        }
        public async Task< CreateOrderRequest> GetBuyer(int order_id){
            var query= from order in _context.Orders 
            join account in _context.Users on order.Order__CreatedByAccountId equals account.Id
            join customer in _context.CustomerInfos on order.Order__CreatedByAccountId equals customer.CustomerInfo__AccountId 
            join address in _context.Addresses on order.Order__AddressId equals address.Address__Id 
            where order.Order__Id==order_id 
            select new CreateOrderRequest{
                            ToName=customer.CustomerInfo__FirstName + " " + customer.CustomerInfo__LastName,
                            ToPhone=address.Address__Phone==null? customer.CustomerInfo__Phone:address.Address__Phone,
                            ToAddress=address.Address__AddressDetail,
                            ToDistrictId=address.Address__DistrictId,
                            ToWardCode=address.Address__WardCode,
                            ToProvinceName=address.Address__ProvinceName,
                        };
                        
            return await query.FirstOrDefaultAsync();

        }
 
    }
}