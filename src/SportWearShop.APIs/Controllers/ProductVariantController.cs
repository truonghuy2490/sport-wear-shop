using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportWearShop.BusinessLogics.Interfaces;
using SportWearShop.BusinessLogics.ResponseModels.ProductModels.ProductVarientModels;

namespace SportWearShop.APIs.Controllers;
[Route("")]
[ApiController]
public class ProductVariantsController : ControllerBase
{
    private readonly IProductVariantService _productVariantService;

    public ProductVariantsController(
        IProductVariantService productVariantService)
    {
        _productVariantService = productVariantService;
    }

    // GET /api/products/{productId}/variants
    // AUTHORIZATION: Allow anonymous, client, admin, staff
    [HttpGet("products/{productId:long}/variants")]
    public async Task<IActionResult> GetByProductIdAsync(
        long productId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _productVariantService.GetByProductIdAsync(
            productId,
            pageNumber,
            pageSize,
            cancellationToken);

        return Ok(result);
    }

    // POST: /api/products/{productId}/product-variants
    // AUTHORIZATION: Admin, Staff
    [Authorize(Policy = "AdminOrStaff")]
    [HttpPost("api/products/{productId:long}/product-variants/batch")]
    public async Task<IActionResult> CreateManyAsync(
        [FromRoute] long productId,
        [FromBody] CreateProductVariantsRequestModel request,
        CancellationToken cancellationToken = default)
    {
        var result = await _productVariantService.CreateManyAsync(
            productId,
            request,
            cancellationToken);

        return Ok(result);
    }

    // GET /api/product-variants/{productVariantId}
    // AUTHORIZATION: Allow anonymous, client, admin, staff
    [HttpGet("api/product-variants/{productVariantId:long}")]
    public async Task<IActionResult> GetByIdAsync(
        [FromRoute] long productVariantId,
        CancellationToken cancellationToken = default)
    {
        var result = await _productVariantService.GetByIdAsync(
            productVariantId,
            cancellationToken);

        return Ok(result);
    }

    

    // PUT: api/product-variants/1
    // AUTHORIZATION: Admin, Staff
    [Authorize(Policy = "AdminOrStaff")]
    [HttpPut("api/product-variants/{productVariantId:long}")]
    public async Task<IActionResult> UpdateAsync(
        [FromRoute] long productVariantId,
        [FromBody] UpdateProductVariantRequestModel request,
        CancellationToken cancellationToken = default)
    {
        var result = await _productVariantService.UpdateAsync(
            productVariantId,
            request,
            cancellationToken);

        return Ok(result);
    }

    

    // DELETE: api/product-variants/1
    // AUTHORIZATION: Admin
    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("api/product-variants/{productVariantId:long}")]
    public async Task<IActionResult> DeleteAsync(
        [FromRoute] long productVariantId,
        CancellationToken cancellationToken = default)
    {
        await _productVariantService.DeleteAsync(
            productVariantId,
            cancellationToken);

        return NoContent();
    }

    [HttpPut("api/product-variants/{productVariantId:long}/images/sort-orders")]
    [Authorize(Policy = "AdminOrStaff")]
    public async Task<IActionResult> UpdateImageSortOrdersAsync(
        [FromRoute] long productVariantId,
        [FromBody] UpdateProductImageSortOrdersRequestModel request,
        CancellationToken cancellationToken)
    {
        var result = await _productVariantService.UpdateSortOrdersAsync(
            productVariantId,
            request,
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("api/admin/product-variants/{productVariantId:long}")]
    [Authorize(Policy = "AdminOrStaff")]
    public async Task<IActionResult> GetAdminByIdAsync(
        long productVariantId,
        CancellationToken cancellationToken)
    {
        var result = await _productVariantService.GetAdminByIdAsync(
            productVariantId,
            cancellationToken);

        return Ok(result);
    }
}