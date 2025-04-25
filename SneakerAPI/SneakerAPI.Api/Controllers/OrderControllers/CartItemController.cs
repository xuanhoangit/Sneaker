using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SneakerAPI.Core.DTOs;
using SneakerAPI.Core.Interfaces;
using SneakerAPI.Core.Models.OrderEntities;

namespace SneakerAPI.Api.Controllers.OrderControllers
{   
    [ApiController]
    [Route("api/cart-items")]
    // [ApiExplorerSettings(IgnoreApi =true)]
    [Authorize(Roles=RolesName.Customer)]//pass
    public class CartItemController : BaseController
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public CartItemController(IUnitOfWork uow, IMapper mapper) : base(uow)
        {
            _uow = uow;
            _mapper = mapper;
        }

        [HttpGet("user-cart")]
        public async Task<IActionResult> GetUserCart()
        {
            try
            {
                var currentAccount = CurrentUser() as CurrentUser;
                if (currentAccount == null)
                    return Unauthorized("User not authenticated.");

                var cartItems = await _uow.CartItem.GetCartItem(currentAccount.AccountId);
                return Ok(cartItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("add")]
        public IActionResult AddItem([FromBody] AddCartDTO cartDTO)
        {
                    System.Console.WriteLine("hahaadddddddddddddddddddddh");
            try
            {
                var currentAccount = CurrentUser() as CurrentUser;
                if (currentAccount == null)
                    return Unauthorized("User not authenticated.");
                if (cartDTO == null)
                    return BadRequest("Invalid input data.");

                var pcs = _uow.ProductColorSize.FirstOrDefault(x => x.ProductColorSize__ProductColorId == cartDTO.ProductColor__Id && x.ProductColorSize__SizeId == cartDTO.Size__Id).ProductColorSize__Id;
                if (pcs == 0)
                {
                    return BadRequest("Item is not available");
                }
                var existingItem = _uow.CartItem.FirstOrDefault(x =>
                    x.CartItem__CreatedByAccountId == currentAccount.AccountId &&
                    x.CartItem__ProductColorSizeId == pcs);

                if (existingItem != null)
                {
                    existingItem.CartItem__Quantity += cartDTO.CartItem__Quantity;
                    if (_uow.CartItem.Update(existingItem))
                        return Ok("Item quantity updated successfully.");
                }
                else
                {
                    var newItem = new CartItem
                    {
                        CartItem__CreatedByAccountId = currentAccount.AccountId,
                        CartItem__ProductColorSizeId = pcs,
                        CartItem__Quantity = cartDTO.CartItem__Quantity,
                        
                     }
                    ;
                    if (_uow.CartItem.Add(newItem))
                        return Ok("Item added successfully.");
                }

                return BadRequest("Failed to add item.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [HttpPatch("item/{item_id}/quantity/{quantity}")]
        public IActionResult UpdateCartItem(int item_id,int quantity){
            try
            {
                if(item_id <= 0)
                    return NotFound("Item not found.");
                var item = _uow.CartItem.Get(item_id);
                item.CartItem__Quantity = quantity;
                if (item.CartItem__Quantity <= 0)
                {
                    _uow.CartItem.Remove(item);
                    return Ok("Item removed successfully.");
                }
                if (_uow.CartItem.Update(item))
                    return Ok("Item updated successfully.");
                return BadRequest("Failed to update cart item.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteItem(int id)
        {
            try
            {
                var currentAccount = CurrentUser() as CurrentUser;
                if (currentAccount == null)
                    return Unauthorized("User not authenticated.");

                var item = _uow.CartItem.Get(id);
                if (item == null || item.CartItem__CreatedByAccountId != currentAccount.AccountId)
                    return NotFound("Item not found or unauthorized.");

                if (_uow.CartItem.Remove(item))
                    return Ok("Item removed successfully.");

                return BadRequest("Failed to remove item.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpDelete]
        public IActionResult DeleteItems([FromBody] List<int> cartItemIds)
        {
            try
            {
                var currentAccount = CurrentUser() as CurrentUser;
                if (currentAccount == null)
                    return Unauthorized("User not authenticated.");

                if (cartItemIds == null || !cartItemIds.Any())
                    return BadRequest("No items provided for deletion.");

                var items = _uow.CartItem.Find(x =>
                    cartItemIds.Contains(x.CartItem__Id) &&
                    x.CartItem__CreatedByAccountId == currentAccount.AccountId).ToList();

                if (!items.Any())
                    return NotFound("No matching items found.");

                if (_uow.CartItem.RemoveRange(items))
                    return Ok("Items removed successfully.");

                return BadRequest("Failed to remove items.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
