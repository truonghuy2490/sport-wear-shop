using Microsoft.AspNetCore.Mvc;
using SportWearShop.Application.Features.Brands.GetBrands;

namespace SportWearShop.WebAPI.Controllers;

[ApiController]
[Route("api/brands")]
public sealed class BrandsController(GetBrandsHandler handler) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<BrandDto>>> Get(
        [FromQuery] bool? isActive, CancellationToken cancellationToken)
        => Ok(await handler.HandleAsync(isActive, cancellationToken));
}
