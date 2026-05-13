using System;
using System.Collections.Generic;
using System.Text;
using SportWearShop.BusinessLogics.ResponseModels.CartModels;

namespace SportWearShop.BusinessLogics.Interfaces;

public interface ICartService
{
    Task<CartResponseModel> GetMyCartAsync(
        long userId,
        CancellationToken cancellationToken = default);

    Task<CartResponseModel> AddItemAsync(
        long userId,
        AddCartItemRequestModel request,
        CancellationToken cancellationToken = default);

    Task<CartResponseModel> UpdateItemQuantityAsync(
        long userId,
        long cartItemId,
        UpdateCartItemQuantityRequestModel request,
        CancellationToken cancellationToken = default);

    Task<CartResponseModel> RemoveItemAsync(
        long userId,
        long cartItemId,
        CancellationToken cancellationToken = default);

    Task ClearCartAsync(
        long userId,
        CancellationToken cancellationToken = default);

    Task<CartSummaryResponseModel> GetCartSummaryAsync(
        long userId,
        CancellationToken cancellationToken = default);
}