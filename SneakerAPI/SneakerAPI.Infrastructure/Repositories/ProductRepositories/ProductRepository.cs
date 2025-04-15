using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SneakerAPI.Core.DTOs;
using SneakerAPI.Core.Interfaces;
using SneakerAPI.Core.Interfaces.ProductInterfaces;
using SneakerAPI.Core.Models;
using SneakerAPI.Core.Models.Filters;
using SneakerAPI.Core.Models.ProductEntities;
using SneakerAPI.Infrastructure.Data;

namespace SneakerAPI.Infrastructure.Repositories.ProductRepositories;
public class ProductRepository :Repository<Product> ,IProductRepository
{
    private readonly SneakerAPIDbContext _db;

    // protected_dbSet<Product> _productSet;

    public ProductRepository(SneakerAPIDbContext db):base(db)
    {
        _db =db;
    }

    public IQueryable<Product> GetFilteredProducts(ProductFilter filter)
    {
        var query =_dbSet.AsQueryable();

        if(!filter.SearchString.IsNullOrEmpty()){
            query=query.Where(x=>
            x.Product__Name.ToLower().Contains(filter.SearchString.ToLower()) ||
            filter.SearchString.ToLower().Contains(x.Product__Name.ToLower()) ||

            x.Product__Description.ToLower().Contains(filter.SearchString.ToLower()) ||
            filter.SearchString.ToLower().Contains(x.Product__Description.ToLower()) 
            
            );
        }
         // Lọc theo Brand (thương hiệu)
        if (filter.BrandIds != null && filter.BrandIds.Any())
        {
            query = query.Where(p => filter.BrandIds.Contains(p.Product__BrandId));
        }
        if(filter.CreatedDate.HasValue)
        {
            query = query.Where(p => p.Product__CreatedDate.Value.Date == filter.CreatedDate.Value.Date);
        }
        if(filter.FromDate.HasValue)
        {
            query = query.Where(p => p.Product__CreatedDate.Value.Date >= filter.FromDate.Value.Date);
        }
       if(filter.FromDate.HasValue)
        {
            query = query.Where(p => p.Product__CreatedDate >= filter.FromDate);
        }
        // Lọc theo Color (màu sắc)
        if (filter.ColorIds != null && filter.ColorIds.Any())
        {
            query = query.Where(p => p.ProductColors.Any(pc => filter.ColorIds.Contains(pc.ProductColor__ColorId)));
        }

        // Lọc theo Size (kích thước)
        if (filter.SizeIds != null && filter.SizeIds.Any())
        {
            query = query.Where(p => p.ProductColors
                .Any(pc => pc.ProductColorSizes
                .Any(pcs => filter.SizeIds.Contains(pcs.ProductColorSize__SizeId))));
        }

        // Lọc theo khoảng giá
        if (filter.RangePrice != null)
        {
            if (filter.RangePrice.MinPrice.HasValue)
            {
                query = query.Where(p => p.ProductColors.Any(pc => pc.ProductColor__Price >= filter.RangePrice.MinPrice.Value));
            }

            if (filter.RangePrice.MaxPrice.HasValue)
            {
                query = query.Where(p => p.ProductColors.Any(pc => pc.ProductColor__Price <= filter.RangePrice.MaxPrice.Value));
            }
        }

        return query;
    }

}