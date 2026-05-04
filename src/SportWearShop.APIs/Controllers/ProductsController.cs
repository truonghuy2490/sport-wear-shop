using Microsoft.AspNetCore.Mvc;
using SportWearShop.BusinessLogics.Interfaces;
using SportWearShop.BusinessLogics.ResponseModels.ProductModels;

namespace SportWearShop.APIs.Controllers;

[Route("api/products")]
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
    [HttpGet]
    public async Task<IActionResult> GetAllAsync(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _productService.GetAllAsync(
            pageNumber,
            pageSize,
            cancellationToken);

        return Ok(result);
    }

    // GET: api/products/1
    // AUTHORIZATION: Allow anonymous, client, admin, staff
    [HttpGet("{productId:long}")]
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
    [HttpPost]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreateProductRequestModel request,
        CancellationToken cancellationToken = default)
    {
        var result = await _productService.CreateAsync(
            request,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetDetailsAsync),
            new { productId = result.ProductId },
            result);
    }

    // PUT: api/products/1
    // AUTHORIZATION: Admin, Staff
    [HttpPut("{productId:long}")]
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
    [HttpDelete("{productId:long}")]
    public async Task<IActionResult> DeleteAsync(
        [FromRoute] long productId,
        CancellationToken cancellationToken = default)
    {
        await _productService.DeleteAsync(
            productId,
            cancellationToken);

        return NoContent();
    }
}