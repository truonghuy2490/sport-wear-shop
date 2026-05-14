using Microsoft.Extensions.Logging;
using SportWearShop.BusinessLogics.Exceptions;
using SportWearShop.BusinessLogics.Interfaces;
using SportWearShop.BusinessLogics.ResponseModels.CartModels;
using SportWearShop.Repositories.Entities;
using SportWearShop.Repositories.Enums;
using SportWearShop.Repositories.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SportWearShop.BusinessLogics.Services;

public class CartService : ICartService
{
    private const string ActiveCartStatus = "Active";

    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CartService> _logger;

    public CartService(
        IUnitOfWork unitOfWork,
        ILogger<CartService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<CartResponseModel> GetMyCartAsync(
        long userId,
        CancellationToken cancellationToken = default)
    {
        var cart = await GetOrCreateActiveCartAsync(userId, cancellationToken);

        return await BuildCartResponseAsync(cart.CartId, cancellationToken);
    }

    public async Task<CartResponseModel> AddItemAsync(
        long userId,
        AddCartItemRequestModel request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.Quantity <= 0)
        {
            throw new BadRequestException("Quantity must be greater than 0.");
        }

        var cart = await GetOrCreateActiveCartAsync(userId, cancellationToken);

        var variant = await _unitOfWork.ProductVariants.FirstOrDefaultAsync(
            predicate: variant => variant.ProductVariantId == request.ProductVariantId
                                  && variant.Status == ProductVariantStatus.Active,
            selector: variant => variant,
            asNoTracking: false,
            cancellationToken: cancellationToken,
            includes: variant => variant.InventoryStock!);

        if (variant == null)
        {
            throw new NotFoundException(
                $"Product variant with ID {request.ProductVariantId} was not found.");
        }

        var availableQuantity = variant.InventoryStock == null
            ? 0
            : variant.InventoryStock.QuantityOnHand - variant.InventoryStock.QuantityReserved;

        if (availableQuantity <= 0)
        {
            throw new BadRequestException("Product variant is out of stock.");
        }

        var existingItem = await _unitOfWork.CartItems.FirstOrDefaultAsync(
            predicate: item => item.CartId == cart.CartId
                               && item.ProductVariantId == request.ProductVariantId,
            selector: item => item,
            asNoTracking: false,
            cancellationToken: cancellationToken);

        var newQuantity = request.Quantity;

        if (existingItem != null)
        {
            newQuantity += existingItem.Quantity;
        }

        if (newQuantity > availableQuantity)
        {
            throw new BadRequestException("Requested quantity exceeds available stock.");
        }

        var unitPrice = variant.SalePrice ?? variant.ListPrice;
        var now = DateTime.UtcNow;

        if (existingItem == null)
        {
            var cartItem = new CartItem
            {
                CartId = cart.CartId,
                ProductVariantId = request.ProductVariantId,
                Quantity = request.Quantity,
                UnitPrice = unitPrice,
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            };

            await _unitOfWork.CartItems.AddAsync(cartItem, cancellationToken);
        }
        else
        {
            existingItem.Quantity = newQuantity;
            existingItem.UnitPrice = unitPrice;
            existingItem.UpdatedAtUtc = now;

            _unitOfWork.CartItems.Update(existingItem);
        }

        cart.UpdatedAtUtc = now;
        _unitOfWork.Carts.Update(cart);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await BuildCartResponseAsync(cart.CartId, cancellationToken);
    }

    public async Task<CartResponseModel> UpdateItemQuantityAsync(
        long userId,
        long cartItemId,
        UpdateCartItemQuantityRequestModel request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.Quantity <= 0)
        {
            throw new BadRequestException("Quantity must be greater than 0.");
        }

        var cart = await GetOrCreateActiveCartAsync(userId, cancellationToken);

        var cartItem = await _unitOfWork.CartItems.FirstOrDefaultAsync(
            predicate: item => item.CartItemId == cartItemId
                               && item.CartId == cart.CartId,
            selector: item => item,
            asNoTracking: false,
            cancellationToken: cancellationToken,
            includes: item => item.ProductVariant.InventoryStock!);

        if (cartItem == null)
        {
            throw new NotFoundException(
                $"Cart item with ID {cartItemId} was not found.");
        }

        if (cartItem.ProductVariant.Status != ProductVariantStatus.Active)
        {
            throw new BadRequestException("Product variant is not available.");
        }

        var stock = cartItem.ProductVariant.InventoryStock;
        var availableQuantity = stock == null
            ? 0
            : stock.QuantityOnHand - stock.QuantityReserved;

        if (request.Quantity > availableQuantity)
        {
            throw new BadRequestException("Requested quantity exceeds available stock.");
        }

        cartItem.Quantity = request.Quantity;
        cartItem.UnitPrice = cartItem.ProductVariant.SalePrice ?? cartItem.ProductVariant.ListPrice;
        cartItem.UpdatedAtUtc = DateTime.UtcNow;

        cart.UpdatedAtUtc = DateTime.UtcNow;

        _unitOfWork.CartItems.Update(cartItem);
        _unitOfWork.Carts.Update(cart);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await BuildCartResponseAsync(cart.CartId, cancellationToken);
    }

    public async Task<CartResponseModel> RemoveItemAsync(
        long userId,
        long cartItemId,
        CancellationToken cancellationToken = default)
    {
        var cart = await GetOrCreateActiveCartAsync(userId, cancellationToken);

        var cartItem = await _unitOfWork.CartItems.FirstOrDefaultAsync(
            predicate: item => item.CartItemId == cartItemId
                               && item.CartId == cart.CartId,
            selector: item => item,
            asNoTracking: false,
            cancellationToken: cancellationToken);

        if (cartItem == null)
        {
            throw new NotFoundException(
                $"Cart item with ID {cartItemId} was not found.");
        }

        _unitOfWork.CartItems.Remove(cartItem);

        cart.UpdatedAtUtc = DateTime.UtcNow;
        _unitOfWork.Carts.Update(cart);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await BuildCartResponseAsync(cart.CartId, cancellationToken);
    }

    public async Task ClearCartAsync(
        long userId,
        CancellationToken cancellationToken = default)
    {
        var cart = await GetOrCreateActiveCartAsync(userId, cancellationToken);

        var cartItems = await _unitOfWork.CartItems.FindAsync(
            filter: item => item.CartId == cart.CartId,
            selector: item => item,
            asNoTracking: false,
            cancellationToken: cancellationToken);

        foreach (var item in cartItems)
        {
            _unitOfWork.CartItems.Remove(item);
        }

        cart.UpdatedAtUtc = DateTime.UtcNow;
        _unitOfWork.Carts.Update(cart);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<CartSummaryResponseModel> GetCartSummaryAsync(
        long userId,
        CancellationToken cancellationToken = default)
    {
        var cart = await GetOrCreateActiveCartAsync(userId, cancellationToken);

        var items = await _unitOfWork.CartItems.FindAsync(
            filter: item => item.CartId == cart.CartId,
            selector: item => new
            {
                item.Quantity,
                item.UnitPrice
            },
            asNoTracking: true,
            cancellationToken: cancellationToken);

        return new CartSummaryResponseModel
        {
            TotalItems = items.Count,
            TotalQuantity = items.Sum(item => item.Quantity),
            SubTotal = items.Sum(item => item.Quantity * item.UnitPrice)
        };
    }

    private async Task<Cart> GetOrCreateActiveCartAsync(
        long userId,
        CancellationToken cancellationToken)
    {
        var cart = await _unitOfWork.Carts.FirstOrDefaultAsync(
            predicate: cart => cart.UserId == userId
                               && cart.CartStatus == ActiveCartStatus,
            selector: cart => cart,
            asNoTracking: false,
            cancellationToken: cancellationToken);

        if (cart != null)
        {
            return cart;
        }

        var now = DateTime.UtcNow;

        cart = new Cart
        {
            UserId = userId,
            CartStatus = ActiveCartStatus,
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };

        await _unitOfWork.Carts.AddAsync(cart, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return cart;
    }

    
    private async Task<CartResponseModel> BuildCartResponseAsync(
        long userId,
        CancellationToken cancellationToken)
    {
        var cart = await _unitOfWork.Carts.GetActiveCartDetailAsync(
            userId,
            cancellationToken);

        if (cart == null)
        {
            throw new NotFoundException("Active cart was not found.");
        }

        var items = cart.CartItems.Select(item =>
        {
            var stock = item.ProductVariant.InventoryStock;

            var availableQuantity = stock == null
                ? 0
                : stock.QuantityOnHand - stock.QuantityReserved;

            return new CartItemResponseModel
            {
                CartItemId = item.CartItemId,
                ProductVariantId = item.ProductVariantId,
                Sku = item.ProductVariant.Sku,
                ProductName = item.ProductVariant.Product.ProductName,
                ProductCode = item.ProductVariant.Product.ProductCode,
                Slug = item.ProductVariant.Product.Slug,

                ThumbnailUrl = item.ProductVariant.ProductImages
                    .OrderBy(image => image.SortOrder)
                    .Select(image => image.ImageUrl)
                    .FirstOrDefault()
                    ?? item.ProductVariant.Product.ProductImages
                        .Where(image => image.IsPrimary)
                        .OrderBy(image => image.SortOrder)
                        .Select(image => image.ImageUrl)
                        .FirstOrDefault(),

                ColorCode = item.ProductVariant.ColorCode,
                ColorName = item.ProductVariant.ColorName,
                SizeCode = item.ProductVariant.SizeCode,
                SizeLabel = item.ProductVariant.SizeLabel,

                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity,
                LineTotal = item.UnitPrice * item.Quantity,

                QuantityOnHand = stock?.QuantityOnHand ?? 0,
                IsAvailable = item.ProductVariant.Status == ProductVariantStatus.Active
                            && item.Quantity <= availableQuantity
            };
        }).ToList();

        return new CartResponseModel
        {
            CartId = cart.CartId,
            UserId = cart.UserId,
            CartStatus = cart.CartStatus,
            UpdatedAtUtc = cart.UpdatedAtUtc,
            Items = items,
            TotalItems = items.Count,
            TotalQuantity = items.Sum(item => item.Quantity),
            SubTotal = items.Sum(item => item.LineTotal)
        };
    }

}