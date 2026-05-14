using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Moq;
using SportWearShop.BusinessLogics.Exceptions;
using SportWearShop.BusinessLogics.ResponseModels.ProductModels;
using SportWearShop.BusinessLogics.Services;
using SportWearShop.Repositories.Entities;
using SportWearShop.Repositories.Enums;
using SportWearShop.Repositories.UnitOfWorks;
using Xunit;

namespace SportWearShop.UnitTests.Services;

public class ProductServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<ProductService>> _loggerMock;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<ProductService>>();

        _service = new ProductService(
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateProduct_WhenRequestIsValid()
    {
        // Arrange
        var request = new CreateProductRequestModel
        {
            ProductName = "Nike Air Max",
            BrandId = 1,
            CategoryId = 2,
            Slug = "Nike Air Max",
            Gender = ProductGender.Men,
            Description = "Running shoes",
            BaseMaterial = "Mesh"
        };

        _unitOfWorkMock
            .Setup(x => x.Brands.AnyAsync(
                It.IsAny<Expression<Func<Brand, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _unitOfWorkMock
            .Setup(x => x.Categories.AnyAsync(
                It.IsAny<Expression<Func<Category, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _unitOfWorkMock
            .Setup(x => x.Products.AnyAsync(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _unitOfWorkMock
            .Setup(x => x.Products.AddAsync(
                It.IsAny<Product>(),
                It.IsAny<CancellationToken>()))
            .Callback<Product, CancellationToken>((product, _) =>
            {
                product.ProductId = 1;
            })
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.ProductId);
        Assert.Equal("Nike Air Max", result.ProductName);
        Assert.Equal("PRD-000001", result.ProductCode);

        _unitOfWorkMock.Verify(
            x => x.Products.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Exactly(2));
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowNotFoundException_WhenBrandNotFound()
    {
        // Arrange
        var request = new CreateProductRequestModel
        {
            ProductName = "Nike Air Max",
            BrandId = 999,
            CategoryId = 2
        };

        _unitOfWorkMock
            .Setup(x => x.Brands.AnyAsync(
                It.IsAny<Expression<Func<Brand, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => _service.CreateAsync(request));

        // Assert
        Assert.Equal("Brand with ID 999 was not found.", exception.Message);

        _unitOfWorkMock.Verify(
            x => x.Products.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateProduct_WhenValid()
    {
        // Arrange
        var product = new Product
        {
            ProductId = 1,
            ProductName = "Old Name",
            BrandId = 1,
            CategoryId = 1,
            Slug = "old-name",
            Status = ProductStatus.Draft
        };

        var request = new UpdateProductRequestModel
        {
            ProductName = "New Name",
            BrandId = 2,
            CategoryId = 3,
            Slug = "New Name",
            Status = ProductStatus.Active
        };

        _unitOfWorkMock
            .Setup(x => x.Products.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<Expression<Func<Product, Product>>>(),
                false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _unitOfWorkMock
            .Setup(x => x.Brands.AnyAsync(
                It.IsAny<Expression<Func<Brand, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _unitOfWorkMock
            .Setup(x => x.Categories.AnyAsync(
                It.IsAny<Expression<Func<Category, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _unitOfWorkMock
            .Setup(x => x.Products.AnyAsync(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _service.UpdateAsync(1, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.ProductId);
        Assert.Equal("New Name", result.ProductName);
        Assert.Equal(ProductStatus.Active.ToString(), result.Status);

        Assert.Equal(2, product.BrandId);
        Assert.Equal(3, product.CategoryId);

        _unitOfWorkMock.Verify(
            x => x.Products.Update(product),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowNotFoundException_WhenProductNotFound()
    {
        // Arrange
        _unitOfWorkMock
            .Setup(x => x.Products.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<Expression<Func<Product, Product>>>(),
                false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var request = new UpdateProductRequestModel
        {
            ProductName = "New Name"
        };

        // Act
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => _service.UpdateAsync(999, request));

        // Assert
        Assert.Equal("Product with ID 999 was not found.", exception.Message);
    }
}