using Microsoft.EntityFrameworkCore;
using SportWearShop.Repositories.Entities;
using SportWearShop.Repositories.Interfaces;

namespace SportWearShop.Repositories.Implementations;


public class CartRepository : BaseRepository<Cart>, ICartRepository
{
    private const string ActiveCartStatus = "Active";

    private readonly AppDbContext _context;

    public CartRepository(AppDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<Cart?> GetActiveCartDetailAsync(
        long userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Carts
            .AsNoTracking()
            .AsSplitQuery()
            .Include(cart => cart.CartItems)
                .ThenInclude(item => item.ProductVariant)
                    .ThenInclude(variant => variant.Product)
            .Include(cart => cart.CartItems)
                .ThenInclude(item => item.ProductVariant)
                    .ThenInclude(variant => variant.InventoryStock)
            .Include(cart => cart.CartItems)
                .ThenInclude(item => item.ProductVariant)
                    .ThenInclude(variant => variant.ProductImages)
            .Include(cart => cart.CartItems)
                .ThenInclude(item => item.ProductVariant)
                    .ThenInclude(variant => variant.Product)
                        .ThenInclude(product => product.ProductImages)
            .FirstOrDefaultAsync(
                cart => cart.UserId == userId &&
                        cart.CartStatus == ActiveCartStatus,
                cancellationToken);
    }

    public async Task<Cart?> GetCartDetailByIdAsync(
        long cartId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Carts
            .AsNoTracking()
            .AsSplitQuery()
            .Include(cart => cart.CartItems)
                .ThenInclude(item => item.ProductVariant)
                    .ThenInclude(variant => variant.Product)
            .Include(cart => cart.CartItems)
                .ThenInclude(item => item.ProductVariant)
                    .ThenInclude(variant => variant.InventoryStock)
            .Include(cart => cart.CartItems)
                .ThenInclude(item => item.ProductVariant)
                    .ThenInclude(variant => variant.ProductImages)
            .Include(cart => cart.CartItems)
                .ThenInclude(item => item.ProductVariant)
                    .ThenInclude(variant => variant.Product)
                        .ThenInclude(product => product.ProductImages)
            .FirstOrDefaultAsync(
                cart => cart.CartId == cartId,
                cancellationToken);
    }
}