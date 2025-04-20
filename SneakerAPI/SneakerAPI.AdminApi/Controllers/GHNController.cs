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
    [HttpPost]
    public IActionResult ReceiveCallback([FromBody] GHNOrderCallbackModel callback)
    {
        // TODO: Xử lý callback tại đây (ghi log, cập nhật DB,...)
        var order = _uow.Order.FirstOrDefault(x=>x.OrderCode==callback.OrderCode);
        if (order == null)
        {
            return BadRequest("order not found");
        }
        var status = (int)GHNStatusMapper.MapToOrderStatus(callback.Status);
        if (status != 400)
        {
            order.Order__Status = status;
            _uow.Order.Update(order);
        }
        

        Console.WriteLine($"📦 Callback GHN - Mã đơn: {callback.OrderCode}, Trạng thái: {callback.Status}, Tổng phí: {callback.TotalFee}");

        return Ok(callback);
    }
    [HttpGet("detail")]
    public async Task<IActionResult> GetOrderDetail(string order_code)
    {
        var result = await _ghn.GetOrderDetail(order_code);
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
             orderRequest.RequiredNote=shipping.RequiredNote;

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
        if (result == null)
        {
            return BadRequest("Somethings went wrong, maybe your address invalid. Please try again with another address");
        }
        order.OrderCode=result.data.order_code;
        _uow.Order.Update(order);
        return Ok(result);
    }
}
