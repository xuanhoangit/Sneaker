using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SneakerAPI.Core.DTOs;
using SneakerAPI.Core.Interfaces;
using SneakerAPI.Core.Models.Filters;
using SneakerAPI.Core.Models.OrderEntities;

namespace SneakerAPI.AdminApi.Controllers.OrderControllers
{   
    [ApiController]
    [Route("api/orders")]
    // [Authorize(Roles = RolesName.Customer)]
    public class OrderController : BaseController
    {
        private readonly IUnitOfWork _uow;
        private readonly int unitInAPage=20;

        public OrderController(IUnitOfWork uow) : base(uow)
        {
            _uow = uow;
        }
        
        //Chi tiêu của cá nhân
        [Authorize(Roles=$"{RolesName.Manager},{RolesName.Admin}")]
        [HttpGet("user-spend")]
        public async Task<IActionResult> GetUserSpend([FromQuery] RangeDateTime rangeDateTime,int accountId)
        {
            try
            {
            
                if(rangeDateTime == null || rangeDateTime.From == null || rangeDateTime.To == null)
                        return BadRequest("Invalid date range.");
                if (rangeDateTime.From > rangeDateTime.To)
                        return BadRequest("Invalid date range. From date must be before To date.");
                var totalSpending =await _uow.Order.GetUserSpend(rangeDateTime,accountId);
                if (totalSpending < 0)
                    return NotFound("No spending found.");
                return Ok(totalSpending);
                   
            }
            catch (System.Exception ex)
            {
                
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [Authorize(Roles=$"{RolesName.Manager},{RolesName.Admin}")]
        [HttpGet("monthly-revenue")]
        public async Task<IActionResult> GetRevenueMonthly([FromQuery] int month, int year){
            var result=await _uow.Order.GetRevenueMonthly(month,year);
            return Ok(result);
        }
        [Authorize(Roles=$"{RolesName.Manager},{RolesName.Admin}")]
        [HttpGet("daily-revenue")]
        public async Task<IActionResult> GetRevenueByDaily([FromQuery] DateTime time)
        {
            try
            {
                if(time==null)
                    return BadRequest("Invalid date range.");
                if (time > DateTime.Now)
                    return BadRequest("Invalid date range. From date must be before To date.");
                var revenue = await _uow.Order.GetRevenueDaily(time);
                return Ok(revenue);
            }
            catch (System.Exception ex)
            {
                
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [Authorize(Roles=$"{RolesName.Manager},{RolesName.Admin},{RolesName.Staff}")]
        [HttpGet("filter/page/{page}")]
        public async Task<IActionResult> GetOrdersByFilter([FromQuery] OrderFilter filter,int page=1)
        {
            try
            {
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
        [Authorize(Roles=$"{RolesName.Manager},{RolesName.Admin},{RolesName.Staff}")]
        [HttpGet("{orderId}/items")]
        public async Task<IActionResult> GetOrderItems(int orderId)
        {
            // var currentAccount = CurrentUser() as CurrentUser;
            // if (currentAccount == null)
            //     return Unauthorized("User not authenticated.");

            // var order = _uow.Order.FirstOrDefault(x =>
            //     x.Order__CreatedByAccountId == currentAccount.AccountId && x.Order__Id == orderId);
            var order = _uow.Order.Find(x=>x.Order__Id==orderId);
            if (order == null)
                return NotFound("Order not found.");

            var orderItems = await _uow.OrderItem.GetOrderItem(orderId);
            return Ok(orderItems);
        }
        [HttpPatch("cancel-order/{orderId:int?}")]
        [Authorize(Roles=RolesName.Staff)]
        public async Task<IActionResult> CancelOrder(int orderId){
            try
            {   
                if(orderId<=0){
                    return NotFound("Order not found.");
                }
                var order=_uow.Order.Get(orderId);
                if(order==null){
                    return NotFound();
                }
                if(
                    order.Order__Status == (int)OrderStatus.Completed ||
                    order.Order__Status == (int)OrderStatus.Delivering ||
                    order.Order__Status == (int)OrderStatus.Delivered ){
                        //Không thể hủy
                        return Ok(new {message="Cannot cancel order. This order has been shipped"});
                }
                if(order.Order__PaymentStatus==(int)PaymentStatus.Unpaid ||
                    order.Order__Status==(int)OrderStatus.Pending ||
                    order.Order__Status==(int)OrderStatus.Processing){
                        // Hủy ngay
                        order.Order__Status=(int)OrderStatus.Cancelled;
                        var result=_uow.Order.Update(order);
                        var orderItems=await _uow.OrderItem.GetAllAsync(x=>x.OrderItem__OrderId==orderId);
                        await _uow.ProductColorSize.UpdateQuantity(orderItems);
                        return Ok(new {result,message="Order cancelled"});
                    }
                if(order.Order__PaymentStatus==(int)PaymentStatus.Paid && 
                    order.Order__Status != (int)OrderStatus.Completed &&
                    order.Order__Status != (int)OrderStatus.Delivering &&
                    order.Order__Status != (int)OrderStatus.Delivered){
                    // HỦy và hoàn tiền
                    order.Order__Status=(int)OrderStatus.Cancelled;
                    order.Order__PaymentStatus=(int)PaymentStatus.Refunding;
                    return Ok(new {
                        result=_uow.Order.Update(order),message="Order cancelled! Money will be refunded in a few days"
                    });
                }
                return Ok();
            }
            catch (System.Exception ex)
            {
                
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [Authorize(Roles=RolesName.Staff)]
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

                var cartItems = await _uow.CartItem.GetCartItem(account_id: currentAccount.AccountId, checkoutDTO.CartItemIds);

                if (!cartItems.Any())
                    return BadRequest("Cart is empty.");

                // Tạo đơn hàng
                var order = new Order
                {
                    Order__CreatedByAccountId = currentAccount.AccountId,
                    Order__CreatedDate=DateTime.Now,
                    Order__AmountDue = cartItems.Sum(c => c.ProductColor.ProductColor__Price * c.CartItem__Quantity),
                    OrderItems = cartItems.Select(c => new OrderItem
                    {
                        OrderItem__ProductColorSizeId = c.CartItem__ProductColorSizeId,
                        OrderItem__Quantity = c.CartItem__Quantity,
                    }).ToList(),
                    Order__PaymentCode = checkoutDTO.OrderPayment,
                    Order__Type = Form_of_purchase.Offline,
                    Order__Status = (int)OrderStatus.Pending,
                    Order__PaymentStatus=(int)PaymentStatus.Unpaid
                };

                var result = _uow.Order.Add(order);
                if (result)
                {   
                    foreach (var cs in cartItems)
                    {
                        var a=await _uow.ProductColorSize.OrderLock(cs.CartItem__ProductColorSizeId,cs.CartItem__Quantity);
                        System.Console.WriteLine(a);
                    }
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
