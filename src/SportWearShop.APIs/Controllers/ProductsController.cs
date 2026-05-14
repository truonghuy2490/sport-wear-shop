using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportWearShop.BusinessLogics.Interfaces;
using SportWearShop.BusinessLogics.ResponseModels.ProductModels;

namespace SportWearShop.APIs.Controllers;

[Route("api")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(
        IProductService productService)
    {
        _productService = productService;
    }

    // GET: api/products
    // AUTHORIZATION: Allow anonymous, client, admin, staff
    [HttpGet("products")]
    public async Task<IActionResult> GetAllAsync(
        [FromQuery] ProductQueryRequestModel request,
        CancellationToken cancellationToken = default)
    {
        var result = await _productService.GetAllAsync(
            request,
            cancellationToken);

        return Ok(result);
    }

    // GET: api/products/1
    // AUTHORIZATION: Allow anonymous, client, admin, staff
    [HttpGet("products/{productId:long}")]
    public async Task<IActionResult> GetDetailsAsync(
        [FromRoute] long productId,
        CancellationToken cancellationToken = default)
    {
        var result = await _productService.GetDetailsAsync(
            productId,
            cancellationToken);

        return Ok(result);
    }

    // POST: api/products
    // AUTHORIZATION: Admin, Staff
    [Authorize(Policy = "AdminOrStaff")]
    [HttpPost("products")]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreateProductRequestModel request,
        CancellationToken cancellationToken = default)
    {
        var result = await _productService.CreateAsync(
            request,
            cancellationToken);

        return Ok(result);
    }

    // PUT: api/products/1
    // AUTHORIZATION: Admin, Staff
    [Authorize(Policy = "AdminOrStaff")]
    [HttpPut("products/{productId:long}")]
    public async Task<IActionResult> UpdateAsync(
        [FromRoute] long productId,
        [FromBody] UpdateProductRequestModel request,
        CancellationToken cancellationToken = default)
    {
        var result = await _productService.UpdateAsync(
            productId,
            request,
            cancellationToken);

        return Ok(result);
    }
    

    // DELETE: api/products/1
    // AUTHORIZATION: Admin
    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("products/{productId:long}")]
    public async Task<IActionResult> DeleteAsync(
        [FromRoute] long productId,
        CancellationToken cancellationToken = default)
    {
        await _productService.DeleteAsync(
            productId,
            cancellationToken);

        return NoContent();
    }

    [HttpGet("admin/products/{productId:long}")]
    
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetAdminDetailsAsync(
        long productId,
        CancellationToken cancellationToken)
    {
        var result = await _productService.GetAdminDetailsAsync(
            productId,
            cancellationToken);

        return Ok(result);
    }

    [HttpPatch("products/{productId:long}/status")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> UpdateStatus(
        long productId,
        [FromBody] UpdateProductStatusRequestModel request,
        CancellationToken cancellationToken)
    {
        var result = await _productService.UpdateStatusAsync(
            productId,
            request.Status,
            cancellationToken);

        return Ok(result);
    }
}