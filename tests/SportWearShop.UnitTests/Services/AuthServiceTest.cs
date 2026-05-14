using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MockQueryable;
using MockQueryable.Moq;
using Moq;
using SportWearShop.BusinessLogics.Constants;
using SportWearShop.BusinessLogics.Exceptions;
using SportWearShop.BusinessLogics.ResponseModels.AuthModels;
using SportWearShop.BusinessLogics.Services;
using SportWearShop.BussinessLogics.Constants;
using SportWearShop.Repositories.Entities;
using SportWearShop.Repositories.Security.Interfaces;

namespace SportWearShop.UnitTests.Services;

public class AuthServiceTests
{
    private readonly Mock<UserManager<AppUser>> _userManagerMock;
    private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
    private readonly Mock<IRefreshTokenService> _refreshTokenServiceMock;
    private readonly Mock<ILogger<AuthService>> _loggerMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userManagerMock = MockUserManager();
        _jwtTokenServiceMock = new Mock<IJwtTokenService>();
        _refreshTokenServiceMock = new Mock<IRefreshTokenService>();
        _loggerMock = new Mock<ILogger<AuthService>>();

        _authService = new AuthService(
            _userManagerMock.Object,
            _jwtTokenServiceMock.Object,
            _refreshTokenServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_WhenEmailExists_ShouldThrowBadRequestException()
    {
        var request = CreateRegisterRequest();

        _userManagerMock
            .Setup(x => x.FindByEmailAsync(request.Email))
            .ReturnsAsync(new AppUser { Email = request.Email });

        await Assert.ThrowsAsync<BadRequestException>(
            () => _authService.RegisterAsync(request));
    }

    [Fact]
    public async Task RegisterAsync_WhenCreateFails_ShouldThrowBadRequestException()
    {
        var request = CreateRegisterRequest();

        _userManagerMock
            .Setup(x => x.FindByEmailAsync(request.Email))
            .ReturnsAsync((AppUser?)null);

        _userManagerMock
            .Setup(x => x.CreateAsync(It.IsAny<AppUser>(), request.Password))
            .ReturnsAsync(IdentityResult.Failed(
                new IdentityError { Description = "Create user failed" }));

        await Assert.ThrowsAsync<BadRequestException>(
            () => _authService.RegisterAsync(request));
    }

    [Fact]
    public async Task RegisterAsync_WhenValid_ShouldReturnAuthResponse()
    {
        var request = CreateRegisterRequest();
        var accessExpiresAt = DateTime.UtcNow.AddMinutes(30);
        var refreshExpiresAt = DateTime.UtcNow.AddDays(7);

        _userManagerMock
            .Setup(x => x.FindByEmailAsync(request.Email))
            .ReturnsAsync((AppUser?)null);

        _userManagerMock
            .Setup(x => x.CreateAsync(It.IsAny<AppUser>(), request.Password))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock
            .Setup(x => x.AddToRoleAsync(It.IsAny<AppUser>(), AppRoles.Customer))
            .ReturnsAsync(IdentityResult.Success);

        SetupTokenGeneration(accessExpiresAt, refreshExpiresAt);

        var result = await _authService.RegisterAsync(request);

        Assert.Equal("access-token", result.AccessToken);
        Assert.Equal("refresh-token", result.RefreshToken);
        Assert.Equal(accessExpiresAt, result.AccessTokenExpiresAtUtc);
        Assert.Equal(refreshExpiresAt, result.RefreshTokenExpiresAtUtc);

        _userManagerMock.Verify(x => x.CreateAsync(
            It.Is<AppUser>(u =>
                u.Email == request.Email &&
                u.UserName == request.Email &&
                u.FirstName == request.FirstName &&
                u.LastName == request.LastName &&
                u.IsActive),
            request.Password), Times.Once);

        _userManagerMock.Verify(x => x.AddToRoleAsync(
            It.IsAny<AppUser>(),
            AppRoles.Customer), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WhenUserNotFound_ShouldThrowUnauthorizedException()
    {
        var request = CreateLoginRequest();

        _userManagerMock
            .Setup(x => x.FindByEmailAsync(request.Email))
            .ReturnsAsync((AppUser?)null);

        await Assert.ThrowsAsync<UnauthorizedException>(
            () => _authService.LoginAsync(request));
    }

    [Fact]
    public async Task LoginAsync_WhenUserInactive_ShouldThrowUnauthorizedException()
    {
        var request = CreateLoginRequest();
        var user = CreateUser();
        user.IsActive = false;

        _userManagerMock
            .Setup(x => x.FindByEmailAsync(request.Email))
            .ReturnsAsync(user);

        await Assert.ThrowsAsync<UnauthorizedException>(
            () => _authService.LoginAsync(request));
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordInvalid_ShouldThrowUnauthorizedException()
    {
        var request = CreateLoginRequest();
        var user = CreateUser();

        _userManagerMock
            .Setup(x => x.FindByEmailAsync(request.Email))
            .ReturnsAsync(user);

        _userManagerMock
            .Setup(x => x.CheckPasswordAsync(user, request.Password))
            .ReturnsAsync(false);

        await Assert.ThrowsAsync<UnauthorizedException>(
            () => _authService.LoginAsync(request));
    }

    [Fact]
    public async Task LoginAsync_WhenValid_ShouldReturnAuthResponse()
    {
        var request = CreateLoginRequest();
        var user = CreateUser();

        var accessExpiresAt = DateTime.UtcNow.AddMinutes(30);
        var refreshExpiresAt = DateTime.UtcNow.AddDays(7);

        _userManagerMock
            .Setup(x => x.FindByEmailAsync(request.Email))
            .ReturnsAsync(user);

        _userManagerMock
            .Setup(x => x.CheckPasswordAsync(user, request.Password))
            .ReturnsAsync(true);

        SetupTokenGeneration(accessExpiresAt, refreshExpiresAt);

        var result = await _authService.LoginAsync(request);

        Assert.Equal("access-token", result.AccessToken);
        Assert.Equal("refresh-token", result.RefreshToken);
    }

    [Fact]
    public async Task RefreshTokenAsync_WhenTokenExpired_ShouldThrowUnauthorizedException()
    {
        var user = CreateUser();
        SetupUsers(user);

        var request = new RefreshTokenRequestModel
        {
            UserId = user.Id,
            RefreshToken = "old-refresh-token"
        };

        _userManagerMock
            .Setup(x => x.GetAuthenticationTokenAsync(
                user,
                AuthTokenProviders.SportWearShop,
                AuthTokenNames.RefreshTokenExpiresAt))
            .ReturnsAsync(DateTime.UtcNow.AddDays(-1).ToString("O"));

        await Assert.ThrowsAsync<UnauthorizedException>(
            () => _authService.RefreshTokenAsync(request));
    }

    [Fact]
    public async Task RefreshTokenAsync_WhenRefreshTokenInvalid_ShouldThrowUnauthorizedException()
    {
        var user = CreateUser();
        SetupUsers(user);

        var request = new RefreshTokenRequestModel
        {
            UserId = user.Id,
            RefreshToken = "wrong-token"
        };

        _userManagerMock
            .Setup(x => x.GetAuthenticationTokenAsync(
                user,
                AuthTokenProviders.SportWearShop,
                AuthTokenNames.RefreshTokenExpiresAt))
            .ReturnsAsync(DateTime.UtcNow.AddDays(7).ToString("O"));

        _userManagerMock
            .Setup(x => x.GetAuthenticationTokenAsync(
                user,
                AuthTokenProviders.SportWearShop,
                AuthTokenNames.RefreshToken))
            .ReturnsAsync("correct-token");

        await Assert.ThrowsAsync<UnauthorizedException>(
            () => _authService.RefreshTokenAsync(request));
    }

    [Fact]
    public async Task RefreshTokenAsync_WhenValid_ShouldReturnNewAuthResponse()
    {
        var user = CreateUser();
        SetupUsers(user);

        var request = new RefreshTokenRequestModel
        {
            UserId = user.Id,
            RefreshToken = "old-refresh-token"
        };

        var accessExpiresAt = DateTime.UtcNow.AddMinutes(30);
        var refreshExpiresAt = DateTime.UtcNow.AddDays(7);

        _userManagerMock
            .Setup(x => x.GetAuthenticationTokenAsync(
                user,
                AuthTokenProviders.SportWearShop,
                AuthTokenNames.RefreshTokenExpiresAt))
            .ReturnsAsync(DateTime.UtcNow.AddDays(1).ToString("O"));

        _userManagerMock
            .Setup(x => x.GetAuthenticationTokenAsync(
                user,
                AuthTokenProviders.SportWearShop,
                AuthTokenNames.RefreshToken))
            .ReturnsAsync("old-refresh-token");

        SetupTokenGeneration(accessExpiresAt, refreshExpiresAt);

        var result = await _authService.RefreshTokenAsync(request);

        Assert.Equal("access-token", result.AccessToken);
        Assert.Equal("refresh-token", result.RefreshToken);
    }

    [Fact]
    public async Task LogoutAsync_WhenUserNotFound_ShouldThrowNotFoundException()
    {
        SetupUsers();

        await Assert.ThrowsAsync<NotFoundException>(
            () => _authService.LogoutAsync(999));
    }

    [Fact]
    public async Task LogoutAsync_WhenValid_ShouldRemoveRefreshTokens()
    {
        var user = CreateUser();
        SetupUsers(user);

        _userManagerMock
            .Setup(x => x.RemoveAuthenticationTokenAsync(
                user,
                AuthTokenProviders.SportWearShop,
                AuthTokenNames.RefreshToken))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock
            .Setup(x => x.RemoveAuthenticationTokenAsync(
                user,
                AuthTokenProviders.SportWearShop,
                AuthTokenNames.RefreshTokenExpiresAt))
            .ReturnsAsync(IdentityResult.Success);

        await _authService.LogoutAsync(user.Id);

        _userManagerMock.Verify(x => x.RemoveAuthenticationTokenAsync(
            user,
            AuthTokenProviders.SportWearShop,
            AuthTokenNames.RefreshToken), Times.Once);

        _userManagerMock.Verify(x => x.RemoveAuthenticationTokenAsync(
            user,
            AuthTokenProviders.SportWearShop,
            AuthTokenNames.RefreshTokenExpiresAt), Times.Once);
    }

    private void SetupUsers(params AppUser[] users)
    {
        var mockUsers = users.BuildMock();

        _userManagerMock
            .Setup(x => x.Users)
            .Returns(mockUsers);
    }

    private void SetupTokenGeneration(
        DateTime accessExpiresAt,
        DateTime refreshExpiresAt)
    {
        _jwtTokenServiceMock
            .Setup(x => x.GenerateAccessTokenAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(("access-token", accessExpiresAt));

        _refreshTokenServiceMock
            .Setup(x => x.GenerateRefreshToken())
            .Returns("refresh-token");

        _refreshTokenServiceMock
            .Setup(x => x.GetRefreshTokenExpiresAtUtc())
            .Returns(refreshExpiresAt);

        _userManagerMock
            .Setup(x => x.SetAuthenticationTokenAsync(
                It.IsAny<AppUser>(),
                AuthTokenProviders.SportWearShop,
                AuthTokenNames.RefreshToken,
                "refresh-token"))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock
            .Setup(x => x.SetAuthenticationTokenAsync(
                It.IsAny<AppUser>(),
                AuthTokenProviders.SportWearShop,
                AuthTokenNames.RefreshTokenExpiresAt,
                It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
    }

    private static RegisterRequestModel CreateRegisterRequest()
    {
        return new RegisterRequestModel
        {
            Email = "test@gmail.com",
            Password = "Password@123",
            FirstName = "Test",
            LastName = "User"
        };
    }

    private static LoginRequestModel CreateLoginRequest()
    {
        return new LoginRequestModel
        {
            Email = "test@gmail.com",
            Password = "Password@123"
        };
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
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };
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
}