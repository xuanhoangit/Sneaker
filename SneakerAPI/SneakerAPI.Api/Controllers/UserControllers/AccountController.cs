
using System.Security.Claims;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;


using SneakerAPI.Core.DTOs;
using SneakerAPI.Core.Interfaces;
using SneakerAPI.Core.Interfaces.UserInterfaces;
using SneakerAPI.Core.Libraries;
using SneakerAPI.Core.Models;
using SneakerAPI.Core.Models.UserEntities;


namespace SneakerAPI.Api.Controllers.UserControllers
{   
    public class GoogleLoginRequest
{
    public required string Credential { get; set; }
}
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/accounts")]
    public class AccountController : BaseController
    {   
        private static Dictionary<string,string> _refreshtoken= new ();
        private readonly UserManager<IdentityAccount> _accountManager;
        private readonly SignInManager<IdentityAccount> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IMemoryCache _cache;
        private readonly IJwtService _jwtService;
        private readonly IUnitOfWork _uow;


        public AccountController(UserManager<IdentityAccount> accountManager,
                                 SignInManager<IdentityAccount> signInManager,
                                 IEmailSender emailSender,
                                 IMemoryCache cache,
                                 IJwtService jwtService,
                                 IUnitOfWork uow):base (uow)
        {
            _accountManager = accountManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _cache = cache;
            _jwtService = jwtService;
            _uow = uow;
        }

        [Authorize(Roles = RolesName.Customer)]
        [HttpGet("current-user")]
        public IActionResult GetCurrentUser()
        {
            
            return Ok(CurrentUser());
        }          
    
    [HttpPost("new-token")]
    public async Task<IActionResult> RefreshToken( [FromBody]TokenResponse model)
    {   
        try
        {
 
        var username = _refreshtoken.FirstOrDefault( x => x.Value == model.RefreshToken).Key;
        System.Console.WriteLine(username);
        if (string.IsNullOrEmpty(username))
            return Unauthorized("Refresh Token is not valid!");
        var account=await _accountManager.FindByEmailAsync(username);
        var roles=await _accountManager.GetRolesAsync(account);
        var newTokens = (TokenResponse)_jwtService.GenerateJwtToken(account,roles);
        
        // Cập nhật refresh token mới
        _refreshtoken[username] = newTokens.RefreshToken;

        return Ok(newTokens);
                   
        }
        catch (System.Exception ex)
        {
            
            throw;
        }
    }

        private bool AutoCreateInfo(int account_id){
            return _uow.CustomerInfo.Add(new CustomerInfo{
                                CustomerInfo__AccountId=account_id,
                                CustomerInfo__Avatar=HandleString.DefaultImage
                            });
        }
        //past
        [Authorize(Roles = RolesName.Customer)]
        [HttpPatch("password-set")]
        public async Task<IActionResult> SetPassword([FromBody]ChangePasswordDto model)
        {
            try
            {
            var currentAccount= CurrentUser() as CurrentUser;
            if(currentAccount == null){
                return Unauthorized();
            }
            System.Console.WriteLine("hahaha");
            if(model.ConfirmNewPassword != model.NewPassword)
                return BadRequest("Your new password and confirm password do not match.");
                           System.Console.WriteLine("email curenttttttttttt"+currentAccount.Email);

            var account = await _accountManager.FindByEmailAsync(currentAccount.Email);
            System.Console.WriteLine("email curenttttttttttt"+currentAccount.Email);
            if (account == null)
                return BadRequest("Anomalous behavior was detected"); 
            if(!string.IsNullOrEmpty(account.PasswordHash))
                // Nếu tài khoản chưa được thiết lập mật khẩu, trả về thông báo lỗi
                return BadRequest("Your account has been set a password!");
            account.PasswordHash = _accountManager.PasswordHasher.HashPassword(account, model.NewPassword);
            var result=await _accountManager.UpdateAsync(account);
            // var result = await _accountManager.ChangePasswordAsync(account, model.Password, model.NewPassword);
    
            if (result.Succeeded){
                var content="You just changed your password at "+ DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                await _emailSender.SendEmailAsync(currentAccount.Email, "Password changed",
                EmailTemplateHtml.RenderEmailNotificationBody(currentAccount.Email,"Security Warning!", content));
                return Ok("Password changed successfully.");
            }
            return BadRequest(result.Errors);
                    
            }
            catch (System.Exception ex)
            {
                
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        //pass
        [HttpPost("google-signin")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Credential))
                    return BadRequest("Missing Google Token");

                var payload = await GoogleJsonWebSignature.ValidateAsync(request.Credential, new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { "429739552674-jgb94p4nggngssiksimcgqith8evpo98.apps.googleusercontent.com" }
                });

                var account = await _accountManager.FindByEmailAsync(payload.Email);

                if (account != null)
                {  
                       // Đăng nhập user vào hệ thống
                        var roles=await _accountManager.GetRolesAsync(account);
                        if(!roles.Contains(RolesName.Customer)){
                            var rs=await _accountManager.AddToRoleAsync(account,RolesName.Customer);
                            if(rs.Succeeded){
                                //Auto create customer info
                                if(_uow.CustomerInfo.FirstOrDefault(x=>x.CustomerInfo__AccountId==account.Id) == null)
                                AutoCreateInfo(account.Id);
                            }
                        }
                        await _signInManager.SignInAsync(account, isPersistent: true);
                        var token = (TokenResponse)_jwtService.GenerateJwtToken(account,roles);
                        // var currentAccount= CurrentUser() as CurrentUser;
                        // System.Console.WriteLine(currentAccount);
                        _refreshtoken[account.Email]=token.RefreshToken;
                        return Ok(token);
                }

                 var newAccount=new IdentityAccount{
                        Email=payload.Email,
                        UserName=payload.Email,
                        EmailConfirmed=true
                    };
                    var result=await _accountManager.CreateAsync(newAccount);
                    if(result.Succeeded)
                    {
                        await _accountManager.AddToRoleAsync(newAccount, RolesName.Customer);
                        //Auto create customer info
                        AutoCreateInfo(newAccount.Id);
                        await _signInManager.SignInAsync(newAccount, isPersistent: true);

                        await _emailSender.SendEmailAsync(newAccount.Email, "Notification",
                            EmailTemplateHtml.RenderEmailNotificationBody(newAccount.Email,"Login Notice", "You have just successfully logged into the Sneaker Luxury Store app."));
                        var uri=new Uri($"{Request.Scheme}://{Request.Host}/api/accounts/google-signin");
                        
                        var token = (TokenResponse)_jwtService.GenerateJwtToken(newAccount,new List<string>{
                            RolesName.Customer
                        });
                        // var currentAccount= CurrentUser() as CurrentUser;
                        _refreshtoken[newAccount.Email]=token.RefreshToken;
                        return Created(uri,token);
                    }
                return BadRequest(new { error = "Google token is not valid!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Google token is not valid!", message = ex.Message });
            }
        }

        //pass
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            try
            {

            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Email and password are required.");
            }
            var accountExist = await _accountManager.FindByEmailAsync(model.Email);
            if (accountExist != null)
            {
                return BadRequest("Email already exists.");
            }
            if (model.Password != model.PasswordComfirm)
            {
                return BadRequest("Password and password confirm do not match");
            }

            var account = new IdentityAccount
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = false
            };

            var result = await _accountManager.CreateAsync(account, model.Password);
            if (result.Succeeded)
            {
                // Sinh OTP và lưu vào MemoryCache (hết hạn sau 5 phút)
                var otpCode = HandleString.GenerateVerifyCode();
                _cache.Set(model.Email, otpCode, TimeSpan.FromMinutes(5));

                // Gửi OTP qua email
                await _emailSender.SendEmailAsync(model.Email, "Confirm email",
                    EmailSenderHTML.EmailBody(otpCode));
                var uri=new Uri($"{Request.Scheme}://{Request.Host}/api/accounts/register");
                return Created(uri, new { message = "User registered successfully. Please check your email for verification code.",email=account.Email});
            }
            return BadRequest(result.Errors);
             }
            catch (System.Exception ex)
            {
                
                throw new Exception("An error has occurred while registering");
            }
        }
        //pass
        [HttpPost("request-otp")]
        public async Task<IActionResult> ResendEmailConfirmation(ResendOTPDto model)
        {   
            try
            {
           
            var account = await _accountManager.FindByEmailAsync(model.Email);
            if (account == null )
            {
                return BadRequest("Please check your email for verification code.");
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
                EmailSenderHTML.EmailBody(otpCode));
                return Ok("Please check your email for verification code.");
            }


            return BadRequest("Please try again in a few minutes.");
                 
            }
            catch (System.Exception ex)
            {
                
                throw new Exception("An error has occurred while resending email confirmation");
            }
        }
        //pass
        [HttpPatch("email-confirm")]
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

            // if (account.EmailConfirmed)
            // {
            //     return BadRequest("Email has been previously verified.");
            // }

            // Cập nhật trạng thái xác thực email
            account.EmailConfirmed = true;
            var result=await _accountManager.UpdateAsync(account);
            if(result.Succeeded){
                var rs=await _accountManager.AddToRoleAsync(account,RolesName.Customer);
                if(rs.Succeeded){
                    //Auto create customer info
                    AutoCreateInfo(account.Id);
                }
            // Xóa OTP khỏi cache sau khi xác nhận thành công
            _cache.Remove(model.Email);
            return Ok("Email has been successfully verified!");
            }

            return BadRequest("An error has occurred while confirming email");
            // return BadRequest(result.Errors);  
            }
            catch (System.Exception ex)
            {
                
                throw new Exception("An error has occurred while confirming email");
            }
        }
        // Đăng nhập
        // pass
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginDto model)
        {
            try
            {
       
            var account = await _accountManager.FindByEmailAsync(model.Email);

            if (account == null)
                return BadRequest("Account does not exist.");

          
            var roles=await _accountManager.GetRolesAsync(account);
            if(!roles.Contains(RolesName.Customer))
                {
                
                if (_cache.TryGetValue(model.Email, out string storedOtp))
                    return BadRequest("Please try again in a few minutes.");

                var otpCode = HandleString.GenerateVerifyCode();
                _cache.Set(model.Email, otpCode, TimeSpan.FromMinutes(5));

                // Gửi OTP qua email
                await _emailSender.SendEmailAsync(model.Email, subject: "Confirm email",
                    EmailSenderHTML.EmailBody(otpCode));
              
                return BadRequest("To login as a customer. Please check your email for verification code.");
            
                }

            if (!await _accountManager.IsEmailConfirmedAsync(account))
                return Unauthorized("Please confirm email before logging in.");

            var result = await _signInManager.PasswordSignInAsync(account, model.Password, true, false);
            if (result.Succeeded)
            {   
                var token = (TokenResponse)_jwtService.GenerateJwtToken(account,roles);
                        _refreshtoken[account.Email]=token.RefreshToken;
                return  Ok(token);
            }
            return BadRequest("Invalid login information.");
                     
            }
            catch (System.Exception ex)
            {
                
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        
        //pass
        [HttpPatch("password-forgot")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            try
            {
            // if (!ModelState.IsValid)
            //     return BadRequest("Invalid your input");
            if (string.IsNullOrEmpty(model.Email))
                return BadRequest("Email is required");

            var account = await _accountManager.FindByEmailAsync(model.Email);

            if (account == null)
            {
                // Không tiết lộ thông tin tài khoản có tồn tại hay không
                return Ok("Password has been emailed to youuu");
            }
            var isInRoleCustomer= await _accountManager.IsInRoleAsync(account,RolesName.Customer);
            if (!isInRoleCustomer)
                return BadRequest("User does not have access");

            var password = HandleString.GenerateRandomString(16);
            // cập nhật account với mật khẩu mới
            account.PasswordHash = _accountManager.PasswordHasher.HashPassword(account, password);
            await _accountManager.UpdateAsync(account);

            await _emailSender.SendEmailAsync(account.Email, "Reset password",
                EmailSenderHTML.EmailBody("Your new password is: " + password));
            return Ok("Password has been emailed to you");
                   
            }
            catch (System.Exception ex)
            {
                
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // Đặt lại mật khẩu
        // pass
        [Authorize(Roles = RolesName.Customer)]
        [HttpPatch("password-change")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto model)
        {
            try
            {
            var currentAccount= (CurrentUser)CurrentUser();
            if(currentAccount == null){
                return Unauthorized();
            }
            if(model.ConfirmNewPassword != model.NewPassword)
                return BadRequest("Your new password and confirm password do not match.");

            var account = await _accountManager.FindByEmailAsync(currentAccount.Email);
            if (account == null)
                return Ok("Wrong account name or password"); // Không tiết lộ thông tin người dùng không tồn tại

            var result = await _accountManager.ChangePasswordAsync(account, model.Password, model.NewPassword);
    
            if (result.Succeeded){
                var content="You just changed your password at"+ DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                await _emailSender.SendEmailAsync(currentAccount.Email, "Password changed",
                EmailTemplateHtml.RenderEmailNotificationBody(currentAccount.Email,"Security Warning!", content));
                return Ok("Password changed successfully.");
            }
            return BadRequest("Wrong account name or password");
                    
            }
            catch (System.Exception ex)
            {
                
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
