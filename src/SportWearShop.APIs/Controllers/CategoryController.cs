using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportWearShop.BusinessLogics.Interfaces;
using SportWearShop.BusinessLogics.ResponseModels.CategoryModels;

namespace SportWearShop.APIs.Controllers;

[Route("api/categories")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(
        ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    // GET: api/categories
    // AUTHORIZATION: Allow anonymous, client, admin, staff
    [HttpGet]
    public async Task<IActionResult> GetAllAsync(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _categoryService.GetAllAsync(
            pageNumber,
            pageSize,
            cancellationToken);

        return Ok(result);
    }

    // GET: api/categories/root
    // AUTHORIZATION: Allow anonymous, client, admin, staff
    [HttpGet("root")]
    public async Task<IActionResult> GetRootCategories(
        CancellationToken cancellationToken)
    {
        var result = await _categoryService.GetRootCategoriesAsync(cancellationToken);
        return Ok(result);
    }

    // GET: api/categories/1
    // AUTHORIZATION: Allow anonymous, client, admin, staff
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _categoryService.GetByIdAsync(id, cancellationToken);
        return Ok(result);
    }

    // GET: api/categories/1/children
    // AUTHORIZATION: Allow anonymous, client, admin, staff
    [HttpGet("{id:int}/children")]
    public async Task<IActionResult> GetChildren(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _categoryService.GetChildrenAsync(id, cancellationToken);
        return Ok(result);
    }

    // POST: api/categories
    // AUTHORIZATION: Allow admin
    [Authorize(Policy = "AdminOnly")]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateCategoryRequestModel request,
        CancellationToken cancellationToken)
    {
        var result = await _categoryService.CreateAsync(request, cancellationToken);

        return Ok(result);
    }

    // PUT: api/categories
    // AUTHORIZATION: Allow admin, staff
    [Authorize(Policy = "AdminOrStaff")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateCategoryRequestModel request,
        CancellationToken cancellationToken)
    {
        var result = await _categoryService.UpdateAsync(id, request, cancellationToken);
        return Ok(result);
    }

    // DELETE: api/categories
    // AUTHORIZATION: Allow admin
    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        await _categoryService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpGet("tree")]
    public async Task<IActionResult> GetTreeAsync(
        CancellationToken cancellationToken = default)
    {
        var result = await _categoryService.GetTreeAsync(cancellationToken);

        return Ok(result);
    }
}