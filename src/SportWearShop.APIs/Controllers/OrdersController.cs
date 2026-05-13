using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportWearShop.BusinessLogics.Interfaces;
using SportWearShop.BusinessLogics.ResponseModels.OrderModels;

namespace SportWearShop.APIs.Controllers;

[Route("api/orders")]
[ApiController]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    // GET: api/orders/my-orders
    [HttpGet("my-orders")]
    public async Task<IActionResult> GetMyOrdersAsync(
        [FromQuery] OrderQueryRequestModel request,
        CancellationToken cancellationToken = default)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var result = await _orderService.GetMyOrdersAsync(
            userId,
            request,
            cancellationToken);

        return Ok(result);
    }

    // GET: api/orders/my-orders/{orderId}
    [HttpGet("my-orders/{orderId:long}")]
    public async Task<IActionResult> GetMyOrderDetailAsync(
        long orderId,
        CancellationToken cancellationToken = default)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var result = await _orderService.GetMyOrderDetailAsync(
            userId,
            orderId,
            cancellationToken);

        return Ok(result);
    }

    // PUT: api/orders/my-orders/{orderId}/cancel
    [HttpPut("my-orders/{orderId:long}/cancel")]
    public async Task<IActionResult> CancelOrderAsync(
        long orderId,
        CancellationToken cancellationToken = default)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        await _orderService.CancelOrderAsync(
            userId,
            orderId,
            cancellationToken);

        return NoContent();
    }

    // GET: api/orders/admin
    [HttpGet("admin")]
    [Authorize(Policy = "AdminOrStaff")]
    public async Task<IActionResult> GetAllForAdminAsync(
        [FromQuery] OrderQueryRequestModel request,
        CancellationToken cancellationToken = default)
    {
        var result = await _orderService.GetAllForAdminAsync(
            request,
            cancellationToken);

        return Ok(result);
    }

    // GET: api/orders/admin/{orderId}
    [HttpGet("admin/{orderId:long}")]
    [Authorize(Policy = "AdminOrStaff")]
    public async Task<IActionResult> GetOrderDetailForAdminAsync(
        long orderId,
        CancellationToken cancellationToken = default)
    {
        var result = await _orderService.GetOrderDetailForAdminAsync(
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