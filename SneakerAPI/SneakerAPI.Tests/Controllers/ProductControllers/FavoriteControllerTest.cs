using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SneakerAPI.Api.Controllers.ProductControllers;
using SneakerAPI.Core.DTOs;
using SneakerAPI.Core.Interfaces;
using SneakerAPI.Core.Models.ProductEntities;
using System.Linq.Expressions;
// using NUnit.Framework;
using Xunit;

// namespace SneakerAPI.Tests.Controllers

    public class FavoriteControllerTest
    {
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<IMapper> _mockMapper;
        private readonly FavoriteController _controller;

        public FavoriteControllerTest()
        {
            _mockUow = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _controller = new FavoriteController(_mockUow.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetFavoriteProducts_ShouldReturnOk_WhenFavoritesExist()
        {
            // Arrange
            var currentAccount = new CurrentUser { AccountId = 1 };
            _controller.SetCurrentUser(currentAccount);

            var favorites = new List<Favorite>
            {
                new Favorite { Favorite__Id = 1, Favorite__AccountId = 1, Favorite__IsDeleted = false },
                new Favorite { Favorite__Id = 2, Favorite__AccountId = 1, Favorite__IsDeleted = false }
            };

            _mockUow.Setup(u => u.Favorite.GetAllAsync(It.IsAny<Expression<Func<Favorite, bool>>>(),It.IsAny<string>()))
                .ReturnsAsync(favorites);

            // Act
            var result = await _controller.GetFavoriteProducts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(favorites, okResult.Value);
        }

        [Fact]
        public async Task GetFavoriteProducts_ShouldReturnUnauthorized_WhenUserNotAuthenticated()
        {
            // Act
            var result = await _controller.GetFavoriteProducts();

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task AddToFavorite_ShouldReturnOk_WhenItemAddedSuccessfully()
        {
            // Arrange
            var currentAccount = new CurrentUser { AccountId = 1 };
            _controller.SetCurrentUser(currentAccount);

            var favoriteDTO = new AddFavoriteDTO { Favorite__ProductColorId = 1 };
            var favorite = new Favorite { Favorite__Id = 1, Favorite__AccountId = 1, Favorite__ProductColorId = 1 };

            _mockMapper.Setup(m => m.Map<Favorite>(favoriteDTO)).Returns(favorite);
            _mockUow.Setup(u => u.Favorite.FirstOrDefaultAsync(It.IsAny<Expression<Func<Favorite, bool>>>()))
                .ReturnsAsync((Favorite)null);
            _mockUow.Setup(u => u.Favorite.AddAsync(It.IsAny<Favorite>())).ReturnsAsync(true);

            // Act
            var result = await _controller.AddToFavorite(favoriteDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Item added to favorites successfully.", okResult.Value);
        }

        [Fact]
        public async Task AddToFavorite_ShouldReturnBadRequest_WhenInputIsInvalid()
        {
            // Act
            var result = await _controller.AddToFavorite(null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void Delete_ShouldReturnOk_WhenItemDeletedSuccessfully()
        {
            // Arrange
            var currentAccount = new CurrentUser { AccountId = 1 };
            _controller.SetCurrentUser(currentAccount);

            var favorite = new Favorite { Favorite__Id = 1, Favorite__AccountId = 1, Favorite__IsDeleted = false };

            _mockUow.Setup(u => u.Favorite.FirstOrDefault(It.IsAny<Expression<Func<Favorite, bool>>>()))
                .Returns(favorite);
            _mockUow.Setup(u => u.Favorite.Update(It.IsAny<Favorite>())).Returns(true);

            // Act
            var result = _controller.Delete(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(new { message = "Deleted successfully" }, okResult.Value);
        }

        [Fact]
        public void Delete_ShouldReturnNotFound_WhenItemDoesNotExist()
        {
            // Arrange
            var currentAccount = new CurrentUser { AccountId = 1 };
            _controller.SetCurrentUser(currentAccount);

            _mockUow.Setup(u => u.Favorite.FirstOrDefault(It.IsAny<Expression<Func<Favorite, bool>>>()))
                .Returns((Favorite)null);

            // Act
            var result = _controller.Delete(1);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void Delete_ShouldReturnUnauthorized_WhenUserNotAuthenticated()
        {
            // Act
            var result = _controller.Delete(1);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result);
        }
    }

    // Helper extension to set the current user in the controller

