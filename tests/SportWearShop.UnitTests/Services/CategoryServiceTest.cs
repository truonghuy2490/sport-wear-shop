using Microsoft.Extensions.Logging;
using Moq;
using SportWearShop.BusinessLogics.Exceptions;
using SportWearShop.BusinessLogics.ResponseModels;
using SportWearShop.BusinessLogics.ResponseModels.CategoryModels;
using SportWearShop.BusinessLogics.Services;
using SportWearShop.Repositories.Entities;
using SportWearShop.Repositories.Implementations;
using SportWearShop.Repositories.UnitOfWorks;
using System.Linq.Expressions;

namespace SportWearShop.UnitTests.Services;

public class CategoryServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<CategoryService>> _loggerMock;
    private readonly CategoryService _categoryService;

    public CategoryServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<CategoryService>>();

        _categoryService = new CategoryService(
            _loggerMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCategoryNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _unitOfWorkMock
            .Setup(x => x.Categories.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<Category, bool>>>(),
                It.IsAny<Expression<Func<Category, CategoryDetailResponseModel>>>(),
                true,
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<Category, object>>[]>()))
            .ReturnsAsync((CategoryDetailResponseModel?)null);

        // Act
        var act = async () => await _categoryService.GetByIdAsync(1);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(act);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCategoryExists_ShouldReturnCategoryDetail()
    {
        // Arrange
        var expected = new CategoryDetailResponseModel
        {
            CategoryId = 1,
            CategoryName = "Shoes",
            CategoryCode = "SHOES",
            IsActive = true,
            Children = new List<CategoryResponseModel>()
        };

        _unitOfWorkMock
            .Setup(x => x.Categories.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<Category, bool>>>(),
                It.IsAny<Expression<Func<Category, CategoryDetailResponseModel>>>(),
                true,
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<Category, object>>[]>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _categoryService.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.CategoryId);
        Assert.Equal("Shoes", result.CategoryName);
        Assert.Equal("SHOES", result.CategoryCode);
    }

    [Fact]
    public async Task CreateAsync_WhenRequestNull_ShouldThrowArgumentNullException()
    {
        // Act
        var act = async () => await _categoryService.CreateAsync(null!);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(act);
    }

    [Fact]
    public async Task CreateAsync_WhenCategoryCodeExists_ShouldThrowConflictException()
    {
        // Arrange
        var request = new CreateCategoryRequestModel
        {
            CategoryName = "Shoes",
            CategoryCode = "SHOES",
            SortOrder = 1
        };

        _unitOfWorkMock
            .Setup(x => x.Categories.AnyAsync(
                It.IsAny<Expression<Func<Category, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var act = async () => await _categoryService.CreateAsync(request);

        // Assert
        await Assert.ThrowsAsync<ConflictException>(act);
    }

    [Fact]
    public async Task CreateAsync_WhenParentCategoryNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var request = new CreateCategoryRequestModel
        {
            ParentCategoryId = 99,
            CategoryName = "Running Shoes",
            CategoryCode = "RUNNING_SHOES",
            SortOrder = 1
        };

        _unitOfWorkMock
            .SetupSequence(x => x.Categories.AnyAsync(
                It.IsAny<Expression<Func<Category, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false) // category code not exists
            .ReturnsAsync(false); // parent category not exists

        // Act
        var act = async () => await _categoryService.CreateAsync(request);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(act);
    }

    [Fact]
    public async Task CreateAsync_WhenValidRootCategory_ShouldCreateCategory()
    {
        // Arrange
        var request = new CreateCategoryRequestModel
        {
            CategoryName = "Shoes",
            CategoryCode = "SHOES",
            Description = "Sport shoes",
            SortOrder = 1
        };

        _unitOfWorkMock
            .Setup(x => x.Categories.AnyAsync(
                It.IsAny<Expression<Func<Category, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _unitOfWorkMock
            .Setup(x => x.Categories.AddAsync(
                It.IsAny<Category>(),
                It.IsAny<CancellationToken>()))
            .Callback<Category, CancellationToken>((category, _) =>
            {
                category.CategoryId = 1;
            })
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _categoryService.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.CategoryId);
        Assert.Equal("Shoes", result.CategoryName);
        Assert.Equal("SHOES", result.CategoryCode);
        Assert.True(result.IsActive);

        _unitOfWorkMock.Verify(x => x.Categories.AddAsync(
            It.Is<Category>(c =>
                c.CategoryName == request.CategoryName &&
                c.CategoryCode == request.CategoryCode &&
                c.IsActive),
            It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetChildrenAsync_WhenCalled_ShouldReturnChildren()
    {
        // Arrange
        var children = new List<CategoryResponseModel>
        {
            new()
            {
                CategoryId = 2,
                ParentCategoryId = 1,
                CategoryName = "Running Shoes",
                CategoryCode = "RUNNING_SHOES",
                IsActive = true
            }
        };

        _unitOfWorkMock
            .Setup(x => x.Categories.FindAsync(
                It.IsAny<Expression<Func<Category, bool>>>(),
                It.IsAny<Expression<Func<Category, CategoryResponseModel>>>(),
                It.IsAny<Expression<Func<Category, object>>>(),
                true,
                true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(children);

        // Act
        var result = await _categoryService.GetChildrenAsync(1);

        // Assert
        Assert.Single(result);
        Assert.Equal("Running Shoes", result[0].CategoryName);
    }
}