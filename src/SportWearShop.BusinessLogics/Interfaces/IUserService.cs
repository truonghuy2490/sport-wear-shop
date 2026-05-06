using System;
using System.Collections.Generic;
using System.Text;
using SportWearShop.BusinessLogics.ResponseModels;
using SportWearShop.BusinessLogics.ResponseModels.UserModels;

namespace SportWearShop.BusinessLogics.Interfaces;

public interface IUserService
{
    // personalize
    Task<UserProfileResponseModel> GetProfileAsync(
        long userId,
        CancellationToken cancellationToken = default);

    Task UpdateProfileAsync(
        long userId,
        UpdateUserProfileRequestModel request,
        CancellationToken cancellationToken = default);

        Task<UserAddressResponseModel> GetAddressByIdAsync(
        long userId,
        CancellationToken cancellationToken = default);

    Task CreateAddressAsync(
        long userId,
        CreateUserAddressRequestModel request,
        CancellationToken cancellationToken = default);

    Task UpdateAddressAsync(
        long userId,
        long userAddressId,
        UpdateUserAddressRequestModel request,
        CancellationToken cancellationToken = default);

    Task DeleteAddressAsync(
        long userId,
        long userAddressId,
        CancellationToken cancellationToken = default);

    // admin 
    Task<PagingResponseModel<UserResponseModel>> GetUsersAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<UserDetailResponseModel> GetUserDetailAsync(
        long userId,
        CancellationToken cancellationToken = default);

    Task ActivateUserAsync(
        long userId,
        CancellationToken cancellationToken = default);

    Task DeactivateUserAsync(
        long userId,
        CancellationToken cancellationToken = default);
}