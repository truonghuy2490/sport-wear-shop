using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SportWearShop.Application.UseCases.Brands.ViewBrands;
using SportWearShop.Application.Utilities;
using SportWearShop.WebAPI.Utilities.Extensions;
using Swashbuckle.AspNetCore.Annotations;

namespace SportWearShop.WebAPI.Controllers.Brands;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/brands")]
public sealed class ViewBrandsController(ISender sender) : BaseApiController(sender)
{
    [HttpGet]
    [ProducesResponseType<PagedList<Response>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(Tags = [ControllerTags.Brands], Summary = "List brands with filtering, sorting, and pagination.")]
    public async Task<IActionResult> Get([FromQuery] Request request, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(request, cancellationToken);
        return result.Match<IActionResult>(Ok, errors => errors.ToProblem(HttpContext));
    }
}
