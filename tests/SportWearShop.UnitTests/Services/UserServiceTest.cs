using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;
using SportWearShop.BusinessLogics.Exceptions;
using SportWearShop.BusinessLogics.ResponseModels.UserModels;
using SportWearShop.BusinessLogics.Services;
using SportWearShop.Repositories.Entities;
using SportWearShop.Repositories.UnitOfWorks;

namespace SportWearShop.UnitTests.Services;

public class UserServiceTests
{
    private readonly Mock<UserManager<AppUser>> _userManagerMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<UserService>> _loggerMock;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _userManagerMock = MockUserManager();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<UserService>>();

        _service = new UserService(
            _userManagerMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task UpdateProfileAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => _service.UpdateProfileAsync(1, null!));
    }

    [Fact]
    public async Task UpdateProfileAsync_ShouldThrowNotFoundException_WhenUserNotFound()
    {
        _userManagerMock
            .Setup(x => x.FindByIdAsync("1"))
            .ReturnsAsync((AppUser?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.UpdateProfileAsync(1, CreateUpdateProfileRequest()));
    }

    [Fact]
    public async Task UpdateProfileAsync_ShouldThrowBadRequestException_WhenIdentityUpdateFails()
    {
        var user = CreateUser();

        _userManagerMock
            .Setup(x => x.FindByIdAsync("1"))
            .ReturnsAsync(user);

        _userManagerMock
            .Setup(x => x.UpdateAsync(user))
            .ReturnsAsync(IdentityResult.Failed(
                new IdentityError { Description = "Update failed" }));

        await Assert.ThrowsAsync<BadRequestException>(
            () => _service.UpdateProfileAsync(1, CreateUpdateProfileRequest()));
    }

    [Fact]
    public async Task UpdateProfileAsync_ShouldUpdateUser_WhenRequestIsValid()
    {
        var user = CreateUser();

        var request = new UpdateUserProfileRequestModel
        {
            FirstName = "Updated",
            LastName = "User",
            PhoneNumber = "0999999999"
        };

        _userManagerMock
            .Setup(x => x.FindByIdAsync("1"))
            .ReturnsAsync(user);

        _userManagerMock
            .Setup(x => x.UpdateAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        await _service.UpdateProfileAsync(1, request);

        Assert.Equal("Updated", user.FirstName);
        Assert.Equal("User", user.LastName);
        Assert.Equal("0999999999", user.PhoneNumber);

        _userManagerMock.Verify(x => x.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task GetAddressByIdAsync_ShouldThrowNotFoundException_WhenAddressNotFound()
    {
        _unitOfWorkMock
            .Setup(x => x.UserAddresses.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<UserAddress, bool>>>(),
                It.IsAny<Expression<Func<UserAddress, UserAddressResponseModel>>>(),
                true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserAddressResponseModel?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.GetAddressByIdAsync(1));
    }

    [Fact]
    public async Task GetAddressByIdAsync_ShouldReturnAddress_WhenAddressExists()
    {
        var expected = CreateAddressResponse();

        _unitOfWorkMock
            .Setup(x => x.UserAddresses.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<UserAddress, bool>>>(),
                It.IsAny<Expression<Func<UserAddress, UserAddressResponseModel>>>(),
                true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _service.GetAddressByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(10, result.UserAddressId);
        Assert.Equal("Nguyen Van A", result.RecipientName);
    }

    [Fact]
    public async Task CreateAddressAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => _service.CreateAddressAsync(1, null!));
    }

    [Fact]
    public async Task CreateAddressAsync_ShouldAddAddress_WhenRequestIsValid()
    {
        // Arrange
        var user = CreateUser();
        SetupUsers(user);

        var request = CreateAddressRequest();

        _unitOfWorkMock
            .Setup(x => x.UserAddresses.AddAsync(
                It.IsAny<UserAddress>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _service.CreateAddressAsync(1, request);

        // Assert
        _unitOfWorkMock.Verify(x => x.UserAddresses.AddAsync(
            It.Is<UserAddress>(a =>
                a.UserId == 1 &&
                a.RecipientName == "Nguyen Van A" &&
                a.CountryCode == "VN"),
            It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAddressAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => _service.UpdateAddressAsync(1, 10, null!));
    }

    [Fact]
    public async Task UpdateAddressAsync_ShouldThrowNotFoundException_WhenAddressNotFound()
    {
        _unitOfWorkMock
            .Setup(x => x.UserAddresses.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<UserAddress, bool>>>(),
                It.IsAny<Expression<Func<UserAddress, UserAddress>>>(),
                false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserAddress?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.UpdateAddressAsync(1, 10, CreateUpdateAddressRequest()));
    }

    [Fact]
    public async Task UpdateAddressAsync_ShouldUpdateAddress_WhenRequestIsValid()
    {
        var address = CreateAddressEntity();
        var request = CreateUpdateAddressRequest();

        _unitOfWorkMock
            .Setup(x => x.UserAddresses.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<UserAddress, bool>>>(),
                It.IsAny<Expression<Func<UserAddress, UserAddress>>>(),
                false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(address);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _service.UpdateAddressAsync(1, 10, request);

        Assert.Equal("Updated Recipient", address.RecipientName);
        Assert.Equal("VN", address.CountryCode);

        _unitOfWorkMock.Verify(x => x.UserAddresses.Update(address), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAddressAsync_ShouldThrowNotFoundException_WhenAddressNotFound()
    {
        _unitOfWorkMock
            .Setup(x => x.UserAddresses.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<UserAddress, bool>>>(),
                It.IsAny<Expression<Func<UserAddress, UserAddress>>>(),
                false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserAddress?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.DeleteAddressAsync(1, 10));
    }

    [Fact]
    public async Task DeleteAddressAsync_ShouldRemoveAddress_WhenAddressExists()
    {
        var address = CreateAddressEntity();

        _unitOfWorkMock
            .Setup(x => x.UserAddresses.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<UserAddress, bool>>>(),
                It.IsAny<Expression<Func<UserAddress, UserAddress>>>(),
                false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(address);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _service.DeleteAddressAsync(1, 10);

        _unitOfWorkMock.Verify(x => x.UserAddresses.Remove(address), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetUserDetailAsync_ShouldThrowNotFoundException_WhenUserNotFound()
    {
        // Arrange
        SetupUsers();

        // Act
        var act = async () => await _service.GetUserDetailAsync(99);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(act);
    }

    [Fact]
    public async Task ActivateUserAsync_ShouldThrowNotFoundException_WhenUserNotFound()
    {
        _userManagerMock
            .Setup(x => x.FindByIdAsync("99"))
            .ReturnsAsync((AppUser?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.ActivateUserAsync(99));
    }

    [Fact]
    public async Task ActivateUserAsync_ShouldActivateUser_WhenUserExists()
    {
        var user = CreateUser();
        user.IsActive = false;

        _userManagerMock
            .Setup(x => x.FindByIdAsync("1"))
            .ReturnsAsync(user);

        _userManagerMock
            .Setup(x => x.UpdateAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        await _service.ActivateUserAsync(1);

        Assert.True(user.IsActive);
        _userManagerMock.Verify(x => x.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task DeactivateUserAsync_ShouldThrowNotFoundException_WhenUserNotFound()
    {
        _userManagerMock
            .Setup(x => x.FindByIdAsync("99"))
            .ReturnsAsync((AppUser?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.DeactivateUserAsync(99));
    }

    [Fact]
    public async Task DeactivateUserAsync_ShouldDeactivateUser_WhenUserExists()
    {
        var user = CreateUser();
        user.IsActive = true;

        _userManagerMock
            .Setup(x => x.FindByIdAsync("1"))
            .ReturnsAsync(user);

        _userManagerMock
            .Setup(x => x.UpdateAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        await _service.DeactivateUserAsync(1);

        Assert.False(user.IsActive);
        _userManagerMock.Verify(x => x.UpdateAsync(user), Times.Once);
    }

    private static Mock<UserManager<AppUser>> MockUserManager()
    {
        var store = new Mock<IUserStore<AppUser>>();

        return new Mock<UserManager<AppUser>>(
            store.Object,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!);
    }

    private static AppUser CreateUser()
    {
        return new AppUser
        {
            Id = 1,
            UserName = "test@gmail.com",
            Email = "test@gmail.com",
            FirstName = "Test",
            LastName = "User",
            PhoneNumber = "0123456789",
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };
    }

    private static UpdateUserProfileRequestModel CreateUpdateProfileRequest()
    {
        return new UpdateUserProfileRequestModel
        {
            FirstName = "Test",
            LastName = "User",
            PhoneNumber = "0123456789"
        };
    }

    private static CreateUserAddressRequestModel CreateAddressRequest()
    {
        return new CreateUserAddressRequestModel
        {
            RecipientName = " Nguyen Van A ",
            PhoneNumber = " 0900000000 ",
            AddressLine1 = " 123 Le Loi ",
            AddressLine2 = " Floor 2 ",
            Ward = " Ward 1 ",
            District = " District 1 ",
            City = " Ho Chi Minh ",
            Province = " Ho Chi Minh ",
            PostalCode = "700000",
            CountryCode = " vn ",
            IsDefaultShipping = true,
            IsDefaultBilling = false
        };
    }

    private static UpdateUserAddressRequestModel CreateUpdateAddressRequest()
    {
        return new UpdateUserAddressRequestModel
        {
            RecipientName = " Updated Recipient ",
            PhoneNumber = " 0911111111 ",
            AddressLine1 = " 456 Nguyen Hue ",
            AddressLine2 = null,
            Ward = " Ward 2 ",
            District = " District 1 ",
            City = " Ho Chi Minh ",
            Province = " Ho Chi Minh ",
            PostalCode = "700000",
            CountryCode = " vn ",
            IsDefaultShipping = false,
            IsDefaultBilling = true
        };
    }

    private static UserAddress CreateAddressEntity()
    {
        return new UserAddress
        {
            UserAddressId = 10,
            UserId = 1,
            RecipientName = "Nguyen Van A",
            PhoneNumber = "0900000000",
            AddressLine1 = "123 Le Loi",
            City = "Ho Chi Minh",
            CountryCode = "VN",
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };
    }

    private static UserAddressResponseModel CreateAddressResponse()
    {
        return new UserAddressResponseModel
        {
            UserAddressId = 10,
            RecipientName = "Nguyen Van A",
            PhoneNumber = "0900000000",
            AddressLine1 = "123 Le Loi",
            City = "Ho Chi Minh",
            CountryCode = "VN",
            IsDefaultShipping = true,
            IsDefaultBilling = false
        };
    }
    private void SetupUsers(params AppUser[] users)
    {
        var mockUsers = users.BuildMock();

        _userManagerMock
            .Setup(x => x.Users)
            .Returns(mockUsers);
    }
}