using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SportWearShop.BusinessLogics.Exceptions;
using SportWearShop.BusinessLogics.Interfaces;
using SportWearShop.BusinessLogics.ResponseModels;
using SportWearShop.BusinessLogics.ResponseModels.UserModels;
using SportWearShop.Repositories.Entities;
using SportWearShop.Repositories.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportWearShop.BusinessLogics.Services;

public class UserService : IUserService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UserService> _logger;

    public UserService(
        UserManager<AppUser> userManager,
        IUnitOfWork unitOfWOrk,
        ILogger<UserService> logger
    )
    {
        _userManager = userManager;
        _unitOfWork = unitOfWOrk;
        _logger = logger;
    }

    // me
    public async Task<UserProfileResponseModel> GetProfileAsync(
        long userId,
        CancellationToken cancellationToken = default
    )
    {
        var user = await _userManager.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);

        if(user == null)
        {
            throw new NotFoundException("User not found.");
        }

        return new UserProfileResponseModel
        {
            UserId = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            IsActive = user.IsActive,
            CreatedAtUtc = user.CreatedAtUtc
        };
    }
    // me
    public async Task UpdateProfileAsync(
        long userId,
        UpdateUserProfileRequestModel request,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(request);

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            throw new NotFoundException("User not found.");

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.PhoneNumber = request.PhoneNumber;
        user.UpdatedAtUtc = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
            throw new BadRequestException(result.Errors.First().Description);

        _logger.LogInformation("Updated user profile. UserId={UserId}", userId);
    }
    // me
    public async Task<UserAddressResponseModel> GetAddressByIdAsync(
        long userId,
        CancellationToken cancellationToken = default
    )
    {
        
        var address = await _unitOfWork.UserAddresses.FirstOrDefaultAsync(
            predicate: ad => ad.UserId == userId,
            selector: ad => new UserAddressResponseModel
            {
                UserAddressId = ad.UserAddressId,
                RecipientName = ad.RecipientName,
                PhoneNumber = ad.PhoneNumber,
                AddressLine1 = ad.AddressLine1,
                AddressLine2 = ad.AddressLine2,
                Ward = ad.Ward,
                District = ad.District,
                City = ad.City,
                Province = ad.Province,
                PostalCode = ad.PostalCode,
                CountryCode = ad.CountryCode,
                IsDefaultShipping = ad.IsDefaultShipping,
                IsDefaultBilling = ad.IsDefaultBilling
            },
            asNoTracking: true,
            cancellationToken: cancellationToken
        );

        if (address is null)
            throw new NotFoundException("User address not found.");

        return address;
    }


    public async Task CreateAddressAsync(
        long userId,
        CreateUserAddressRequestModel request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var userExists = await _userManager.Users.AnyAsync(
            u => u.Id == userId && u.IsActive,
            cancellationToken);

        if (!userExists)
            throw new NotFoundException("User not found.");

        var now = DateTime.UtcNow;

        var address = new UserAddress
        {
            UserId = userId,
            RecipientName = request.RecipientName.Trim(),
            PhoneNumber = request.PhoneNumber.Trim(),
            AddressLine1 = request.AddressLine1.Trim(),
            AddressLine2 = request.AddressLine2?.Trim(),
            Ward = request.Ward?.Trim(),
            District = request.District?.Trim(),
            City = request.City.Trim(),
            Province = request.Province?.Trim(),
            PostalCode = request.PostalCode?.Trim(),
            CountryCode = request.CountryCode.Trim().ToUpper(),
            IsDefaultShipping = request.IsDefaultShipping,
            IsDefaultBilling = request.IsDefaultBilling,
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };

        await _unitOfWork.UserAddresses.AddAsync(address, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Added user address. UserId={UserId}", userId);
    }

    public async Task UpdateAddressAsync(
        long userId,
        long userAddressId,
        UpdateUserAddressRequestModel request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var address = await _unitOfWork.UserAddresses.FirstOrDefaultAsync(
            predicate: x => x.UserAddressId == userAddressId && x.UserId == userId,
            selector: x => x,
            asNoTracking: false,
            cancellationToken: cancellationToken);

        if (address is null)
            throw new NotFoundException("User address not found.");

        address.RecipientName = request.RecipientName.Trim();
        address.PhoneNumber = request.PhoneNumber.Trim();
        address.AddressLine1 = request.AddressLine1.Trim();
        address.AddressLine2 = request.AddressLine2?.Trim();
        address.Ward = request.Ward?.Trim();
        address.District = request.District?.Trim();
        address.City = request.City.Trim();
        address.Province = request.Province?.Trim();
        address.PostalCode = request.PostalCode?.Trim();
        address.CountryCode = request.CountryCode.Trim().ToUpper();
        address.IsDefaultShipping = request.IsDefaultShipping;
        address.IsDefaultBilling = request.IsDefaultBilling;
        address.UpdatedAtUtc = DateTime.UtcNow;

        _unitOfWork.UserAddresses.Update(address);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Updated user address. UserId={UserId}, UserAddressId={UserAddressId}",
            userId,
            userAddressId);
    }

    public async Task DeleteAddressAsync(
        long userId,
        long userAddressId,
        CancellationToken cancellationToken = default)
    {
        var address = await _unitOfWork.UserAddresses.FirstOrDefaultAsync(
            predicate: x => x.UserAddressId == userAddressId && x.UserId == userId,
            selector: x => x,
            asNoTracking: false,
            cancellationToken: cancellationToken);

        if (address is null)
            throw new NotFoundException("User address not found.");

        _unitOfWork.UserAddresses.Remove(address);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Deleted user address. UserId={UserId}, UserAddressId={UserAddressId}",
            userId,
            userAddressId);
    }

    public async Task<PagingResponseModel<UserResponseModel>> GetUsersAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber <= 0) pageNumber = 1;
        if (pageSize <= 0) pageSize = 10;

        var query = _userManager.Users.AsNoTracking();

        var total = await query.CountAsync(cancellationToken);

        var users = await query
            .OrderByDescending(u => u.CreatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
            

        var result = new List<UserResponseModel>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            result.Add(new UserResponseModel
            {
                UserId = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                Roles = roles.ToList(),
                CreatedAtUtc = user.CreatedAtUtc
            });
        }

        return new PagingResponseModel<UserResponseModel>
        (
            result, 
            total, 
            pageNumber, 
            pageSize
        );
            
    }
    
    // admin
    public async Task<UserDetailResponseModel> GetUserDetailAsync(
        long userId,
        CancellationToken cancellationToken = default
    )
    {
        var user = await _userManager.Users
                .AsNoTracking()
                .FirstOrDefaultAsync( u => u.Id == userId, cancellationToken);
        if (user is null)
            throw new NotFoundException("User not found.");

        var roles = await _userManager.GetRolesAsync(user);
        
        var addresses = await _unitOfWork.UserAddresses.FindAsync(
            filter: ad => ad.UserId == userId,
            selector: ad => new UserAddressResponseModel
            {
                UserAddressId = ad.UserAddressId,
                RecipientName = ad.RecipientName,
                PhoneNumber = ad.PhoneNumber,
                AddressLine1 = ad.AddressLine1,
                AddressLine2 = ad.AddressLine2,
                Ward = ad.Ward,
                District = ad.District,
                City = ad.City,
                Province = ad.Province,
                PostalCode = ad.PostalCode,
                CountryCode = ad.CountryCode,
                IsDefaultShipping = ad.IsDefaultShipping,
                IsDefaultBilling = ad.IsDefaultBilling
            },
            sortBy: ad => ad.CreatedAtUtc,
            ascending: false,
            asNoTracking: true,
            cancellationToken: cancellationToken);

        return new UserDetailResponseModel
        {
            UserId = user.Id,
            Email = user.Email!,
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            EmailConfirmed = user.EmailConfirmed,
            PhoneNumberConfirmed = user.PhoneNumberConfirmed,
            IsActive = user.IsActive,
            Roles = roles.ToList(),
            CreatedAtUtc = user.CreatedAtUtc,
            UpdatedAtUtc = user.UpdatedAtUtc,
            Addresses = addresses
        };
    }

    public async Task ActivateUserAsync(
        long userId,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
            throw new NotFoundException("User not found.");

        user.IsActive = true;
        user.UpdatedAtUtc = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
            throw new BadRequestException(result.Errors.First().Description);

        _logger.LogInformation("Activated user. UserId={UserId}", userId);
    }

    public async Task DeactivateUserAsync(
        long userId,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
            throw new NotFoundException("User not found.");

        user.IsActive = false;
        user.UpdatedAtUtc = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
            throw new BadRequestException(result.Errors.First().Description);

        _logger.LogInformation("Deactivated user. UserId={UserId}", userId);
    }
}