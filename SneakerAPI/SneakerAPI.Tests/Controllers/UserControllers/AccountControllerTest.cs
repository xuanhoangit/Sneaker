using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using SneakerAPI.Api.Controllers.UserControllers;
using SneakerAPI.Core.DTOs;
using SneakerAPI.Core.Interfaces;
using Xunit;
// using NUnit.Framework;
using SneakerAPI.Core.Models;
using SneakerAPI.Core.Interfaces.UserInterfaces;
using Microsoft.AspNetCore.Identity.UI.Services;

// namespace SneakerAPI.Tests.Controllers.UserControllers

public class AccountControllerTest
    {
        private readonly Mock<UserManager<IdentityAccount>> _mockUserManager;
        private readonly Mock<SignInManager<IdentityAccount>> _mockSignInManager;
        private readonly Mock<IEmailSender> _mockEmailSender;
        private readonly Mock<IMemoryCache> _mockCache;
        private readonly Mock<IJwtService> _mockJwtService;
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly AccountController _controller;

        public AccountControllerTest()
        {
            _mockUserManager = MockUserManager();
            _mockSignInManager = MockSignInManager();
            _mockEmailSender = new Mock<IEmailSender>();
            _mockCache = new Mock<IMemoryCache>();
            _mockJwtService = new Mock<IJwtService>();
            _mockUow = new Mock<IUnitOfWork>();

            _controller = new AccountController(
                _mockUserManager.Object,
                _mockSignInManager.Object,
                _mockEmailSender.Object,
                _mockCache.Object,
                _mockJwtService.Object,
                _mockUow.Object
            );
        }

        [Fact]
        public async Task Register_ShouldReturnCreated_WhenRegistrationIsSuccessful()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Email = "test@example.com",
                Password = "Password123!",
                PasswordComfirm = "Password123!"
            };

            _mockUserManager.Setup(u => u.FindByEmailAsync(registerDto.Email))
                .ReturnsAsync((IdentityAccount)null);
            _mockUserManager.Setup(u => u.CreateAsync(It.IsAny<IdentityAccount>(), registerDto.Password))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(u => u.AddToRoleAsync(It.IsAny<IdentityAccount>(), RolesName.Customer))
                .ReturnsAsync(IdentityResult.Success);
            _mockCache.Setup(c => c.CreateEntry(registerDto.Email)).Returns(Mock.Of<ICacheEntry>());

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result);
            Assert.Equal("User registered successfully", ((dynamic)createdResult.Value).message);
        }

        [Fact]
        public async Task Register_ShouldReturnBadRequest_WhenPasswordsDoNotMatch()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Email = "test@example.com",
                Password = "Password123!",
                PasswordComfirm = "Password456!"
            };

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Password and password confirm do not match", badRequestResult.Value);
        }

        [Fact]
        public async Task Login_ShouldReturnOk_WhenLoginIsSuccessful()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "Password123!"
            };

            var account = new IdentityAccount { Email = loginDto.Email, UserName = loginDto.Email };
            var roles = new List<string> { RolesName.Customer };

            _mockUserManager.Setup(u => u.FindByEmailAsync(loginDto.Email)).ReturnsAsync(account);
            _mockUserManager.Setup(u => u.GetRolesAsync(account)).ReturnsAsync(roles);
            _mockUserManager.Setup(u => u.IsEmailConfirmedAsync(account)).ReturnsAsync(true);
            _mockSignInManager.Setup(s => s.PasswordSignInAsync(account, loginDto.Password, false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
            _mockJwtService.Setup(j => j.GenerateJwtToken(account.UserName, roles))
                .Returns(new TokenResponse { AccessToken = "jwt-token" });

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("jwt-token", ((TokenResponse)okResult.Value).AccessToken);
        }

        [Fact]
        public async Task Login_ShouldReturnBadRequest_WhenAccountDoesNotExist()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "nonexistent@example.com",
                Password = "Password123!"
            };

            _mockUserManager.Setup(u => u.FindByEmailAsync(loginDto.Email)).ReturnsAsync((IdentityAccount)null);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Account does not exist.", badRequestResult.Value);
        }

        [Fact]
        public async Task ForgotPassword_ShouldReturnOk_WhenEmailIsSent()
        {
            // Arrange
            var forgotPasswordDto = new ForgotPasswordDto { Email = "test@example.com" };
            var account = new IdentityAccount { Email = forgotPasswordDto.Email };

            _mockUserManager.Setup(u => u.FindByEmailAsync(forgotPasswordDto.Email)).ReturnsAsync(account);
            _mockEmailSender.Setup(e => e.SendEmailAsync(forgotPasswordDto.Email, It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.ForgotPassword(forgotPasswordDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Password has been emailed to you", okResult.Value);
        }

        [Fact]
        public async Task ForgotPassword_ShouldReturnOk_WhenAccountDoesNotExist()
        {
            // Arrange
            var forgotPasswordDto = new ForgotPasswordDto { Email = "nonexistent@example.com" };

            _mockUserManager.Setup(u => u.FindByEmailAsync(forgotPasswordDto.Email)).ReturnsAsync((IdentityAccount)null);

            // Act
            var result = await _controller.ForgotPassword(forgotPasswordDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Password has been emailed to youuu", okResult.Value);
        }

        private Mock<UserManager<IdentityAccount>> MockUserManager()
        {
            var store = new Mock<IUserStore<IdentityAccount>>();
            return new Mock<UserManager<IdentityAccount>>(store.Object, null, null, null, null, null, null, null, null);
        }

        private Mock<SignInManager<IdentityAccount>> MockSignInManager()
        {
            var userManager = MockUserManager();
            var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<IdentityAccount>>();
            return new Mock<SignInManager<IdentityAccount>>(userManager.Object, contextAccessor.Object, claimsFactory.Object, null, null, null, null);
        }
    }
