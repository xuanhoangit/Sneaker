
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SneakerAPI.Api.Controllers;
using SneakerAPI.Core.DTOs;
using SneakerAPI.Core.Interfaces;
using SneakerAPI.Core.Models.UserEntities;

namespace SneakerAPI.AdminApi.Controllers.ProductControllers
{   
    [Route("api/addresses")]
    [ApiController]
    [Authorize(Roles = RolesName.Customer)]
    public class AddressController : BaseController
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public AddressController(IUnitOfWork uow,IMapper mapper):base(uow)
        {
            _uow = uow;
            _mapper = mapper;
        }
        
        [HttpGet("user-information/{userInformationId}/addresses")]
        public async Task<IActionResult> GetAddresses(int userInformationId)
        {
            try
            {   
                var currentAccount = CurrentUser() as CurrentUser;
                if (currentAccount == null)
                    return Unauthorized("User not authenticated.");
                var _accountId=_uow.CustomerInfo.Get(userInformationId).CustomerInfo__AccountId;
                if(_accountId!=currentAccount.AccountId)
                    return Unauthorized();
                var addresses = await _uow.Address.GetAllAsync(x => x.Address__CustomerInfo == userInformationId);

                if (addresses == null || !addresses.Any())
                {
                    return NotFound(new { message = "No addresses found for this user information." });
                }

                return Ok(addresses);
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "Error fetching addresses for user information ID {UserInformationId}", userInformationId);
                return StatusCode(500, new { error = "An internal server error occurred." });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetAddress(int id)
        {
            try
            {   
                var address = _uow.Address.Get(id);
                if (address == null)
                {
                    return NotFound(new { message = "Address not found" });
                }
                return Ok(address);
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "Error while fetching address with ID {Id}", id);
                return StatusCode(500, new { error = "An internal server error occurred." });
            }
        }

        [HttpPost("create")]
        public IActionResult CreateAddress([FromBody] AddressDTO addressDTO)
        {
            try
            {   
                if (addressDTO == null)
                {
                    return BadRequest(new { error = "Invalid input" });
                }

                // Kiểm tra nếu đã có địa chỉ mặc định
                var existingDefaultAddress = _uow.Address
                    .GetFirstOrDefault(x => x.Address__IsDefault==true && x.Address__CustomerInfo == addressDTO.Address__CustomerInfo);
            
                var address = _mapper.Map<Address>(addressDTO);

                // Nếu chưa có địa chỉ mặc định, đặt địa chỉ này làm mặc định
                address.Address__IsDefault = existingDefaultAddress == null;

                _uow.Address.Add(address);

                return CreatedAtAction(nameof(GetAddress), new { id = address.Address__Id }, address);
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "Error while creating address");
                return StatusCode(500, new { error = "An internal server error occurred." });
            }
        }

        [HttpPatch("{id}")]
        public IActionResult UpdateAddress(int id, [FromBody] AddressDTO addressDTO)
        {
            try
            {
                if (addressDTO == null)
                {
                    return BadRequest(new { error = "Invalid input" });
                }

                var address = _uow.Address.Get(id);
                if (address == null)
                {
                    return NotFound(new { error = "Address not found" });
                }

                // Cập nhật dữ liệu
                if (!string.IsNullOrEmpty(addressDTO.Address__FullAddress))
                    address.Address__FullAddress = addressDTO.Address__FullAddress;
                if (!string.IsNullOrEmpty(addressDTO.Address__Phone))
                    address.Address__Phone = addressDTO.Address__Phone;
                if (!string.IsNullOrEmpty(addressDTO.Address__ReceiverName))
                    address.Address__ReceiverName = addressDTO.Address__ReceiverName;

                // Nếu cập nhật địa chỉ mặc định
                if (addressDTO.Address__IsDefault ==true && !address.Address__IsDefault==true)
                {
                    // Cập nhật tất cả các địa chỉ khác thành không mặc định
                    var allAddresses = _uow.Address
                        .GetAllAsync(x => x.Address__CustomerInfo == address.Address__CustomerInfo).Result;
                    foreach (var addr in allAddresses)
                    {
                        addr.Address__IsDefault = false;
                    }
                    address.Address__IsDefault = true;
                }

                _uow.Address.Update(address);
                _uow.Save();

                return Ok(new { message = "Address updated successfully" });
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "Error while updating address with ID {Id}", id);
                return StatusCode(500, new { error = "An internal server error occurred." });
            }
        }

    [HttpDelete("{id}")]
    public IActionResult DeleteAddress(int id)
    {
        try
        {
            var address = _uow.Address.Get(id);
            if (address == null)
            {
                return NotFound(new { error = "Address not found" });
            }

            _uow.Address.Remove(address);

            return Ok(new { message = "Address deleted successfully" });
        }
        catch (Exception ex)
        {
            // _logger.LogError(ex, "Error while deleting address with ID {Id}", id);
            return StatusCode(500, new { error = "An internal server error occurred." });
        }
    }
    }
}