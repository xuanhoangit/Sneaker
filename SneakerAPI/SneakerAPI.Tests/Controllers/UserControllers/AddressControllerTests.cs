using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SneakerAPI.AdminApi.Controllers.ProductControllers;
using SneakerAPI.Core.DTOs;
using SneakerAPI.Core.Interfaces;
using SneakerAPI.Core.Models.UserEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;
// using NUnit.Framework;

// namespace SneakerAPI.Tests.Controllers.UserControllers;

    public class AddressControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<IMapper> _mockMapper;
        private readonly AddressController _controller;

        public AddressControllerTests()
        {
            _mockUow = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _controller = new AddressController(_mockUow.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetAddresses_ReturnsOk_WhenAddressesExist()
        {
            // Arrange
            int userInformationId = 1;
            var addresses = new List<Address>
            {
                new Address { Address__Id = 1, Address__FullAddress = "123 Street" },
                new Address { Address__Id = 2, Address__FullAddress = "456 Avenue" }
            };
            _mockUow.Setup(u => u.Address.GetAllAsync(It.IsAny<Expression<Func<Address, bool>>>()))
                .ReturnsAsync(addresses);

            // Act
            var result = await _controller.GetAddresses(userInformationId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(addresses, okResult.Value);
        }

        [Fact]
        public async Task GetAddresses_ReturnsNotFound_WhenNoAddressesExist()
        {
            // Arrange
            int userInformationId = 1;
            _mockUow.Setup(u => u.Address.GetAllAsync(It.IsAny<Expression<Func<Address, bool>>>()))
                .ReturnsAsync(new List<Address>());

            // Act
            var result = await _controller.GetAddresses(userInformationId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void GetAddress_ReturnsOk_WhenAddressExists()
        {
            // Arrange
            int addressId = 1;
            var address = new Address { Address__Id = addressId, Address__FullAddress = "123 Street" };
            _mockUow.Setup(u => u.Address.Get(addressId)).Returns(address);

            // Act
            var result = _controller.GetAddress(addressId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(address, okResult.Value);
        }

        [Fact]
        public void GetAddress_ReturnsNotFound_WhenAddressDoesNotExist()
        {
            // Arrange
            int addressId = 1;
            _mockUow.Setup(u => u.Address.Get(addressId)).Returns((Address)null);

            // Act
            var result = _controller.GetAddress(addressId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void CreateAddress_ReturnsCreatedAtAction_WhenAddressIsCreated()
        {
            // Arrange
            var addressDto = new AddressDTO { Address__CustomerInfo = 1, Address__FullAddress = "123 Street" };
            var address = new Address { Address__Id = 1, Address__FullAddress = "123 Street" };
            _mockMapper.Setup(m => m.Map<Address>(addressDto)).Returns(address);

            // Act
            var result = _controller.CreateAddress(addressDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(address, createdResult.Value);
        }

        [Fact]
        public void UpdateAddress_ReturnsOk_WhenAddressIsUpdated()
        {
            // Arrange
            int addressId = 1;
            var addressDto = new AddressDTO { Address__FullAddress = "Updated Address" };
            var address = new Address { Address__Id = addressId, Address__FullAddress = "Old Address" };
            _mockUow.Setup(u => u.Address.Get(addressId)).Returns(address);

            // Act
            var result = _controller.UpdateAddress(addressId, addressDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Address updated successfully", ((dynamic)okResult.Value).message);
        }

        [Fact]
        public void UpdateAddress_ReturnsNotFound_WhenAddressDoesNotExist()
        {
            // Arrange
            int addressId = 1;
            var addressDto = new AddressDTO { Address__FullAddress = "Updated Address" };
            _mockUow.Setup(u => u.Address.Get(addressId)).Returns((Address)null);

            // Act
            var result = _controller.UpdateAddress(addressId, addressDto);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void DeleteAddress_ReturnsOk_WhenAddressIsDeleted()
        {
            // Arrange
            int addressId = 1;
            var address = new Address { Address__Id = addressId };
            _mockUow.Setup(u => u.Address.Get(addressId)).Returns(address);

            // Act
            var result = _controller.DeleteAddress(addressId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Address deleted successfully", ((dynamic)okResult.Value).message);
        }

        [Fact]
        public void DeleteAddress_ReturnsNotFound_WhenAddressDoesNotExist()
        {
            // Arrange
            int addressId = 1;
            _mockUow.Setup(u => u.Address.Get(addressId)).Returns((Address)null);

            // Act
            var result = _controller.DeleteAddress(addressId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
