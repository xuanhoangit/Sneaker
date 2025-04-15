using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SneakerAPI.Api.Controllers.OrderControllers;
using SneakerAPI.Core.DTOs;
using SneakerAPI.Core.Interfaces;
using SneakerAPI.Core.Models.OrderEntities;
using SneakerAPI.Api.Controllers;
using System;
// using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Http;

// namespace SneakerAPI.Tests.Controllers

public class CartItemControllerTest
    {
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CartItemController _controller;

        public CartItemControllerTest()
        {
            _mockUow = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _controller = new CartItemController(_mockUow.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetUserCart_ShouldReturnOk_WhenCartItemsExist()
        {
            // Arrange
            var currentAccount = new CurrentUser { AccountId = 1 };
            _controller.SetCurrentUser(currentAccount);

            var cartItems = new List<GetCartItemDTO>
            {
                new GetCartItemDTO { CartItem__Id = 1, CartItem__CreatedByAccountId = 1 },
                new GetCartItemDTO { CartItem__Id = 2, CartItem__CreatedByAccountId = 1 }
            };

            _mockUow.Setup(u => u.CartItem.GetCartItem(1,null)).ReturnsAsync(cartItems);

            // Act
            var result = await _controller.GetUserCart();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(cartItems, okResult.Value);
        }

        [Fact]
        public async Task GetUserCart_ShouldReturnUnauthorized_WhenUserNotAuthenticated()
        {
            // Act
            var result = await _controller.GetUserCart();

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public void AddItem_ShouldReturnOk_WhenItemAddedSuccessfully()
        {
            // Arrange
            var currentAccount = new CurrentUser { AccountId = 1 };
            _controller.SetCurrentUser(currentAccount);

            var cartDTO = new AddCartDTO { CartItem__ProductColorSizeId = 1, CartItem__Quantity = 2 };
            var newItem = new CartItem { CartItem__Id = 1, CartItem__CreatedByAccountId = 1 };

            _mockMapper.Setup(m => m.Map<CartItem>(cartDTO)).Returns(newItem);
            _mockUow.Setup(u => u.CartItem.FirstOrDefault(It.IsAny<Expression<Func<CartItem, bool>>>()))
                .Returns((CartItem)null);
            _mockUow.Setup(u => u.CartItem.Add(newItem)).Returns(true);

            // Act
            var result = _controller.AddItem(cartDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Item added successfully.", okResult.Value);
        }

        [Fact]
        public void AddItem_ShouldReturnBadRequest_WhenInputIsInvalid()
        {
            // Act
            var result = _controller.AddItem(null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void UpdateCartItem_ShouldReturnOk_WhenItemUpdatedSuccessfully()
        {
            // Arrange
            var item = new CartItem { CartItem__Id = 1, CartItem__Quantity = 2 };
            _mockUow.Setup(u => u.CartItem.Get(1)).Returns(item);
            _mockUow.Setup(u => u.CartItem.Update(item)).Returns(true);

            // Act
            var result = _controller.UpdateCartItem(1, 5);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Item updated successfully.", okResult.Value);
        }

        [Fact]
        public void UpdateCartItem_ShouldReturnNotFound_WhenItemDoesNotExist()
        {
            // Arrange
            _mockUow.Setup(u => u.CartItem.Get(1)).Returns((CartItem)null);

            // Act
            var result = _controller.UpdateCartItem(1, 5);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void DeleteItem_ShouldReturnOk_WhenItemDeletedSuccessfully()
        {
            // Arrange
            var currentAccount = new CurrentUser { AccountId = 1 };
            _controller.SetCurrentUser(currentAccount);

            var item = new CartItem { CartItem__Id = 1, CartItem__CreatedByAccountId = 1 };
            _mockUow.Setup(u => u.CartItem.Get(1)).Returns(item);
            _mockUow.Setup(u => u.CartItem.Remove(item)).Returns(true);

            // Act
            var result = _controller.DeleteItem(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Item removed successfully.", okResult.Value);
        }

        [Fact]
        public void DeleteItem_ShouldReturnNotFound_WhenItemDoesNotExist()
        {
            // Arrange
            _mockUow.Setup(u => u.CartItem.Get(1)).Returns((CartItem)null);

            // Act
            var result = _controller.DeleteItem(1);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void DeleteItems_ShouldReturnOk_WhenItemsDeletedSuccessfully()
        {
            // Arrange
            var currentAccount = new CurrentUser { AccountId = 1 };
            _controller.SetCurrentUser(currentAccount);

            var items = new List<CartItem>
            {
                new CartItem { CartItem__Id = 1, CartItem__CreatedByAccountId = 1 },
                new CartItem { CartItem__Id = 2, CartItem__CreatedByAccountId = 1 }
            };

            _mockUow.Setup(u => u.CartItem.Find(It.IsAny<Expression<Func<CartItem, bool>>>()))
                .Returns(items.AsQueryable());
            _mockUow.Setup(u => u.CartItem.RemoveRange(items)).Returns(true);

            // Act
            var result = _controller.DeleteItems(new List<int> { 1, 2 });

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Items removed successfully.", okResult.Value);
        }
    }


