using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using SneakerAPI.Core.DTOs;
using SneakerAPI.Core.Interfaces;
using SneakerAPI.Core.Libraries;
using SneakerAPI.Core.Models;
using SneakerAPI.Core.Models.UserEntities;

namespace SneakerAPI.AdminApi.Controllers.UserController;
  [ApiController]
    // [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/admins")]

    [Authorize(Roles=RolesName.Admin)]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<IdentityAccount> _accountManager;
        private readonly RoleManager<IdentityAccount> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly IUnitOfWork _uow;
        private readonly int unitInAPage=20;
        public AdminController(UserManager<IdentityAccount> accountManager,
                                RoleManager<IdentityAccount> roleManager,
                                 IEmailSender emailSender,
                                 IUnitOfWork uow)
        {
            _accountManager = accountManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
           _uow = uow;
        }
       
        [HttpPut("role-envoke")]
        public async Task<IActionResult> EnvokeRole([FromBody]UserRole model){
            try
            {
 
            var account= await _accountManager.FindByEmailAsync(model.Email);
            if(account==null)
                return BadRequest("Account is not exist");
            var isInRole= await _accountManager.IsInRoleAsync(account,model.Role);
            if(isInRole){
            var result=await _accountManager.RemoveFromRoleAsync(account,model.Role);
                if(result.Succeeded){
                await _emailSender.SendEmailAsync(account.Email,"Terminate the contract",EmailTemplateHtml.RenderEmailNotificationBody(account.UserName,"","You are no longer an SLS salesperson."));
                return Ok(new {result.Succeeded,Message="Manager rights revoked"});
                }
            }
            return BadRequest("Account is not in role Manager");
                           
            }
            catch (System.Exception e)
            {
                
                return BadRequest(e);
            }
        }
        [HttpPut("unlock-account")]
        public async Task<IActionResult> UnlockAccount([FromBody]EmailUser model)
        {
            try
            {
                var account =await  _accountManager.FindByEmailAsync(model.Email);
                if (account == null)
                    return BadRequest("Account does not exist.");

                account.LockoutEnabled = false;
                account.LockoutEnd = null;
                await _accountManager.UpdateAsync(account);
                return Ok("Account has been unlocked.");
            }
            catch (System.Exception)
            {
                
                return BadRequest("An error has occurred while blocking account");
            }
        }
       
        [HttpPut("block-account")]
    
        public async Task<IActionResult> BlockAccount([FromBody]EmailUser model)
        {
            try
            {
                var account =await  _accountManager.FindByEmailAsync(model.Email);
                if (account == null)
                    return BadRequest("Account does not exist.");

                account.LockoutEnabled = true;
                account.LockoutEnd = DateTimeOffset.MaxValue;
                await _accountManager.UpdateAsync(account);
                return Ok("Account has been blocked.");
            }
            catch (System.Exception)
            {
                
                return BadRequest("An error has occurred while blocking account");
            }
        }
       
 
        [HttpGet("accounts/role/{role}/page/{page:int}")]
        public async Task<IActionResult> GetAccountsByRole(string role,int page=1){
            try
            {
                var accounts= (await _accountManager.GetUsersInRoleAsync(role)).Skip(unitInAPage*page).Take(unitInAPage);
                if(accounts==null)
                    return BadRequest("Have not an account!");
                return Ok(accounts);
            }
            catch (System.Exception)
            {
                
                return BadRequest("An error has occurred while getting account");
            }
            
        }

        [HttpGet("account")]
        public async Task<IActionResult> GetAccountByEmail([FromQuery] EmailUser model){
            try
            {
                var account=await  _accountManager.FindByEmailAsync(model.Email);
                if(account==null)
                    return BadRequest("Account not found");
                return Ok(account);
            }
            catch (System.Exception)
            {
                
                return BadRequest("An error has occurred while getting account");
            }
            
        }
        [HttpGet("account/{id:int}")]
        public async Task<IActionResult> GetAccountById(int id){
            try
            {
                var account=await  _accountManager.FindByIdAsync(id.ToString());
                if(account==null)
                    return NotFound("Account not found");
                return Ok(account);
            }
            catch (System.Exception)
            {
                
                return BadRequest("An error has occurred while getting account");
            }
            
        }

        private bool AutoCreateInfo(int account_id){
                    var staffInfo=new StaffInfo{
                    StaffInfo__AccountId=account_id,
                    StaffInfo__Avatar=HandleString.DefaultImage
                };
                return _uow.StaffInfo.Add(staffInfo);
        }
        [HttpPatch("appoint")]
        //Bổ nhiệm
        public async Task<IActionResult> AppointEmployees([FromBody]UserRole model)
        {
            try
            {
                
            if(string.IsNullOrEmpty(model.Email))
            {
                return BadRequest("Email is required.");
            }
            var account=await _accountManager.FindByEmailAsync(model.Email);
            if (account != null)
                {
                    var role = await _roleManager.RoleExistsAsync(model.Role);
                    if (!role)
                    {
                        return BadRequest($"Role '/{model.Role}'/ is not exist");
                    }
                    var isInRole = await _accountManager.IsInRoleAsync(account, model.Role);
                    if (isInRole)
                        return BadRequest("Account has access");
                    var rs = await _accountManager.AddToRoleAsync(account, model.Role);
                    if (rs.Succeeded)
                    {
                        if (null == _uow.StaffInfo.FirstOrDefault(x => x.StaffInfo__AccountId == account.Id))
                            AutoCreateInfo(account.Id);
                        await _emailSender.SendEmailAsync(account.Email, "Appointment employee", EmailTemplateHtml.RenderEmailNotificationBody(account.UserName, "", $"You are appointed as a {model.Role} of SLS."));

                    }
                    return Ok("Granted permission to manager successfully");
                }

         
            return BadRequest(new {message="Account does not exist contact administrator to grant access"});    
             }
            catch (System.Exception)
            {
                
                return BadRequest(new {message="An error has occurred white appointed"});
            }
        }
    }
 
 