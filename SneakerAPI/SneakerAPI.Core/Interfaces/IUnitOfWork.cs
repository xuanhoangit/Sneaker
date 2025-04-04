//Tạo InterfaceRepo trước
using SneakerAPI.Core.Interfaces.OrderInterfaces;
using SneakerAPI.Core.Interfaces.ProductInterfaces;
using SneakerAPI.Core.Interfaces.UserInterfaces;

namespace SneakerAPI.Core.Interfaces;
public interface IUnitOfWork
{   
    //VNpay
    IVnpay Vnpay{get;}
    //IOrder
    ICartItemRepository CartItem { get; }
    IOrderRepository Order { get; }
    IOrderItemRepository OrderItem { get; }
    //IProduct
    IProductColorFileRepository ProductColorFile { get; }
    IFavoriteRepository Favorite { get; }
    IProductRepository Product { get; }
    IProductTagRepository ProductTag { get; }
    ISizeRepository Size { get; }
    ICategoryRepository Category { get; }
    IBrandRepository Brand { get; }
    IColorRepository Color { get; }
    ITagRepository Tag { get; }
    IProductColorRepository ProductColor { get; }
    IProductColorSizeRepository ProductColorSize { get; }
    IProductCategoryRepository ProductCategory { get; }

    ICustomerInfoRepository CustomerInfo { get; }
    IStaffInfoRepository StaffInfo { get; }
    IAddressRepository Address { get; }
    void Save();
}