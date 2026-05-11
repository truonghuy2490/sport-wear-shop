using SportWearShop.Repositories.Entities;

namespace SportWearShop.Repositories.Interfaces;
public interface ICartRepository : IBaseRepository<Cart>
{
    Task<Cart?> GetActiveCartDetailAsync(
        long userId,
        CancellationToken cancellationToken = default);

    Task<Cart?> GetCartDetailByIdAsync(
        long cartId,
        CancellationToken cancellationToken = default);
}