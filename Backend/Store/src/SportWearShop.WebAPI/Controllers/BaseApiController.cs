using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SportWearShop.WebAPI.Controllers;

[ApiController]
public abstract class BaseApiController(ISender sender) : ControllerBase
{
    protected ISender Sender { get; } = sender;
}
