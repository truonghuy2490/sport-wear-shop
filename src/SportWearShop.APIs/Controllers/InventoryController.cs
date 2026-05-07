<<<<<<< HEAD
using Microsoft.AspNetCore.Authorization;
=======
>>>>>>> 0f1984f89c4758af659b95b7677becfbc0e7f653
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
<<<<<<< HEAD
    [Authorize(Policy = "AdminOrStaff")]
=======
>>>>>>> 0f1984f89c4758af659b95b7677becfbc0e7f653
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
<<<<<<< HEAD
    [Authorize(Policy = "AdminOrStaff")]
=======
>>>>>>> 0f1984f89c4758af659b95b7677becfbc0e7f653
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
<<<<<<< HEAD
    [Authorize(Policy = "AdminOrStaff")]
=======
>>>>>>> 0f1984f89c4758af659b95b7677becfbc0e7f653
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