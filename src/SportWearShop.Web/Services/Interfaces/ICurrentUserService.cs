namespace SportWearShop.Web.Services.Interfaces;

public interface ICurrentUserService
{
    bool IsAuthenticated { get; }

    long? UserId { get; }

    string? Email { get; }

    string? Name { get; }

    string DisplayName { get; }
}