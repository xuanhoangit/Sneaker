using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SneakerAPI.AdminApi.Controllers;
using SneakerAPI.Core.DTOs;
using SneakerAPI.Core.Interfaces;
using SneakerAPI.Core.Libraries;
using SneakerAPI.Core.Models.UserEntities;

namespace SneakerAPI.Api.Controllers.UserControllers
{   
    [Area("Dashboard")]
    [Route("[Area]/[controller]")]
    [ApiController]
    [Authorize(Roles = RolesName.Staff)]
    public class StaffInfomationController : BaseController
    {
        private readonly IUnitOfWork _uow;
        
        private readonly string uploadFilePath=Path.Combine(Directory.GetCurrentDirectory(),"wwwroot/uploads/avatars");
        
        public StaffInfomationController(IUnitOfWork uow) : base(uow)
        {
            _uow = uow;
        }

        [HttpGet("profile")]
        
        public IActionResult GetStaffInfomation(int staffInfo)
        {
            try
            {
                var staff = _uow.StaffInfo.Find(x=>x.StaffInfo__AccountId==staffInfo);
                if(staff == null)
                {
                    return NotFound("User profile not found");
                }
                return Ok(staff);
            }
            catch (System.Exception)
            {
                
                throw new Exception("An error occurred while getting user profile");
            }
        }
        [HttpPatch("profile-update")]
        public async Task<IActionResult> UpdateStaffInfomation(StaffInfoDTO staffInfoDTO)
        {
           try
           {    
                if(staffInfoDTO==null){
                    return BadRequest("Invalid staff information provided");
                 }
                var _staffInfo = _uow.StaffInfo.FirstOrDefault(x=>x.StaffInfo__AccountId==staffInfoDTO.StaffInfo__Id);
                if(_staffInfo == null)
                {
                    return NotFound("User profile not found");
                }
                // Cập nhật chỉ những trường có giá trị hợp lệ
                if (!string.IsNullOrEmpty(staffInfoDTO.StaffInfo__FirstName))
                    _staffInfo.StaffInfo__FirstName = staffInfoDTO.StaffInfo__FirstName;

                if (!string.IsNullOrEmpty(staffInfoDTO.StaffInfo__LastName))
                    _staffInfo.StaffInfo__LastName = staffInfoDTO.StaffInfo__LastName;

                if (staffInfoDTO.StaffInfo__AccountId > 0)
                    _staffInfo.StaffInfo__AccountId = staffInfoDTO.StaffInfo__AccountId;

                if (!string.IsNullOrEmpty(staffInfoDTO.StaffInfo__Phone))
                    _staffInfo.StaffInfo__Phone = staffInfoDTO.StaffInfo__Phone;


                // Chỉ cập nhật avatar nếu cần

                if (staffInfoDTO.File!=null){
                    _staffInfo.StaffInfo__Avatar = Guid.NewGuid().ToString() + ".jpg";
                }
                var result=_uow.StaffInfo.Update(_staffInfo);
                if(result){
                    var isSuccess=await HandleFile.Upload(staffInfoDTO.File,Path.Combine(uploadFilePath,_staffInfo.StaffInfo__Avatar));
                }
                return Ok("Updated profile successfully");
           }
           catch (System.Exception)
           {
                return BadRequest("An error occurred while updating user profile");
           }
        }

    }
}
