using Microsoft.AspNetCore.Mvc;
using Moq;
using SneakerAPI.Api.Controllers;
using SneakerAPI.Core.Interfaces;
using SneakerAPI.Core.Models.Filters;
using SneakerAPI.Core.Models.ProductEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
// using NUnit.Framework;

// namespace SneakerAPI.Tests.Controllers

    public class ProductControllerTest
    {
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly ProductController _controller;

        public ProductControllerTest()
        {
            _mockUow = new Mock<IUnitOfWork>();
            _controller = new ProductController(_mockUow.Object);
        }

        [Fact]
        public void GetProductsByStatus_ShouldReturnOk_WhenProductsExist()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Product__Id = 1, Product__Status = 1 },
                new Product { Product__Id = 2, Product__Status = 1 }
            };
            _mockUow.Setup(u => u.Product.GetAll(It.IsAny<Func<Product, bool>>()))
                .Returns(products.AsQueryable());

            // Act
            var result = _controller.GetProductsByStatus(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(products, okResult.Value);
        }

        [Fact]
        public void GetProductsByStatus_ShouldReturnNotFound_WhenNoProductsExist()
        {
            // Arrange
            _mockUow.Setup(u => u.Product.GetAll(It.IsAny<Func<Product, bool>>()))
                .Returns(Enumerable.Empty<Product>().AsQueryable());

            // Act
            var result = _controller.GetProductsByStatus(1);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void GetProduct_ShouldReturnOk_WhenProductExists()
        {
            // Arrange
            var product = new Product { Product__Id = 1 };
            _mockUow.Setup(u => u.Product.Get(1)).Returns(product);

            // Act
            var result = _controller.GetProduct(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(product, okResult.Value);
        }

        [Fact]
        public void GetProduct_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            _mockUow.Setup(u => u.Product.Get(1)).Returns((Product)null);

            // Act
            var result = _controller.GetProduct(1);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void GetProductsByFilter_ShouldReturnOk_WhenProductsExist()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Product__Id = 1 },
                new Product { Product__Id = 2 }
            };
            _mockUow.Setup(u => u.Product.GetFilteredProducts(It.IsAny<ProductFilter>()))
                .Returns(products.AsQueryable());

            // Act
            var result = _controller.GetProductsByFilter(new ProductFilter());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(products, okResult.Value);
        }

        [Fact]
        public void GetProductsByFilter_ShouldReturnNotFound_WhenNoProductsExist()
        {
            // Arrange
            _mockUow.Setup(u => u.Product.GetFilteredProducts(It.IsAny<ProductFilter>()))
                .Returns(Enumerable.Empty<Product>().AsQueryable());

            // Act
            var result = _controller.GetProductsByFilter(new ProductFilter());

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void GetProductsByCategory_ShouldReturnOk_WhenProductsExist()
        {
            // Arrange
            var category = new { CategoryId = 1 };
            var productIds = new List<int> { 1, 2 };
            var products = new List<Product>
            {
                new Product { Product__Id = 1 },
                new Product { Product__Id = 2 }
            };

            _mockUow.Setup(u => u.Category.Get(1)).Returns(category);
            _mockUow.Setup(u => u.ProductCategory.GetAll(It.IsAny<Func<dynamic, bool>>()))
                .Returns(productIds.Select(id => new { ProductCategory__ProductId = id }).AsQueryable());
            _mockUow.Setup(u => u.Product.GetAll(It.IsAny<Func<Product, bool>>()))
                .Returns(products.AsQueryable());

            // Act
            var result = _controller.GetProductsByCategory(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(products, okResult.Value);
        }

        [Fact]
        public void GetProductsByCategory_ShouldReturnNotFound_WhenCategoryDoesNotExist()
        {
            // Arrange
            _mockUow.Setup(u => u.Category.Get(1)).Returns((object)null);

            // Act
            var result = _controller.GetProductsByCategory(1);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void GetProductsByCategory_ShouldReturnNotFound_WhenNoProductsExist()
        {
            // Arrange
            var category = new { CategoryId = 1 };
            _mockUow.Setup(u => u.Category.Get(1)).Returns(category);
            _mockUow.Setup(u => u.ProductCategory.GetAll(It.IsAny<Func<dynamic, bool>>()))
                .Returns(Enumerable.Empty<dynamic>().AsQueryable());

            // Act
            var result = _controller.GetProductsByCategory(1);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
