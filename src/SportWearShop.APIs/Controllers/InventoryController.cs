using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportWearShop.BusinessLogics.Interfaces;
using SportWearShop.BusinessLogics.ResponseModels.InventoryModels;

namespace SportWearShop.APIs.Controllers;

[Route("api/inventory")]
[ApiController]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(
        IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    // GET: api/inventory/{productVariantId}/stock
    // AUTHORIZATION: Allow anonymous, client, admin, staff
    [HttpGet("{productVariantId:long}/stock")]
    public async Task<IActionResult> GetStockByVariantIdAsync(
        [FromRoute] long productVariantId,
        CancellationToken cancellationToken = default)
    {
        var result = await _inventoryService.GetStockByVariantIdAsync(
            productVariantId,
            cancellationToken);

        return Ok(result);
    }

    // GET: api/inventory/{productVariantId}/movements
    [Authorize(Policy = "AdminOrStaff")]
    // AUTHORIZATION: Admin, Staff
    [HttpGet("{productVariantId:long}/movements")]
    public async Task<IActionResult> GetMovementsByVariantIdAsync(
        [FromRoute] long productVariantId,
        CancellationToken cancellationToken = default)
    {
        var result = await _inventoryService.GetMovementsByVariantIdAsync(
            productVariantId,
            cancellationToken);

        return Ok(result);
    }

    // POST: api/inventory/stock-in
    // AUTHORIZATION: Admin, Staff
    [Authorize(Policy = "AdminOrStaff")]
    [HttpPost("stock-in")]
    public async Task<IActionResult> StockInAsync(
        [FromBody] StockInRequestModel request,
        CancellationToken cancellationToken = default)
    {
        var result = await _inventoryService.StockInAsync(
            request,
            cancellationToken);

        return Ok(result);
    }

    // POST: api/inventory/stock-out
    // AUTHORIZATION: Admin, Staff
    [Authorize(Policy = "AdminOrStaff")]
    [HttpPost("stock-out")]
    public async Task<IActionResult> StockOutAsync(
        [FromBody] StockOutRequestModel request,
        CancellationToken cancellationToken = default)
    {
        var result = await _inventoryService.StockOutAsync(
            request,
            cancellationToken);

        return Ok(result);
    }

    
}