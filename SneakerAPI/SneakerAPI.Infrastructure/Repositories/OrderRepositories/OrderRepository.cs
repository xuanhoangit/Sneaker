
using Microsoft.EntityFrameworkCore;
using SneakerAPI.Core.DTOs;
using SneakerAPI.Core.Interfaces.OrderInterfaces;
using SneakerAPI.Core.Models.Filters;
using SneakerAPI.Core.Models.GHN;
using SneakerAPI.Core.Models.OrderEntities;
using SneakerAPI.Infrastructure.Data;

namespace SneakerAPI.Infrastructure.Repositories.OrderRepositories;
public class OrderRepository :Repository<Order> ,IOrderRepository
{

    public OrderRepository(SneakerAPIDbContext db):base(db)
    {
    }



    public IQueryable<Order> GetOrderFiltered(OrderFilter filter)
    {   
        var query=_dbSet.AsQueryable();
        if(filter.RangePrice != null){
            if(filter.RangePrice.MinPrice.HasValue){
                query= query.Where(x=>x.Order__AmountDue>=filter.RangePrice.MinPrice);
            }
            if(filter.RangePrice.MaxPrice.HasValue ){
                query= query.Where(x=>x.Order__AmountDue<=filter.RangePrice.MaxPrice);
            }   
        }
        if(filter.RangeDateTime != null)
        {
            if(filter.RangeDateTime.From.HasValue){
                query =query.Where(x=>x.Order__CreatedDate>=filter.RangeDateTime.From);
            }
            if(filter.RangeDateTime.To.HasValue){
                query =query.Where(x=>x.Order__CreatedDate<=filter.RangeDateTime.To);
            }
        }
        if(filter.Account__Id.HasValue){
            query= query.Where(x=>x.Order__CreatedByAccountId==filter.Account__Id);
        }
        if(filter.Order__Status.HasValue){
            query= query.Where(x=>x.Order__Status==filter.Order__Status);
        }
        if(filter.Payment__Status.HasValue){
            query= query.Where(x=>x.Order__PaymentStatus==filter.Payment__Status);
        }

        if(filter.SortBy== (int)Sort.DateIncrease){
            query= query.OrderBy(x=>x.Order__CreatedDate);
        }
        else if(filter.SortBy== (int)Sort.DateDecrease){
            query= query.OrderByDescending(x=>x.Order__CreatedDate);
        }
        else if(filter.SortBy== (int)Sort.PriceIncrease){
            query= query.OrderBy(x=>x.Order__AmountDue);
        }
        else if(filter.SortBy== (int)Sort.DateIncrease){
            query= query.OrderByDescending(x=>x.Order__AmountDue);
        }
        else if(filter.SortBy== (int)Sort.Default){
            query= query.OrderByDescending(x=>x.Order__CreatedDate);
        }
        return query;
    }
    public async Task<List<GetRevenueByDayDto>> GetRevenueDaily(DateTime time)
    {
        var revenueData = await _dbSet
            .Where(o => o.Order__CreatedDate.Date == time.Date)
            .GroupBy(o => o.Order__CreatedDate.Date)
            .Select(g => new GetRevenueByDayDto
            {
                Date = g.Key.Date,
                Revenue = g.Sum(o => o.Order__AmountDue)
            })
            .OrderBy(r => r.Date).ToListAsync();

        return revenueData;
    }
    public async Task<List<GetRevenueByMonthDto>> GetRevenueMonthly(int month,int year)
{
  var revenueData = await _context.Orders
    .Where(o => o.Order__CreatedDate.Year == year && o.Order__CreatedDate.Month == month)
    .GroupBy(o => new { o.Order__CreatedDate.Year, o.Order__CreatedDate.Month }) // Nhóm theo năm + tháng
    .Select(g => new GetRevenueByMonthDto
    {
        Year = g.Key.Year,
        Month = g.Key.Month,
        Revenue = g.Sum(o => o.Order__AmountDue)
    })
    .OrderBy(r => r.Year).ThenBy(r => r.Month)
    .ToListAsync();


    return revenueData;
}

    public async Task<decimal> GetUserSpend(RangeDateTime rangeDateTime,int accountId){
         var ordersAmountDue = _dbSet
            .Where(x=>x.Order__CreatedByAccountId==accountId &&x.Order__Status==(int)OrderStatus.Completed);
            
            if(rangeDateTime!=null){
                if(rangeDateTime.From.HasValue){
                    ordersAmountDue=ordersAmountDue.Where(x=>x.Order__CreatedDate>=rangeDateTime.From.Value);
                }
                if(rangeDateTime.To.HasValue){
                    ordersAmountDue=ordersAmountDue.Where(x=>x.Order__CreatedDate<=rangeDateTime.To.Value);
                }
            }

            if (!ordersAmountDue.Any())
                return -1;

            var totalSpending=await ordersAmountDue.Select(x=>x.Order__AmountDue).ToListAsync();



            return totalSpending.Sum(x=>x);
    }

}