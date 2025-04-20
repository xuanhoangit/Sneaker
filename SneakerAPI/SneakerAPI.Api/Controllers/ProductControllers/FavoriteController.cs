using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SneakerAPI.Core.DTOs;
using SneakerAPI.Core.Interfaces;
using SneakerAPI.Core.Models.ProductEntities;

namespace SneakerAPI.Api.Controllers.ProductControllers
{   
    //pass
    [Route("api/favorites")]
    // [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    [Authorize(Roles=RolesName.Customer)]
    public class FavoriteController : BaseController
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public FavoriteController(IUnitOfWork uow,IMapper mapper) : base(uow)
        {
            _uow = uow;
            _mapper = mapper;
        }
        [HttpGet("")]
        //pass
        public async Task<IActionResult> GetFavoriteProducts()
        {
            try
            {
                var currentAccount = CurrentUser() as CurrentUser;
                if (currentAccount == null)
                    return Unauthorized("User not authenticated.");

               var favoriteProducts = await _uow.Favorite.GetFavorites(currentAccount.AccountId);
                Random random = new Random();
                return Ok(favoriteProducts[random.Next(0, favoriteProducts.Count())]);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        //pass
        [HttpPost("add")]
        public async Task<IActionResult> AddToFavorite([FromBody] AddFavoriteDTO favoriteDTO)
        {
            try
            {
                var currentAccount = CurrentUser() as CurrentUser;
                if (currentAccount == null)
                    return Unauthorized("User not authenticated.");
                if (favoriteDTO == null)
                    return BadRequest("Invalid input data.");
                var productIsExist=_uow.Product.FirstOrDefault(x=>
                x.Product__Id==favoriteDTO.Favorite__ProductId && x.Product__Status==(int)Status.Released);
                if(productIsExist==null)
                    return BadRequest("Product not found or has been discontinued.");

                var existingItem = await _uow.Favorite.FirstOrDefaultAsync(x =>
                    x.Favorite__AccountId == currentAccount.AccountId &&
                    x.Favorite__ProductId == favoriteDTO.Favorite__ProductId);

                if (existingItem != null && existingItem.Favorite__IsDeleted)
                {
                    existingItem.Favorite__IsDeleted = false;
                    if (_uow.Favorite.Update(existingItem))
                        return Ok("Item added to favorites successfully.");
                }
                else
                {   
                    var favorite= new Favorite{
                        Favorite__ProductId=favoriteDTO.Favorite__ProductId,
                        Favorite__IsDeleted=false,
                        Favorite__CreatedDate=DateTime.UtcNow,
                        Favorite__AccountId = currentAccount.AccountId
                    };
                    
       
                    if (await _uow.Favorite.AddAsync(favorite))
                        return Ok("Item added to favorites successfully.");
                }

                return BadRequest("Failed to add item to favorites.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        //pass
        [HttpPatch("delete")]
        public IActionResult Delete(int favor_id){
            try
            {
                var currentAccount = CurrentUser() as CurrentUser;
                if (currentAccount == null)
                    return Unauthorized("User not authenticated.");
                var favor= _uow.Favorite.FirstOrDefault(x=>x.Favorite__Id==favor_id 
                && x.Favorite__AccountId==currentAccount.AccountId);
                if(favor==null){
                    return NotFound(new {message="No results found"});
                }
                favor.Favorite__IsDeleted=true;
                var result=_uow.Favorite.Update(favor);
                if(result){
                    return Ok(new {message="Deleted successfully"});
                }
                return BadRequest(new {message="Failed to delete"});
            }
            catch (System.Exception ex)
            {
                
                 return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    
    }

}