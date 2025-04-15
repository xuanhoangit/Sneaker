using SneakerAPI.Core.DTOs;
using SneakerAPI.Core.Models.GHN;

namespace SneakerAPI.Core.Interfaces;
public interface IGHN
{   
    Task<GHNResponse> CreateShippingOrderAsync(CreateOrderRequest res);
    Task<GHNResponse> GetOrderDetail(string clientOrderCode);
}