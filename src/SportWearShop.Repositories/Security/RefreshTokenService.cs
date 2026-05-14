using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SportWearShop.Repositories.Security.Interfaces;

namespace SportWearShop.Repositories.Security;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly IConfiguration _configuration;
    public RefreshTokenService(
        IConfiguration configuration
    )
    {
        _configuration = configuration;
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(randomBytes);
    }

    public DateTime GetRefreshTokenExpiresAtUtc()
    {
        var refreshTokenExpirationDays =
        int.Parse(_configuration["Jwt:RefreshTokenExpirationDays"]!);

        return DateTime.UtcNow.AddDays(refreshTokenExpirationDays);
    }
}