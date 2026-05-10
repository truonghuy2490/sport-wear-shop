<<<<<<< HEAD
using Microsoft.AspNetCore.Authorization;
=======
<<<<<<< HEAD
using Microsoft.AspNetCore.Authorization;
=======
>>>>>>> 0f1984f89c4758af659b95b7677becfbc0e7f653
>>>>>>> b9a449bbf09be8444339b1e75284695aec3d8227
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
<<<<<<< HEAD
    [Authorize(Policy = "AdminOrStaff")]
    [HttpPost("product-images")]
    public async Task<IActionResult> CreateAsync(
        [FromForm] CreateProductImageRequestModel request,
=======
<<<<<<< HEAD
    [Authorize(Policy = "AdminOrStaff")]
=======
>>>>>>> 0f1984f89c4758af659b95b7677becfbc0e7f653
    [HttpPost("product-images")]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreateProductImageRequestModel request,
>>>>>>> b9a449bbf09be8444339b1e75284695aec3d8227
        CancellationToken cancellationToken = default)
    {
        var result = await _productImageService.CreateAsync(
            request,
            cancellationToken);

<<<<<<< HEAD
        return Ok(result);
=======
        return CreatedAtAction(
            nameof(GetByProductIdAsync),
            new { productId = request.ProductId },
            result);
>>>>>>> b9a449bbf09be8444339b1e75284695aec3d8227
    }

    // PUT: api/product-images/1
    // AUTHORIZATION: Admin, Staff
<<<<<<< HEAD
    [Authorize(Policy = "AdminOrStaff")]
    [HttpPut("product-images/{productImageId:long}")]
    public async Task<IActionResult> UpdateAsync(
        [FromRoute] long productImageId,
        [FromForm] UpdateProductImageRequestModel request,
=======
<<<<<<< HEAD
    [Authorize(Policy = "AdminOrStaff")]
=======
>>>>>>> 0f1984f89c4758af659b95b7677becfbc0e7f653
    [HttpPut("product-images/{productImageId:long}")]
    public async Task<IActionResult> UpdateAsync(
        [FromRoute] long productImageId,
        [FromBody] UpdateProductImageRequestModel request,
>>>>>>> b9a449bbf09be8444339b1e75284695aec3d8227
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
<<<<<<< HEAD
    [Authorize(Policy = "AdminOrStaff")]
=======
<<<<<<< HEAD
    [Authorize(Policy = "AdminOrStaff")]
=======
>>>>>>> 0f1984f89c4758af659b95b7677becfbc0e7f653
>>>>>>> b9a449bbf09be8444339b1e75284695aec3d8227
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
<<<<<<< HEAD
    [Authorize(Policy = "AdminOnly")]
=======
<<<<<<< HEAD
    [Authorize(Policy = "AdminOnly")]
=======
>>>>>>> 0f1984f89c4758af659b95b7677becfbc0e7f653
>>>>>>> b9a449bbf09be8444339b1e75284695aec3d8227
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