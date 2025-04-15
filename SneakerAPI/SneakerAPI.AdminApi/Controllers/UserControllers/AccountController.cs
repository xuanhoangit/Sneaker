using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

using SneakerAPI.Core.DTOs;
using SneakerAPI.Core.Interfaces;
using SneakerAPI.Core.Interfaces.UserInterfaces;
using SneakerAPI.Core.Libraries;
using SneakerAPI.Core.Models;
using SneakerAPI.Core.Models.UserEntities;


namespace SneakerAPI.AdminApi.Controllers.UserController
{   
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/managers")]
    [Authorize(Roles=RolesName.Manager)]
    public class ManagerController : BaseController {
        private readonly UserManager<IdentityAccount> _accountManager;
        private readonly IEmailSender _emailSender;
        private readonly IUnitOfWork _uow;

        public ManagerController(UserManager<IdentityAccount> accountManager,
                                 IEmailSender emailSender,
                                 IUnitOfWork uow):base(uow)
        {
            _accountManager = accountManager;
            _emailSender = emailSender;
            _uow = uow;
        }
        private bool AutoCreateInfo(int account_id){
             //Auto tạo bảng info
                    var staffInfo=new StaffInfo{
                        StaffInfo__AccountId=account_id,
                        StaffInfo__Avatar=HandleString.DefaultImage
                    };
                    return _uow.StaffInfo.Add(staffInfo);
        }
        //Đã test
        [HttpGet("staffs")]
        public async Task<IActionResult> GetStaffAccount(){
            try
            {
                var accounts= await _accountManager.GetUsersInRoleAsync(RolesName.Staff);
                if(accounts==null)
                    return BadRequest("Have not an account!");
                return Ok(accounts);
            }
            catch (System.Exception)
            {
                
                return BadRequest("An error has occurred while getting account");
            }
            
        }
        //Đã test
        [HttpPost("appoint-staff")]
        public async Task<IActionResult> AppointStaff(EmailUser model )
        {
            try
            {
                
            if(string.IsNullOrEmpty(model.Email))
            {
                return BadRequest("Email is required.");
            }
            var account=await _accountManager.FindByEmailAsync(model.Email);
            if(account!=null)
            {   
                var isInRoleStaff=await _accountManager.IsInRoleAsync(account,RolesName.Staff);
                if(isInRoleStaff)
                    return BadRequest("Account has access");
                var rs=await _accountManager.AddToRoleAsync(account,RolesName.Staff);
                if(rs.Succeeded)
                {
                    //Auto tạo bảng 
                    if(null==_uow.StaffInfo.FirstOrDefault(x=>x.StaffInfo__AccountId==account.Id))
                    AutoCreateInfo(account.Id);
                    await _emailSender.SendEmailAsync(account.Email,"Appointment employee",EmailTemplateHtml.RenderEmailNotificationBody(account.UserName,"","You are appointed as a sales person of SLS."));
                }

                return Ok("Granted permission to staff successfully");
            }
           
            return BadRequest(new {message="Account does not exist contact your administrator to grant access"}); 
             }
            catch (System.Exception)
            {
                
                return BadRequest(new {message="An error has occurred white appointed"});
            }
        }
    }
    //--------------------------------------------------------------------------------------------------------------
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/admins")]

    [Authorize(Roles=RolesName.Admin)]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<IdentityAccount> _accountManager;
        private readonly IEmailSender _emailSender;
        private readonly IUnitOfWork _uow;
        private readonly int unitInAPage=20;
        public AdminController(UserManager<IdentityAccount> accountManager,
                                 IEmailSender emailSender,
                                 IUnitOfWork uow)
        {
            _accountManager = accountManager;
            _emailSender = emailSender;
           _uow = uow;
        }

        [HttpPut("role-envoke")]
        public async Task<IActionResult> EnvokeRole([FromBody]EmailUser model){
            try
            {
 
            var account= await _accountManager.FindByEmailAsync(model.Email);
            if(account==null)
                return BadRequest("Account is not exist");
            var isInRole= await _accountManager.IsInRoleAsync(account,RolesName.Manager);
            if(isInRole){
            var result=await _accountManager.RemoveFromRoleAsync(account,RolesName.Manager);
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
       
 
        [HttpGet("list-accounts/roles/{role}/page/{page:int?}")]
        public async Task<IActionResult> GetAccountsByRole(string role,int page){
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

        [HttpGet("list-accounts")]
        public async Task<IActionResult> GetAccountByEmail([FromBody] EmailUser model){
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
        [HttpGet("list-accounts/{accountId:int?}")]
        public async Task<IActionResult> GetAccountById(int accountId){
            try
            {
                var account=await  _accountManager.FindByIdAsync(accountId.ToString());
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
        [HttpPost("appoint-manager")]
        //Bổ nhiệm
        public async Task<IActionResult> AppointManager([FromBody]EmailUser model)
        {
            try
            {
                
            if(string.IsNullOrEmpty(model.Email))
            {
                return BadRequest("Email is required.");
            }
            var account=await _accountManager.FindByEmailAsync(model.Email);
            if(account!=null)
            {
                var isInRole=await _accountManager.IsInRoleAsync(account,RolesName.Manager);
                if(isInRole)
                    return BadRequest("Account has access");
                var rs=await _accountManager.AddToRoleAsync(account,RolesName.Manager);
                if(rs.Succeeded)
                {   
                    if(null==_uow.StaffInfo.FirstOrDefault(x=>x.StaffInfo__AccountId==account.Id))
                    AutoCreateInfo(account.Id);
                    await _emailSender.SendEmailAsync(account.Email,"Appointment employee",EmailTemplateHtml.RenderEmailNotificationBody(account.UserName,"","You are appointed as a manager of SLS."));

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
    [ApiController]
    [Route("api/accounts")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Area("Dashboard")]
    public class AccountController : BaseController{

        private static Dictionary<string,string> _refreshtoken= new ();
        private readonly UserManager<IdentityAccount> _accountManager;
        private readonly SignInManager<IdentityAccount> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IMemoryCache _cache;
        private readonly IJwtService _jwtService;

        public AccountController(UserManager<IdentityAccount> accountManager,
                                 SignInManager<IdentityAccount> signInManager,
                                 IEmailSender emailSender,
                                 IMemoryCache cache,
                                 IJwtService jwtService,
                                 IUnitOfWork uow):base(uow)
        {
            _accountManager = accountManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _cache = cache;
            _jwtService = jwtService;
            
        }

    [HttpPost("new-token")]
    public async Task<IActionResult> RefreshToken([FromBody] TokenResponse model)
    {   

        try
        {
 
        var username = _refreshtoken.FirstOrDefault(predicate: x => x.Value == model.RefreshToken).Key;
        if (string.IsNullOrEmpty(username))
            return Unauthorized("Refresh Token is not valid!");
        var account=await _accountManager.FindByEmailAsync(username);
        var roles=await _accountManager.GetRolesAsync(account);
        var newTokens = (TokenResponse)_jwtService.GenerateJwtToken(account,roles);
        
        // Cập nhật refresh token mới
        _refreshtoken[username] = newTokens.RefreshToken;

        return Ok(newTokens);
                   
        }
        catch (System.Exception e)
        {
            
            return BadRequest(e);
        }
    }



        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto model )
        {
            try
            {
   
            if (string.IsNullOrEmpty(model.Email))
            {
                return BadRequest("Email is required.");
            }

            var account=await _accountManager.FindByEmailAsync(model.Email);
            if (model.Password != model.PasswordComfirm)
            {
                return BadRequest("Password and password confirm do not match");
            }
            if(account!=null)
            {   
                return Ok(new {message="Account already exists contact administrator to grant access"});
            }
            var newAccount = new IdentityAccount
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = false
            };

            var result = await _accountManager.CreateAsync(newAccount, model.Password);
            if (result.Succeeded)
            {   
                // Sinh OTP và lưu vào MemoryCache (hết hạn sau 5 phút)
                var otpCode = HandleString.GenerateVerifyCode();
                _cache.Set(model.Email, otpCode, TimeSpan.FromMinutes(5));

                // Gửi OTP qua email
              
    
                string title="Verify your email address";
                await _emailSender.SendEmailAsync(model.Email, title,
                    EmailTemplateHtml.RenderEmailRegisterBody(model.Email, otpCode));

                var uri=new Uri($"{Request.Scheme}://{Request.Host}/api/manager/register-staff/{newAccount.Email}");
                return Created(uri, new { message = "Create account successfully. Please check your email for verification code.",email=newAccount.Email});

            }
            return BadRequest(result.Errors);
             }
            catch (System.Exception)
            {
                
                return BadRequest("An error has occurred while registering");
            }
        }
 
        [HttpPost("email-confirmation-resend")]
        public async Task<IActionResult> ResendEmailConfirmation(ResendOTPDto model)
        {   
            try
            {
           
            var account = await _accountManager.FindByEmailAsync(model.Email);
            if (account == null )
            {
                return BadRequest("Email does not exist.");
            }

            if (await _accountManager.IsEmailConfirmedAsync(account))
            {
                return BadRequest("Your email has been previously verified.");
            }

            if (!_cache.TryGetValue(model.Email, out string storedOtp))
            {   
                // Sinh OTP và lưu vào MemoryCache (hết hạn sau 5 phút)
                var otpCode = HandleString.GenerateVerifyCode();
                _cache.Set(model.Email, otpCode, TimeSpan.FromMinutes(5));

                // Gửi OTP qua email
                await _emailSender.SendEmailAsync(model.Email, "Confirm email",
                EmailTemplateHtml.RenderEmailRegisterBody(model.Email, otpCode));
                return Ok("Please check your email for verification code.");
            }
            return BadRequest("Please try again in a few minutes.");
                 
            }
            catch (System.Exception)
            {
                
                return BadRequest("An error has occurred while resending email confirmation");
            }
        }
        [HttpPost("email-confirm")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailDto model)
        {   
            try
            {
           
            if (!_cache.TryGetValue(model.Email, out string storedOtp))
            {
                return BadRequest("Mã OTP không hợp lệ hoặc đã hết hạn.");
            }

            if (storedOtp != model.OtpCode)
            {
                return BadRequest("OTP code is incorrect.");
            }

            var account = _accountManager.FindByEmailAsync(model.Email).Result;
            if (account == null)
            {
                return BadRequest("Email does not exist.");
            }

            if (account.EmailConfirmed)
            {
                return BadRequest("Email has been previously verified.");
            }

            // Cập nhật trạng thái xác thực email
            account.EmailConfirmed = true;
            var a=await _accountManager.UpdateAsync(account);

            // Xóa OTP khỏi cache sau khi xác nhận thành công
            _cache.Remove(model.Email);

            return Ok("Email has been successfully verified. Let's contact administrator to grant access");
                 
            }
            catch (System.Exception)
            {
                
                return BadRequest("An error has occurred while confirming email");
            }
        }
        // Đăng nhập
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            try
            {
       
            var account = await _accountManager.FindByEmailAsync(model.Email);
            if (account == null || !await _accountManager.CheckPasswordAsync(account, model.Password))
                return Unauthorized("Invalid email or password");
          
            var roles=await _accountManager.GetRolesAsync(account);
            if(!roles.Any())
                return Unauthorized("Account does not have access");
            if(roles.Count()==1 && roles.Contains(RolesName.Customer)){
                return Unauthorized("Account does not have access");
            }
            if (!await _accountManager.IsEmailConfirmedAsync(account))
                return Unauthorized("Please confirm email before logging in.");

            var result = await _signInManager.PasswordSignInAsync(account, model.Password, false, false);
            if (result.Succeeded)
            {   
                var token = (TokenResponse)_jwtService.GenerateJwtToken(account,roles);
                _refreshtoken[account.Email]=token.RefreshToken;
                return Ok(token);
            }
            return Unauthorized("Invalid login information.");
                     
            }
            catch (System.Exception)
            {
                
                return BadRequest("An error has occurred while logging in");
            }
        }

        [HttpPost("password-forgot")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            try
            {
         
            var account = await _accountManager.FindByEmailAsync(model.Email);
            var role= await _accountManager.GetRolesAsync(account);
            if(role.Count()==1 && role[0]==RolesName.Customer)
                return BadRequest("You do not have access");

            if (account == null)
            {
                // Không tiết lộ thông tin tài khoản có tồn tại hay không
                return Ok("Password has been emailed to youuu");
            }
            var password = HandleString.GenerateRandomString(16);
            // cập nhật account với mật khẩu mới
            account.PasswordHash = _accountManager.PasswordHasher.HashPassword(account, password);
            await _accountManager.UpdateAsync(account);

            await _emailSender.SendEmailAsync(model.Email, "Reset password",
                EmailTemplateHtml.RenderEmailForgotPasswordBody(model.Email, password));
            return Ok("Password has been emailed to you");
                   
            }
            catch (System.Exception)
            {
                
                return BadRequest("An error has occurred while sending email");
            }
        }

        // Đặt lại mật khẩu
        [Authorize(Roles=$"{RolesName.Admin},{RolesName.Manager},{RolesName.Staff}")]
        [HttpPost("password-change")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto model)
        {
            try
            {
            CurrentUser currentAccount=(CurrentUser)CurrentUser();

            if(model.ConfirmNewPassword != model.NewPassword)
                return BadRequest("Your new password and confirm password do not match.");

            var account = await _accountManager.FindByEmailAsync(currentAccount.Email);


            if(currentAccount.Roles.Count()==1 && currentAccount.Roles[0]==RolesName.Customer)
                return BadRequest("You do not have access");

            if (account == null)
                
                return BadRequest("Anomalous behavior was detected"); // Không tiết lộ thông tin người dùng không tồn tại

            var result = await _accountManager.ChangePasswordAsync(account, model.Password, model.NewPassword);
    
            if (result.Succeeded){
                var content="You just changed your password at"+ DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                await _emailSender.SendEmailAsync(currentAccount.Email, "Password changed",
                EmailTemplateHtml.RenderEmailNotificationBody(currentAccount.Email,"Security Warning!", content));
                return Ok("Password changed successfully.");
            }
            return Ok("Wrong account name or password");
                    
            }
            catch (System.Exception)
            {
                
                return BadRequest("An error has occurred while resetting password");
            }
        }
    }
}
