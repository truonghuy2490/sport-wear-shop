using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SportWearShop.BusinessLogics.Constants;
using SportWearShop.BusinessLogics.Exceptions;
using SportWearShop.BusinessLogics.Interfaces;
using SportWearShop.BusinessLogics.ResponseModels.AuthModels;
using SportWearShop.BusinessLogics.ResponseModels.UserModels;
using SportWearShop.BussinessLogics.Constants;
using SportWearShop.Repositories.Entities;
using SportWearShop.Repositories.Security.Interfaces;
using SportWearShop.Repositories.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportWearShop.BusinessLogics.Services;
public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<AppUser> userManager,
        IJwtTokenService jwtTokenService,
        IRefreshTokenService refreshTokenService,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
        _refreshTokenService = refreshTokenService;
        _logger = logger;
    }

    public async Task<AuthResponseModel> RegisterAsync(
        RegisterRequestModel request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Register attempt for email: {Email}",
            request.Email);

        var existingUser = await _userManager.FindByEmailAsync(request.Email);

        if (existingUser is not null)
        {
            throw new BadRequestException("Email already exists.");
        }

        var user = new AppUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        var createResult = await _userManager.CreateAsync(
            user,
            request.Password);

        if (!createResult.Succeeded)
        {
            var error = createResult.Errors.First().Description;
            throw new BadRequestException(error);
        }

        // Add Customer Role
        var roleResult = await _userManager.AddToRoleAsync(user, AppRoles.Customer);

        if (!roleResult.Succeeded)
        {
            var error = createResult.Errors.First().Description;
            throw new BadRequestException(error);
        }

        _logger.LogInformation(
            "Register success. UserId: {UserId}",
            user.Id);

        return await GenerateAuthResponseAsync(user);
    }

    public async Task<AuthResponseModel> LoginAsync(
        LoginRequestModel request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Login attempt for email: {Email}",
            request.Email);

        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            throw new UnauthorizedException("Invalid email or password.");
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedException("User account was disable.");
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!isPasswordValid)
        {
            throw new UnauthorizedException("Invalid email or password.");
        }

        _logger.LogInformation(
            "Login success. UserId: {UserId}",
            user.Id);

        return await GenerateAuthResponseAsync(user);
    }

    public async Task<AuthResponseModel> RefreshTokenAsync(
        long userId,
        RefreshTokenRequestModel request,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.Users.FirstAsync(
                u => u.Id == userId && u.IsActive,
                cancellationToken
        );

        if (user is null)
        {
            throw new UnauthorizedException("Invalid refresh token.");
        }

        var refreshTokenExpiresAt = await _userManager.GetAuthenticationTokenAsync(
                user,
                AuthTokenProviders.SportWearShop,
                AuthTokenNames.RefreshTokenExpiresAt);
        
        if (string.IsNullOrWhiteSpace(refreshTokenExpiresAt) ||
            !DateTime.TryParse(refreshTokenExpiresAt, out var expiresAtUtc) ||
            expiresAtUtc <= DateTime.UtcNow)
        {
            throw new UnauthorizedException("Refresh token expired.");
        }

        var refreshToken = await _userManager.GetAuthenticationTokenAsync(
            user,
            AuthTokenProviders.SportWearShop,
            AuthTokenNames.RefreshToken
        );

        if(refreshToken != request.RefreshToken)
        {
            throw new UnauthorizedException("Invalid refresh token");
        }

        _logger.LogInformation("Refresh token success. UserId: {UserId}", user.Id);

        return await GenerateAuthResponseAsync(user);
    }

    public async Task LogoutAsync(
        long userId,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(
                u => u.Id == userId && u.IsActive,
                cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        await _userManager.RemoveAuthenticationTokenAsync(
            user,
            AuthTokenProviders.SportWearShop,
            AuthTokenNames.RefreshToken);

        await _userManager.RemoveAuthenticationTokenAsync(
            user,
            AuthTokenProviders.SportWearShop,
            AuthTokenNames.RefreshTokenExpiresAt);

        _logger.LogInformation("Logout success. UserId: {UserId}", userId);
    }

    private async Task<AuthResponseModel> GenerateAuthResponseAsync(AppUser user)
    {
        var accessToken = await _jwtTokenService.GenerateAccessTokenAsync(user);

        var refreshToken = _refreshTokenService.GenerateRefreshToken();

        var refreshTokenExpiresAtUtc = _refreshTokenService.GetRefreshTokenExpiresAtUtc();

        await _userManager.SetAuthenticationTokenAsync(
            user,
            AuthTokenProviders.SportWearShop,
            AuthTokenNames.RefreshToken,
            refreshToken);

        await _userManager.SetAuthenticationTokenAsync(
            user,
            AuthTokenProviders.SportWearShop,
            AuthTokenNames.RefreshTokenExpiresAt,
            refreshTokenExpiresAtUtc.ToString("O"));

        return new AuthResponseModel
        {
            AccessToken = accessToken.token,
            AccessTokenExpiresAtUtc = accessToken.expiresAtUtc,
            RefreshToken = refreshToken,
            RefreshTokenExpiresAtUtc = refreshTokenExpiresAtUtc
        };
    }
}

/*
    In case Google Login:
    - Save in AspNetUserLogins Table
        LoginProvider = Google
        ProviderKey = google_user_id
        UserId = 1

    In case refresh token
    - AspNetUserTokens Table: Refresh Token nhung k co expired 
        UserId = 1      
        LoginProvider = SportWearShop
        Name = RefreshToken
        Value = abcxyz...
        UserId = 1      
        LoginProvider = SportWearShop
        Name = RefreshTokenExpiresAt
        Value = abcxyz...
*/