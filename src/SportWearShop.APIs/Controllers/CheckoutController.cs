using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportWearShop.BusinessLogics.Interfaces;
using SportWearShop.BusinessLogics.ResponseModels.CheckOutModels;

namespace SportWearShop.APIs.Controllers;

[Route("api/checkout")]
[ApiController]
[Authorize]
public class CheckoutController : ControllerBase
{
    private readonly ICheckoutService _checkoutService;

    public CheckoutController(ICheckoutService checkoutService)
    {
        _checkoutService = checkoutService;
    }

    // GET: api/checkout/preview
    [HttpGet("preview")]
    public async Task<IActionResult> PreviewAsync(
        CancellationToken cancellationToken = default)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var result = await _checkoutService.PreviewAsync(
            userId,
            cancellationToken);

        return Ok(result);
    }

    // POST: api/checkout/orders
    [HttpPost("orders")]
    public async Task<IActionResult> CreateOrderFromCartAsync(
        [FromBody] CreateOrderFromCartRequestModel request,
        CancellationToken cancellationToken = default)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var result = await _checkoutService.CreateOrderFromCartAsync(
            userId,
            request,
            cancellationToken);

        return Ok(result);
    }

    // POST: api/checkout/orders/{orderId}/confirm-payment
    // Later: this can be called by VNPay / PayOS callback
    [HttpPost("orders/{orderId:long}/confirm-payment")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmPaymentAsync(
        long orderId,
        [FromQuery] bool isSuccess,
        CancellationToken cancellationToken = default)
    {
        var result = await _checkoutService.ConfirmPaymentAsync(
            orderId,
            isSuccess,
            cancellationToken);

        return Ok(result);
    }

    // POST: api/checkout/orders/{orderId}/confirm-cod
    [HttpPost("orders/{orderId:long}/confirm-cod")]
    [Authorize(Policy = "AdminOrStaff")]
    public async Task<IActionResult> ConfirmCodOrderAsync(
        long orderId,
        CancellationToken cancellationToken = default)
    {
        var result = await _checkoutService.ConfirmCodOrderAsync(
            orderId,
            cancellationToken);

        return Ok(result);
    }

    private bool TryGetUserId(out long userId)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return long.TryParse(userIdClaim, out userId);
    }
}