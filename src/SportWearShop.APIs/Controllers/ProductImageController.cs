using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportWearShop.BusinessLogics.Interfaces;
using SportWearShop.BusinessLogics.ResponseModels.ProductModels.ProductImageModels;

namespace SportWearShop.APIs.Controllers;

[Route("api")]
[ApiController]
public class ProductImagesController : ControllerBase
{
    private readonly IProductImageService _productImageService;

    public ProductImagesController(
        IProductImageService productImageService)
    {
        _productImageService = productImageService;
    }

    // GET: api/product/1/product-images
    // AUTHORIZATION: Allow anonymous, client, admin, staff
    [HttpGet("product/{productId:long}/product-images")]
    public async Task<IActionResult> GetByProductIdAsync(
        [FromRoute] long productId,
        CancellationToken cancellationToken = default)
    {
        var result = await _productImageService.GetByProductIdAsync(
            productId,
            cancellationToken);

        return Ok(result);
    }

    // GET: api/variant/1/product-images
    // AUTHORIZATION: Allow anonymous, client, admin, staff
    [HttpGet("variant/{productVariantId:long}/product-images")]
    public async Task<IActionResult> GetByVariantIdAsync(
        [FromRoute] long productVariantId,
        CancellationToken cancellationToken = default)
    {
        var result = await _productImageService.GetByVariantIdAsync(
            productVariantId,
            cancellationToken);

        return Ok(result);
    }

    // POST: api/product-images
    // AUTHORIZATION: Admin, Staff
    [Authorize(Policy = "AdminOrStaff")]
    [HttpPost("product-images")]
    public async Task<IActionResult> CreateAsync(
        [FromForm] CreateProductImageRequestModel request,
        CancellationToken cancellationToken = default)
    {
        var result = await _productImageService.CreateAsync(
            request,
            cancellationToken);

        return Ok(result);
    }

    // PUT: api/product-images/1
    // AUTHORIZATION: Admin, Staff
    [Authorize(Policy = "AdminOrStaff")]
    [HttpPut("product-images/{productImageId:long}")]
    public async Task<IActionResult> UpdateAsync(
        [FromRoute] long productImageId,
        [FromForm] UpdateProductImageRequestModel request,
        CancellationToken cancellationToken = default)
    {
        var result = await _productImageService.UpdateAsync(
            productImageId,
            request,
            cancellationToken);

        return Ok(result);
    }

    // PATCH: api/product-images/1/set-primary
    // AUTHORIZATION: Admin, Staff
    [Authorize(Policy = "AdminOrStaff")]
    [HttpPatch("product-images/{productImageId:long}/set-primary")]
    public async Task<IActionResult> SetPrimaryAsync(
        [FromRoute] long productImageId,
        CancellationToken cancellationToken = default)
    {
        await _productImageService.SetPrimaryAsync(
            productImageId,
            cancellationToken);

        return NoContent();
    }

    // DELETE: api/product-images/1
    // AUTHORIZATION: Admin
    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("product-images/{productImageId:long}")]
    public async Task<IActionResult> DeleteAsync(
        [FromRoute] long productImageId,
        CancellationToken cancellationToken = default)
    {
        await _productImageService.DeleteAsync(
            productImageId,
            cancellationToken);

        return NoContent();
    }
}