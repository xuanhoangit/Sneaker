using Microsoft.EntityFrameworkCore;
using SneakerAPI.Core.Interfaces;
using SneakerAPI.Core.Interfaces.ProductInterfaces;
using SneakerAPI.Core.Models;
using SneakerAPI.Core.Models.OrderEntities;
using SneakerAPI.Core.Models.ProductEntities;
using SneakerAPI.Infrastructure.Data;

namespace SneakerAPI.Infrastructure.Repositories.ProductRepositories;
public class ProductColorSizeRepository :Repository<ProductColorSize> ,IProductColorSizeRepository
{
    private readonly SneakerAPIDbContext _db;

    public ProductColorSizeRepository(SneakerAPIDbContext db):base(db)
    {
        _db = db;
    }
    public async Task<bool> UpdateQuantity(IEnumerable<OrderItem> orderItems){
        using var transaction = await _db.Database.BeginTransactionAsync();
        foreach (var ot in orderItems)
        {
            var pcsToUpdate = await _dbSet.FindAsync(ot.OrderItem__ProductColorSizeId);
            if (pcsToUpdate == null)
            {
                return false;
            }
            // Giảm số lượng sản phẩm
            pcsToUpdate.ProductColorSize__Quantity += ot.OrderItem__Quantity;
            _dbSet.Update(pcsToUpdate);
        }
      
        await _db.SaveChangesAsync();

        await transaction.CommitAsync();
        return true;
    }
    public async Task<bool> UpdateQuantity(int pcs_id, int quantity){
        using var transaction = await _db.Database.BeginTransactionAsync();
        var pcs= _dbSet.FindAsync(pcs_id).Result;
        if (pcs == null)
        {
            return false;
        }

        // Giảm số lượng sản phẩm
        pcs.ProductColorSize__Quantity += quantity;
        await _db.SaveChangesAsync();

        await transaction.CommitAsync();
        return true;
    }
    public async Task<object> OrderLock(int pcs_id,int quantity){
        using var transaction = await _db.Database.BeginTransactionAsync();

        var productCS = await _dbSet
            .FromSqlRaw($"SELECT * FROM ProductColorSizes WITH (UPDLOCK, ROWLOCK) WHERE ProductColorSize__Id = {pcs_id}", pcs_id)
            .FirstOrDefaultAsync();

        if (productCS == null || productCS.ProductColorSize__Quantity < quantity)
        {
            return new {message="Sản phẩm đã hết hàng."};
        }

        // Giảm số lượng sản phẩm
        productCS.ProductColorSize__Quantity -= quantity;
        await _db.SaveChangesAsync();

        await transaction.CommitAsync();
        return new {message="Mua hàng thành công!"};

    }
}