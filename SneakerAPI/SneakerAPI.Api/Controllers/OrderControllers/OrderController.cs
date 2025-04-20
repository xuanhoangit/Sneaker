using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SneakerAPI.Api.Controllers;
using SneakerAPI.Core.DTOs;
using SneakerAPI.Core.Interfaces;
using SneakerAPI.Core.Models.Filters;
using SneakerAPI.Core.Models.OrderEntities;
// using StackExchange.Redis;

namespace SneakerAPI.Api.Controllers.OrderControllers
{   
    [ApiController]
    [Route("api/orders")]
    [ApiExplorerSettings(IgnoreApi =true)]
    [Authorize(Roles = RolesName.Customer)]
    //pass 
    public class OrderController : BaseController
    {
        private readonly IUnitOfWork _uow;
        private readonly int unitInAPage=20;

        public OrderController(IUnitOfWork uow) : base(uow)
        {
            _uow = uow;
        }
    
 
        [HttpGet("filter/page/{page}")]
        public async Task<IActionResult> GetOrdersByFilter([FromQuery] OrderFilter filter,int page=1)
        {
            try
            {
            var currentAccount = CurrentUser() as CurrentUser;
            if (currentAccount == null)
                return Unauthorized("User not authenticated.");
            filter.Account__Id=currentAccount.AccountId;

            var orders =await _uow.Order.GetOrderFiltered(filter).Skip((page-1)*unitInAPage).Take(unitInAPage).ToListAsync();

            if (!orders.Any())
                return NotFound("No orders found.");

            return Ok(orders);
                
            }
            catch (System.Exception ex)
            {
                
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
       //Chi tiêu của cá nhân
        [HttpGet("user-spend")]
        public IActionResult GetSpending([FromQuery] RangeDateTime rangeDateTime)
        {
            try
            {
            
            var currentAccount = CurrentUser() as CurrentUser;
            if (currentAccount == null)
                return Unauthorized("User not authenticated.");
            var totalSpending = _uow.Order.GetUserSpend(rangeDateTime,currentAccount.AccountId).Result;
            return Ok(totalSpending);
                   
            }
            catch (System.Exception ex)
            {
                
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [HttpGet("{orderId}/items")]
        public async Task<IActionResult> GetOrderItems(int orderId)
        {
            var currentAccount = CurrentUser() as CurrentUser;
            if (currentAccount == null)
                return Unauthorized("User not authenticated.");

            var order = _uow.Order.FirstOrDefault(x =>
                x.Order__CreatedByAccountId == currentAccount.AccountId && x.Order__Id == orderId);
            // var order = _uow.Order.Find(x=>x.Order__Id==orderId);
            if (order == null)
                return NotFound("Order not found.");

            var orderItems = await _uow.OrderItem.GetOrderItem(orderId);
            return Ok(orderItems);
        }
        [HttpPatch("cancel-order/{orderId:int?}")]
        public async Task<IActionResult> CancelOrder(int orderId){
            try
            {   
                var currentAccount = CurrentUser() as CurrentUser;
                if (currentAccount == null)
                    return Unauthorized("User not authenticated.");
                if(orderId<=0){
                    return NotFound("Order not found.");
                }
                var order=_uow.Order.Get(orderId);
                if(order.Order__CreatedByAccountId!=currentAccount.AccountId){
                    return Unauthorized("User not authorized to cancel this order.");
                }
                if(order==null){
                    return NotFound();
                }
                if( order.Order__Status >= (int)OrderStatus.Delivering ){
                        //Không thể hủy
                        return Ok(new {message="Cannot cancel order. This order has been shipped"});
                }
                if(order.Order__PaymentStatus==(int)PaymentStatus.Unpaid ||
                  order.Order__Status < (int)OrderStatus.Delivering){
                        // Hủy ngay
                        order.Order__Status=(int)OrderStatus.Cancelled;
                        var result=_uow.Order.Update(order);
                        //Lấy ra list items để cập nhật lại số lượng sản phẩm
                        var orderItems=await _uow.OrderItem.GetAllAsync(x=>x.OrderItem__OrderId==orderId);
                        await _uow.ProductColorSize.UpdateQuantity(orderItems);
                        return Ok(new {result,message="Order cancelled"});
                    }
                if(order.Order__PaymentStatus==(int)PaymentStatus.Paid && 
                    order.Order__Status < (int)OrderStatus.Delivering){
                    // HỦy và hoàn tiền
                    order.Order__Status=(int)OrderStatus.Cancelled;
                    order.Order__PaymentStatus=(int)PaymentStatus.Refunding;
                    var result=_uow.Order.Update(order);
                    //Lấy ra list items để cập nhật trả lại số lượng sản phẩm
                    var orderItems=await _uow.OrderItem.GetAllAsync(x=>x.OrderItem__OrderId==orderId);
                    await _uow.ProductColorSize.UpdateQuantity(orderItems);
                    return Ok(new {
                        result,message="Order cancelled! Money will be refunding in a few days"
                    });
                }
                return Ok();
            }
            catch (System.Exception ex)
            {
                
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutOrderDTO checkoutDTO)
        {
            try
            {
                var currentAccount = CurrentUser() as CurrentUser;
                if (currentAccount == null)
                    return Unauthorized("User not authenticated.");

                if (checkoutDTO == null || checkoutDTO.CartItemIds == null || !checkoutDTO.CartItemIds.Any())
                    return BadRequest("Invalid cart items.");

                var cartItems = await _uow.CartItem.GetCartItem(currentAccount.AccountId, checkoutDTO.CartItemIds);

                if (!cartItems.Any())
                    return BadRequest("Cart is empty.");
                var amountDue = cartItems.Sum(c => c.ProductColor.ProductColor__Price * c.CartItem__Quantity);
                if (amountDue > 10000000)
                {
                    return BadRequest("Your order cannot exceed 10.000.000đ. Please split into small orders");
                }
                // Tạo đơn hàng
                var order = new Order
                {
                    Order__CreatedByAccountId = currentAccount.AccountId,
                    Order__CreatedDate=DateTime.UtcNow,
                    Order__AmountDue = amountDue,
                    OrderItems = cartItems.Select(c => new OrderItem
                    {
                        OrderItem__ProductColorSizeId = c.CartItem__ProductColorSizeId,
                        OrderItem__Quantity = c.CartItem__Quantity,
                    }).ToList(),
                    Order__PaymentCode = checkoutDTO.OrderPayment,
                    Order__Type = Form_of_purchase.Online,
                    Order__Status = (int)OrderStatus.Pending,
                    Order__PaymentStatus=(int)PaymentStatus.Unpaid,
                    Order__AddressId=checkoutDTO.AddressId
                };

                var result = _uow.Order.Add(order);
                if (result)
                {   
                    var userIf=_uow.CustomerInfo.Get(currentAccount.AccountId);
                    foreach (var cs in cartItems)
                    {
                        var a=await _uow.ProductColorSize.OrderLock(cs.CartItem__ProductColorSizeId,cs.CartItem__Quantity);
                        System.Console.WriteLine(a);
                         userIf.CustomerInfo__TotalSpent+=cs.CartItem__Quantity* cs.ProductColor.ProductColor__Price;
                    }
                    //Cập nhật user totalspend
                    _uow.CustomerInfo.Update(userIf);
                   
                    // Cập nhật trạng thái giỏ hàng
                    _uow.CartItem.RemoveRange(_uow.CartItem.Find(x => checkoutDTO.CartItemIds.Contains(x.CartItem__Id)));
                    return Ok(new { Message = "Order placed successfully.", OrderId = order.Order__Id });
                }

                return BadRequest("Failed to place order.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
