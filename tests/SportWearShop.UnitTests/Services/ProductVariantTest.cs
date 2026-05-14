using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Moq;
using SportWearShop.BusinessLogics.Exceptions;
using SportWearShop.BusinessLogics.ResponseModels;
using SportWearShop.BusinessLogics.ResponseModels.ProductModels.ProductVarientModels;
using SportWearShop.BussinessLogics.Services;
using SportWearShop.Repositories.Entities;
using SportWearShop.Repositories.Enums;
using SportWearShop.Repositories.Implementations;
using SportWearShop.Repositories.UnitOfWorks;

namespace SportWearShop.UnitTests.Services;

public class ProductVariantServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<ProductVariantService>> _loggerMock;
    private readonly ProductVariantService _service;

    public ProductVariantServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<ProductVariantService>>();

        _service = new ProductVariantService(
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task CreateManyAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => _service.CreateManyAsync(1, null!));
    }

    [Fact]
    public async Task CreateManyAsync_ShouldThrowBadRequestException_WhenVariantsEmpty()
    {
        var request = new CreateProductVariantsRequestModel
        {
            Variants = []
        };

        await Assert.ThrowsAsync<BadRequestException>(
            () => _service.CreateManyAsync(1, request));
    }

    [Fact]
    public async Task CreateManyAsync_ShouldThrowNotFoundException_WhenProductNotFound()
    {
        var request = CreateValidCreateManyRequest();

        _unitOfWorkMock
            .Setup(x => x.Products.AnyAsync(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => _service.CreateManyAsync(99, request));

        Assert.Equal("Product with ID 99 was not found.", exception.Message);
    }

    [Fact]
    public async Task CreateManyAsync_ShouldThrowConflictException_WhenDuplicateColorSizeInRequest()
    {
        var request = new CreateProductVariantsRequestModel
        {
            Variants =
            [
                new CreateProductVariantRequestModel
                {
                    ColorCode = "red",
                    ColorName = "Red",
                    SizeCode = "m",
                    SizeLabel = "M",
                    ListPrice = 100,
                    SalePrice = 90,
                    WeightGrams = 500
                },
                new CreateProductVariantRequestModel
                {
                    ColorCode = "RED",
                    ColorName = "Red",
                    SizeCode = "M",
                    SizeLabel = "M",
                    ListPrice = 100,
                    SalePrice = 90,
                    WeightGrams = 500
                }
            ]
        };

        _unitOfWorkMock
            .Setup(x => x.Products.AnyAsync(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<ConflictException>(
            () => _service.CreateManyAsync(1, request));
    }

    [Fact]
    public async Task CreateManyAsync_ShouldThrowBadRequestException_WhenSalePriceGreaterThanListPrice()
    {
        var request = new CreateProductVariantsRequestModel
        {
            Variants =
            [
                new CreateProductVariantRequestModel
                {
                    ColorCode = "BLK",
                    ColorName = "Black",
                    SizeCode = "M",
                    SizeLabel = "M",
                    ListPrice = 100,
                    SalePrice = 120,
                    WeightGrams = 500
                }
            ]
        };

        _unitOfWorkMock
            .Setup(x => x.Products.AnyAsync(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<BadRequestException>(
            () => _service.CreateManyAsync(1, request));
    }

    [Fact]
    public async Task CreateManyAsync_ShouldThrowConflictException_WhenSkuExists()
    {
        var request = CreateValidCreateManyRequest();

        _unitOfWorkMock
            .Setup(x => x.Products.AnyAsync(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _unitOfWorkMock
            .Setup(x => x.ProductVariants.AnyAsync(
                It.IsAny<Expression<Func<ProductVariant, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<ConflictException>(
            () => _service.CreateManyAsync(1, request));
    }

    [Fact]
    public async Task CreateManyAsync_ShouldCreateVariants_WhenRequestIsValid()
    {
        var request = CreateValidCreateManyRequest();

        _unitOfWorkMock
            .Setup(x => x.Products.AnyAsync(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _unitOfWorkMock
            .SetupSequence(x => x.ProductVariants.AnyAsync(
                It.IsAny<Expression<Func<ProductVariant, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false) // sku exists
            .ReturnsAsync(false); // color-size exists

        _unitOfWorkMock
            .Setup(x => x.ProductVariants.AddRangeAsync(
                It.IsAny<List<ProductVariant>>(),
                It.IsAny<CancellationToken>()))
            .Callback<IEnumerable<ProductVariant>, CancellationToken>((variants, _) =>
            {
                var id = 1L;
                foreach (var variant in variants)
                {
                    variant.ProductVariantId = id++;
                }
            })
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _service.CreateManyAsync(1, request);

        Assert.Single(result);
        Assert.Equal("SPW-1-BLK-M", result[0].Sku);
        Assert.Equal("Draft", result[0].Status);

        _unitOfWorkMock.Verify(x => x.ProductVariants.AddRangeAsync(
            It.Is<List<ProductVariant>>(variants =>
                variants.Count == 1 &&
                variants[0].Sku == "SPW-1-BLK-M" &&
                variants[0].Status == ProductVariantStatus.Draft),
            It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowNotFoundException_WhenVariantNotFound()
    {
        _unitOfWorkMock
            .Setup(x => x.ProductVariants.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<ProductVariant, bool>>>(),
                It.IsAny<Expression<Func<ProductVariant, ProductVariant>>>(),
                false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductVariant?)null);

        var request = CreateValidUpdateRequest();

        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.UpdateAsync(99, request));
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowConflictException_WhenSkuExists()
    {
        var variant = CreateVariant();

        _unitOfWorkMock
            .Setup(x => x.ProductVariants.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<ProductVariant, bool>>>(),
                It.IsAny<Expression<Func<ProductVariant, ProductVariant>>>(),
                false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(variant);

        _unitOfWorkMock
            .Setup(x => x.ProductVariants.AnyAsync(
                It.IsAny<Expression<Func<ProductVariant, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var request = CreateValidUpdateRequest();

        await Assert.ThrowsAsync<ConflictException>(
            () => _service.UpdateAsync(1, request));
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowBadRequestException_WhenSalePriceGreaterThanListPrice()
    {
        var variant = CreateVariant();

        _unitOfWorkMock
            .Setup(x => x.ProductVariants.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<ProductVariant, bool>>>(),
                It.IsAny<Expression<Func<ProductVariant, ProductVariant>>>(),
                false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(variant);

        _unitOfWorkMock
            .SetupSequence(x => x.ProductVariants.AnyAsync(
                It.IsAny<Expression<Func<ProductVariant, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false)
            .ReturnsAsync(false);

        var request = CreateValidUpdateRequest();
        request.ListPrice = 100;
        request.SalePrice = 150;

        await Assert.ThrowsAsync<BadRequestException>(
            () => _service.UpdateAsync(1, request));
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateVariant_WhenRequestIsValid()
    {
        var variant = CreateVariant();

        _unitOfWorkMock
            .Setup(x => x.ProductVariants.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<ProductVariant, bool>>>(),
                It.IsAny<Expression<Func<ProductVariant, ProductVariant>>>(),
                false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(variant);

        _unitOfWorkMock
            .SetupSequence(x => x.ProductVariants.AnyAsync(
                It.IsAny<Expression<Func<ProductVariant, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false)
            .ReturnsAsync(false);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var request = CreateValidUpdateRequest();

        var result = await _service.UpdateAsync(1, request);

        Assert.Equal("SPW-1-WHT-L", result.Sku);
        Assert.Equal("WHT", result.ColorCode);
        Assert.Equal("L", result.SizeCode);
        Assert.Equal(200, result.ListPrice);
        Assert.Equal(150, result.SalePrice);

        _unitOfWorkMock.Verify(x => x.ProductVariants.Update(variant), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowNotFoundException_WhenVariantNotFound()
    {
        _unitOfWorkMock
            .Setup(x => x.ProductVariants.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<ProductVariant, bool>>>(),
                It.IsAny<Expression<Func<ProductVariant, ProductVariant>>>(),
                false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductVariant?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.DeleteAsync(99));
    }

    [Fact]
    public async Task DeleteAsync_ShouldSoftDeleteVariant_WhenVariantExists()
    {
        var variant = CreateVariant();

        _unitOfWorkMock
            .Setup(x => x.ProductVariants.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<ProductVariant, bool>>>(),
                It.IsAny<Expression<Func<ProductVariant, ProductVariant>>>(),
                false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(variant);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _service.DeleteAsync(1);

        Assert.Equal(ProductVariantStatus.Deleted, variant.Status);

        _unitOfWorkMock.Verify(x => x.ProductVariants.Update(variant), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldThrowNotFoundException_WhenVariantNotFound()
    {
        _unitOfWorkMock
            .Setup(x => x.ProductVariants.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<ProductVariant, bool>>>(),
                It.IsAny<Expression<Func<ProductVariant, ProductVariant>>>(),
                false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductVariant?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.UpdateStatusAsync(99, ProductVariantStatus.Active));
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldThrowBadRequestException_WhenVariantDeleted()
    {
        var variant = CreateVariant();
        variant.Status = ProductVariantStatus.Deleted;

        _unitOfWorkMock
            .Setup(x => x.ProductVariants.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<ProductVariant, bool>>>(),
                It.IsAny<Expression<Func<ProductVariant, ProductVariant>>>(),
                false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(variant);

        await Assert.ThrowsAsync<BadRequestException>(
            () => _service.UpdateStatusAsync(1, ProductVariantStatus.Active));
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldUpdateStatus_WhenValid()
    {
        var variant = CreateVariant();

        _unitOfWorkMock
            .Setup(x => x.ProductVariants.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<ProductVariant, bool>>>(),
                It.IsAny<Expression<Func<ProductVariant, ProductVariant>>>(),
                false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(variant);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _service.UpdateStatusAsync(1, ProductVariantStatus.Active);

        Assert.Equal("Active", result.Status);
        Assert.Equal(ProductVariantStatus.Active, variant.Status);

        _unitOfWorkMock.Verify(x => x.ProductVariants.Update(variant), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private static CreateProductVariantsRequestModel CreateValidCreateManyRequest()
    {
        return new CreateProductVariantsRequestModel
        {
            Variants =
            [
                new CreateProductVariantRequestModel
                {
                    ColorCode = "blk",
                    ColorName = "Black",
                    SizeCode = "m",
                    SizeLabel = "M",
                    ListPrice = 200,
                    SalePrice = 150,
                    WeightGrams = 500
                }
            ]
        };
    }

    private static UpdateProductVariantRequestModel CreateValidUpdateRequest()
    {
        return new UpdateProductVariantRequestModel
        {
            Sku = "spw-1-wht-l",
            ColorCode = "wht",
            ColorName = "White",
            SizeCode = "l",
            SizeLabel = "L",
            ListPrice = 200,
            SalePrice = 150,
            WeightGrams = 600
        };
    }

    private static ProductVariant CreateVariant()
    {
        return new ProductVariant
        {
            ProductVariantId = 1,
            ProductId = 1,
            Sku = "SPW-1-BLK-M",
            ColorCode = "BLK",
            ColorName = "Black",
            SizeCode = "M",
            SizeLabel = "M",
            ListPrice = 100,
            SalePrice = 90,
            WeightGrams = 500,
            Status = ProductVariantStatus.Draft,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };
    }
}