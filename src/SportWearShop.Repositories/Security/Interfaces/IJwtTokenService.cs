using SportWearShop.Repositories.Entities;

namespace SportWearShop.Repositories.Security.Interfaces;

public interface IJwtTokenService
{
    Task<(string token, DateTime expiresAtUtc)> GenerateAccessTokenAsync(AppUser user);
}