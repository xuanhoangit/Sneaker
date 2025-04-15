using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SneakerAPI.Core.DTOs;
using SneakerAPI.Core.Interfaces;
using SneakerAPI.Core.Models.OrderEntities;

namespace SneakerAPI.AdminApi.Controllers.OrderControllers
{   
    [ApiController]
    [Route("api/cart-items")]
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
            try
            {
                var currentAccount = CurrentUser() as CurrentUser;
                if (currentAccount == null)
                    return Unauthorized("User not authenticated.");
                if (cartDTO == null)
                    return BadRequest("Invalid input data.");
                // 5 Replace with currentAccount.AccountId
                var existingItem = _uow.CartItem.FirstOrDefault(x =>
                    x.CartItem__CreatedByAccountId == currentAccount.AccountId &&
                    x.CartItem__ProductColorSizeId == cartDTO.CartItem__ProductColorSizeId);

                if (existingItem != null)
                {
                    existingItem.CartItem__Quantity += cartDTO.CartItem__Quantity;
                    if (_uow.CartItem.Update(existingItem))
                        return Ok("Item quantity updated successfully.");
                }
                else
                {
                    var newItem = _mapper.Map<CartItem>(cartDTO);
                    newItem.CartItem__CreatedByAccountId = currentAccount.AccountId; // Replace with currentAccount.AccountId

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
