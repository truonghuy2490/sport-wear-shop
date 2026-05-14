using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportWearShop.BusinessLogics.Interfaces;
using System.Security.Claims;
using SportWearShop.BusinessLogics.ResponseModels.CartModels;

namespace SportWearShop.APIs.Controllers;

[Route("api/cart")]
[ApiController]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyCartAsync(
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();

        var result = await _cartService.GetMyCartAsync(
            userId,
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItemAsync(
        [FromBody] AddCartItemRequestModel request,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();

        var result = await _cartService.AddItemAsync(
            userId,
            request,
            cancellationToken);

        return Ok(result);
    }

    [HttpPut("items/{cartItemId:long}")]
    public async Task<IActionResult> UpdateItemQuantityAsync(
        [FromRoute] long cartItemId,
        [FromBody] UpdateCartItemQuantityRequestModel request,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();

        var result = await _cartService.UpdateItemQuantityAsync(
            userId,
            cartItemId,
            request,
            cancellationToken);

        return Ok(result);
    }

    [HttpDelete("items/{cartItemId:long}")]
    public async Task<IActionResult> RemoveItemAsync(
        [FromRoute] long cartItemId,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();

        var result = await _cartService.RemoveItemAsync(
            userId,
            cartItemId,
            cancellationToken);

        return Ok(result);
    }

    [HttpDelete]
    public async Task<IActionResult> ClearCartAsync(
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();

        await _cartService.ClearCartAsync(
            userId,
            cancellationToken);

        return NoContent();
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetCartSummaryAsync(
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();

        var result = await _cartService.GetCartSummaryAsync(
            userId,
            cancellationToken);

        return Ok(result);
    }

    private long GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(userIdClaim) ||
            !long.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user identity.");
        }

        return userId;
    }
}