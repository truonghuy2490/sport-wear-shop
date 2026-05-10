using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportWearShop.BusinessLogics.Interfaces;
using SportWearShop.BusinessLogics.ResponseModels.UserModels;

namespace SportWearShop.APIs.Controllers;

[Route("api/users")]
[ApiController]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    // GET: api/users/me
    [HttpGet("me")]
    public async Task<IActionResult> GetProfileAsync(
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();

        var result = await _userService.GetProfileAsync(
            userId,
            cancellationToken);

        return Ok(result);
    }

    // PUT: api/users/me
    [HttpPut("me")]
    public async Task<IActionResult> UpdateProfileAsync(
        [FromBody] UpdateUserProfileRequestModel request,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();

        await _userService.UpdateProfileAsync(
            userId,
            request,
            cancellationToken);

        return NoContent();
    }

    

    // POST: api/users/me/addresses
    [HttpPost("me/addresses")]
    public async Task<IActionResult> CreateAddressAsync(
        [FromBody] CreateUserAddressRequestModel request,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();

        await _userService.CreateAddressAsync(
            userId,
            request,
            cancellationToken);

        return NoContent();
    }

    // PUT: api/users/me/addresses/{userAddressId}
    [HttpPut("me/addresses/{userAddressId:long}")]
    public async Task<IActionResult> UpdateAddressAsync(
        [FromRoute] long userAddressId,
        [FromBody] UpdateUserAddressRequestModel request,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();

        await _userService.UpdateAddressAsync(
            userId,
            userAddressId,
            request,
            cancellationToken);

        return NoContent();
    }

    // DELETE: api/users/me/addresses/{userAddressId}
    [HttpDelete("me/addresses/{userAddressId:long}")]
    public async Task<IActionResult> DeleteAddressAsync(
        [FromRoute] long userAddressId,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();

        await _userService.DeleteAddressAsync(
            userId,
            userAddressId,
            cancellationToken);

        return NoContent();
    }

    // GET: api/users/{userId}/addresses/{userAddressId}
    [Authorize(Policy = "AdminOrStaff")]
    [HttpGet("{userId:long}/addresses")]
    public async Task<IActionResult> GetAddressByIdAsync(
        [FromRoute] long userId,
        CancellationToken cancellationToken = default)
    {
        var result = await _userService.GetAddressByIdAsync(
            userId,
            cancellationToken);

        return Ok(result);
    }

    // GET: api/users?pageNumber=1&pageSize=10
    [HttpGet]
    [Authorize(Policy = "AdminOrStaff")]
    public async Task<IActionResult> GetUsersAsync(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _userService.GetUsersAsync(
            pageNumber,
            pageSize,
            cancellationToken);

        return Ok(result);
    }

    // GET: api/users/1
    [HttpGet("{userId:long}")]
    [Authorize(Policy = "AdminOrStaff")]
    public async Task<IActionResult> GetUserDetailAsync(
        [FromRoute] long userId,
        CancellationToken cancellationToken = default)
    {
        var result = await _userService.GetUserDetailAsync(
            userId,
            cancellationToken);

        return Ok(result);
    }

    // PATCH: api/users/1/activate
    [HttpPatch("{userId:long}/activate")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> ActivateUserAsync(
        [FromRoute] long userId,
        CancellationToken cancellationToken = default)
    {
        await _userService.ActivateUserAsync(
            userId,
            cancellationToken);

        return NoContent();
    }

    // PATCH: api/users/1/deactivate
    [HttpPatch("{userId:long}/deactivate")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeactivateUserAsync(
        [FromRoute] long userId,
        CancellationToken cancellationToken = default)
    {
        await _userService.DeactivateUserAsync(
            userId,
            cancellationToken);

        return NoContent();
    }

    private long GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!long.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user token.");
        }

        return userId;
    }
}