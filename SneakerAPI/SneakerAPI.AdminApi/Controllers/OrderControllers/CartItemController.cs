using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SneakerAPI.Core.DTOs;
using SneakerAPI.Core.Interfaces;
using SneakerAPI.Core.Models.OrderEntities;

namespace SneakerAPI.AdminApi.Controllers.OrderControllers
{
    [ApiController]
    [Route("api/cart-items")]
    [Authorize(Roles=RolesName.Staff)]
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
                System.Console.WriteLine(currentAccount.AccountId);
                if (currentAccount == null)
                    return Unauthorized("User not authenticated.");
                if (cartDTO == null)
                    return BadRequest("Invalid input data.");
                // 5 Replace with currentAccount.AccountId
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

                    };// Replace with currentAccount.AccountId

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
