namespace SportWearShop.Repositories.Security.Interfaces;

public interface IRefreshTokenService  
{
    string GenerateRefreshToken();
    DateTime GetRefreshTokenExpiresAtUtc();
}