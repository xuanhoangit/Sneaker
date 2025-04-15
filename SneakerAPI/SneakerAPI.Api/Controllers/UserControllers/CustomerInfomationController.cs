using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SneakerAPI.Core.DTOs;
using SneakerAPI.Core.Interfaces;
using SneakerAPI.Core.Libraries;


namespace SneakerAPI.Api.Controllers.UserControllers
{
    [Route("api/users")]
    [ApiController]
    //pass
    [ApiExplorerSettings(IgnoreApi =true)]
    [Authorize(Roles = RolesName.Customer)]
    public class UserInformationController : BaseController
    {
        private readonly IUnitOfWork _uow;
        private readonly string localStorage="uploads/avatars";
        private readonly string _uploadFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/avatars");

        public UserInformationController(IUnitOfWork uow) : base(uow)
        {
            _uow = uow;
        }

        [HttpGet("profile")]
        public IActionResult GetCustomerInformation()
        {
            try
            {
                var currentAccount = CurrentUser() as CurrentUser;
                if (currentAccount == null)
                    return Unauthorized("User not authenticated.");

                var customer = _uow.CustomerInfo.Get(currentAccount.AccountId);
                if (customer == null)
                    return NotFound("User profile not found.");
                customer.CustomerInfo__Avatar=$"{Request.Scheme}://{Request.Host}/{localStorage}/{customer.CustomerInfo__Avatar}";
                return Ok(customer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPatch("profile")]
        public async Task<IActionResult> UpdateCustomerInformation([FromForm] CustomerInfoDTO customerInfoDTO)
        {
            try
            {
                var currentAccount = CurrentUser() as CurrentUser;
                if (currentAccount == null)
                    return Unauthorized("User not authenticated.");

                var customerInfo = _uow.CustomerInfo.Get(currentAccount.AccountId);
                if (customerInfo == null)
                    return NotFound("User profile not found.");

                // Cập nhật thông tin cơ bản
                if (!string.IsNullOrEmpty(customerInfoDTO.CustomerInfo__FirstName))
                    customerInfo.CustomerInfo__FirstName = customerInfoDTO.CustomerInfo__FirstName;

                if (!string.IsNullOrEmpty(customerInfoDTO.CustomerInfo__LastName))
                    customerInfo.CustomerInfo__LastName = customerInfoDTO.CustomerInfo__LastName;

                // Không cho phép cập nhật SpendingPoint và TotalSpent
                // Những giá trị này phải do hệ thống quản lý.

                // Xử lý upload avatar nếu có file mới
                if (customerInfoDTO.File != null)
                {
                    var newAvatarFileName = $"{Guid.NewGuid()}.jpg";
                    var filePath = Path.Combine(_uploadFilePath, newAvatarFileName);

                    var isSuccess = await HandleFile.Upload(customerInfoDTO.File, filePath);
                    if (!isSuccess)
                        return BadRequest("Failed to upload avatar.");

                    // Xóa avatar cũ (nếu cần)
                    if (!string.IsNullOrEmpty(customerInfo.CustomerInfo__Avatar))
                    {
                        var oldFilePath = Path.Combine(_uploadFilePath, customerInfo.CustomerInfo__Avatar);
                        if (System.IO.File.Exists(oldFilePath))
                            System.IO.File.Delete(oldFilePath);
                    }

                    customerInfo.CustomerInfo__Avatar = newAvatarFileName;
                }

                var result = _uow.CustomerInfo.Update(customerInfo);
                if (!result)
                    return BadRequest("Failed to update user profile.");
                 customerInfo.CustomerInfo__Avatar=$"{Request.Scheme}://{Request.Host}/{localStorage}/{customerInfo.CustomerInfo__Avatar}";
                return Ok(new { Message = "Profile updated successfully.", Data = customerInfo });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
