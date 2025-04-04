using Microsoft.AspNetCore.Mvc;
using Moq;
using SneakerAPI.AdminApi.Controllers;
using SneakerAPI.Api.Controllers.UserControllers;
using SneakerAPI.Core.DTOs;
using SneakerAPI.Core.Interfaces;
using SneakerAPI.Core.Models.UserEntities;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
// using NUnit.Framework;

// namespace SneakerAPI.Tests.Controllers.UserControllers;

    public class UserInformationControllerTest
    {
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly UserInformationController _controller;

        public UserInformationControllerTest()
        {
            _mockUow = new Mock<IUnitOfWork>();
            _controller = new UserInformationController(_mockUow.Object);
        }

        [Fact]
        public void GetCustomerInformation_ShouldReturnOk_WhenCustomerExists()
        {
            // Arrange
            var currentAccount = new CurrentUser { AccountId = 1 };
            _controller.SetCurrentUser(currentAccount);

            var customerInfo = new CustomerInfo
            {
                CustomerInfo__AccountId = 1,
                CustomerInfo__FirstName = "John",
                CustomerInfo__LastName = "Doe"
            };

            _mockUow.Setup(u => u.CustomerInfo.Get(1)).Returns(customerInfo);

            // Act
            var result = _controller.GetCustomerInformation();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(customerInfo, okResult.Value);
        }

        [Fact]
        public void GetCustomerInformation_ShouldReturnNotFound_WhenCustomerDoesNotExist()
        {
            // Arrange
            var currentAccount = new CurrentUser { AccountId = 1 };
            _controller.SetCurrentUser(currentAccount);

            _mockUow.Setup(u => u.CustomerInfo.Get(1)).Returns((CustomerInfo)null);

            // Act
            var result = _controller.GetCustomerInformation();

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task UpdateCustomerInformation_ShouldReturnOk_WhenUpdateIsSuccessful()
        {
            // Arrange
            var currentAccount = new CurrentUser { AccountId = 1 };
            _controller.SetCurrentUser(currentAccount);

            var customerInfo = new CustomerInfo
            {
                CustomerInfo__AccountId = 1,
                CustomerInfo__FirstName = "John",
                CustomerInfo__LastName = "Doe",
                CustomerInfo__Avatar = "old-avatar.jpg"
            };

            var customerInfoDTO = new CustomerInfoDTO
            {
                CustomerInfo__FirstName = "Jane",
                CustomerInfo__LastName = "Smith",
                File = null // No file upload in this test
            };

            _mockUow.Setup(u => u.CustomerInfo.Get(1)).Returns(customerInfo);
            _mockUow.Setup(u => u.CustomerInfo.Update(It.IsAny<CustomerInfo>())).Returns(true);

            // Act
            var result = await _controller.UpdateCustomerInformation(customerInfoDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Profile updated successfully.", ((dynamic)okResult.Value).Message);
        }

        [Fact]
        public async Task UpdateCustomerInformation_ShouldReturnBadRequest_WhenUpdateFails()
        {
            // Arrange
            var currentAccount = new CurrentUser { AccountId = 1 };
            _controller.SetCurrentUser(currentAccount);

            var customerInfo = new CustomerInfo
            {
                CustomerInfo__AccountId = 1,
                CustomerInfo__FirstName = "John",
                CustomerInfo__LastName = "Doe"
            };

            var customerInfoDTO = new CustomerInfoDTO
            {
                CustomerInfo__FirstName = "Jane",
                CustomerInfo__LastName = "Smith",
                File = null
            };

            _mockUow.Setup(u => u.CustomerInfo.Get(1)).Returns(customerInfo);
            _mockUow.Setup(u => u.CustomerInfo.Update(It.IsAny<CustomerInfo>())).Returns(false);

            // Act
            var result = await _controller.UpdateCustomerInformation(customerInfoDTO);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Failed to update user profile.", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateCustomerInformation_ShouldReturnBadRequest_WhenFileUploadFails()
        {
            // Arrange
            var currentAccount = new CurrentUser { AccountId = 1 };
            _controller.SetCurrentUser(currentAccount);

            var customerInfo = new CustomerInfo
            {
                CustomerInfo__AccountId = 1,
                CustomerInfo__FirstName = "John",
                CustomerInfo__LastName = "Doe"
            };

            var customerInfoDTO = new CustomerInfoDTO
            {
                CustomerInfo__FirstName = "Jane",
                CustomerInfo__LastName = "Smith",
                File = Mock.Of<Microsoft.AspNetCore.Http.IFormFile>() // Mock file upload
            };

            _mockUow.Setup(u => u.CustomerInfo.Get(1)).Returns(customerInfo);
            _mockUow.Setup(u => u.CustomerInfo.Update(It.IsAny<CustomerInfo>())).Returns(true);

            // Simulate file upload failure
            HandleFile.Upload = (file, path) => Task.FromResult(false);

            // Act
            var result = await _controller.UpdateCustomerInformation(customerInfoDTO);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Failed to upload avatar.", badRequestResult.Value);
        }

        private static class HandleFile
        {
            public static Func<Microsoft.AspNetCore.Http.IFormFile, string, Task<bool>> Upload = (file, path) => Task.FromResult(true);
        }

        // Helper extension to set the current user in the controller

    }
