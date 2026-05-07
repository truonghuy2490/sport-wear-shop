using global::SportWearShop.BusinessLogics.Interfaces;
using global::SportWearShop.BusinessLogics.ResponseModels.BrandModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SportWearShop.APIs.Controllers;

[Route("api/brands")]
[ApiController]
public class BrandController : ControllerBase
{
    private readonly IBrandService _brandService;

    public BrandController(IBrandService brandService)
    {
        _brandService = brandService;
    }

    // GET: api/brands
    // AUTHORIZATION: Allow anonymous, client, admin, staff
    [HttpGet]
    public async Task<IActionResult> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var result = await _brandService.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    // GET: api/brands/1
    // AUTHORIZATION: Allow anonymous, client, admin, staff
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _brandService.GetByIdAsync(id, cancellationToken);
        return Ok(result);
    }

    // POST: api/brands
    // AUTHORIZATION: Allow admin
    [Authorize(Policy = "AdminOnly")]
    [HttpPost]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreateBrandRequestModel request,
        CancellationToken cancellationToken)
    {
        var result = await _brandService.CreateAsync(request, cancellationToken);

        return CreatedAtAction(
            nameof(GetByIdAsync),
            new { id = result.BrandId },
            result);
    }

    // PUT: api/brands/1
    // AUTHORIZATION: Allow admin, staff
    [Authorize(Policy = "AdminOrStaff")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateAsync(
        int id,
        [FromBody] UpdateBrandRequestModel request,
        CancellationToken cancellationToken)
    {
        var result = await _brandService.UpdateAsync(id, request, cancellationToken);
        return Ok(result);
    }

    // DELETE: api/brands/1
    // AUTHORIZATION: Allow admin
    
    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync(
        int id,
        CancellationToken cancellationToken)
    {
        await _brandService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}