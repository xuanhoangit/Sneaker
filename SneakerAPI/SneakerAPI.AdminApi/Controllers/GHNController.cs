using Microsoft.AspNetCore.Mvc;
using SneakerAPI.AdminApi.Controllers;
using SneakerAPI.Core.DTOs;
using SneakerAPI.Core.Interfaces;
using SneakerAPI.Core.Models.GHN;

[ApiController]
[Route("api/shipping")]
public class GHNController : BaseController
{
    private readonly IGHN _ghn;
    private readonly IUnitOfWork _uow;

    public GHNController(IGHN ghn, IUnitOfWork uow) : base(uow)
    {
        _ghn = ghn;
        _uow = uow;
    }
    [HttpGet("detail")]
    public async Task<IActionResult> GetOrderDetail(string clientOrderCode)
    {
        var result = await _ghn.GetOrderDetail(clientOrderCode);
        return Ok(result);
    }
    [HttpGet("create-order")]
    public async Task<IActionResult> Create([FromQuery]Shipping shipping)

    {   
        var order=_uow.Order.Get(shipping.OrderId);
        var amountDue= order.Order__AmountDue;
        if(amountDue>10000000)
        {
            return BadRequest("Order amount due must not exceed 10.000.000 VND");
        }
        var orderRequest=await _uow.CustomerInfo.GetBuyer(shipping.OrderId);
            orderRequest.ServiceTypeId=shipping.ServiceTypeId;
             orderRequest.PaymentTypeId=shipping.PaymentTypeId;
             orderRequest.CodAmount=order.Order__PaymentStatus==(int)PaymentStatus.Paid?0:(int)amountDue;
             
             orderRequest.Items= await _uow.OrderItem.GetItemDTO(shipping.OrderId);
             orderRequest.RequiredNote="CHOTHUHANG";

             var itemCount=orderRequest.Items.Count();
             if(500*itemCount> 30000)
             {
                 return BadRequest("Weight must not exceed 30kg");
             }
             if(10*itemCount> 150)
             {
                 return BadRequest("Width must not exceed 150cm");
             }
             if(25*itemCount> 150)
             {
                 return BadRequest("Length must not exceed 150cm");
             }
             if(itemCount<2)
             {
                orderRequest.Weight=500;
                orderRequest.Width=10;
                orderRequest.Length=25;
                orderRequest.Height=10;
             }else{
                orderRequest.Weight=500*itemCount;
                orderRequest.Width=10*itemCount;
                orderRequest.Length=25*itemCount;
                orderRequest.Height=10*itemCount;
             }
             orderRequest.PickShift=[3,4];
            //test
             orderRequest.InsuranceValue=(int)amountDue/2;
             orderRequest.ServiceId=0;
             orderRequest.ClientOrderCode="";
             orderRequest.Content="Theo New York Times";
             orderRequest.PickStationId=null;
             orderRequest.CodFailedAmount=2000;
             orderRequest.Coupon=(string)null;
            //test
             orderRequest.ReturnAddress=null;
             orderRequest.ReturnPhone="0368154633";
             orderRequest.ReturnDistrictId=(int?)null;
             orderRequest.ReturnWardCode="";
             orderRequest.Note="Giày thời trang";
             orderRequest.DeliverStationId=(int?)null;

        
        var result = await _ghn.CreateShippingOrderAsync(orderRequest);
        order.OrderCode=result.data.order_code;
        _uow.Order.Update(order);
        return Ok(result);
    }
}
