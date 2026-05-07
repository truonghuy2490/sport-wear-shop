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
    [HttpGet("api/products/{productId:long}/product-variants")]
    public async Task<IActionResult> GetByProductIdAsync(
        [FromRoute] long productId,
        CancellationToken cancellationToken = default)
    {
        var result = await _productVariantService.GetByProductIdAsync(
            productId,
            cancellationToken);

        return Ok(result);
    }

    // POST: /api/products/{productId}/product-variants
    // AUTHORIZATION: Admin, Staff
<<<<<<< HEAD
    [Authorize(Policy = "AdminOrStaff")]
=======
<<<<<<< HEAD
    [Authorize(Policy = "AdminOrStaff")]
=======
>>>>>>> 0f1984f89c4758af659b95b7677becfbc0e7f653
>>>>>>> b9a449bbf09be8444339b1e75284695aec3d8227
    [HttpPost("api/products/{productId:long}/product-variants")]
    public async Task<IActionResult> CreateAsync(
        [FromRoute] long productId, 
        [FromBody] CreateProductVariantRequestModel request,
        CancellationToken cancellationToken = default)
    {
        var result = await _productVariantService.CreateAsync(
            productId,
            request,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetByIdAsync),
            new { productVariantId = result.ProductVariantId },
            result);
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
<<<<<<< HEAD
    [Authorize(Policy = "AdminOrStaff")]
=======
<<<<<<< HEAD
    [Authorize(Policy = "AdminOrStaff")]
=======
>>>>>>> 0f1984f89c4758af659b95b7677becfbc0e7f653
>>>>>>> b9a449bbf09be8444339b1e75284695aec3d8227
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
<<<<<<< HEAD
    [Authorize(Policy = "AdminOnly")]
=======
<<<<<<< HEAD
    [Authorize(Policy = "AdminOnly")]
=======
>>>>>>> 0f1984f89c4758af659b95b7677becfbc0e7f653
>>>>>>> b9a449bbf09be8444339b1e75284695aec3d8227
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
<<<<<<< HEAD

    [HttpPut("{productVariantId:long}/images/sort-orders")]
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
=======
>>>>>>> b9a449bbf09be8444339b1e75284695aec3d8227
}