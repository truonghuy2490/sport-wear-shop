using SportWearShop.BusinessLogics.ResponseModels.BrandModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportWearShop.BusinessLogics.Interfaces;

public interface IBrandService
{
    Task<List<BrandResponseModel>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<BrandDetailResponseModel?> GetByIdAsync(
        int brandId,
        CancellationToken cancellationToken = default);

    Task<BrandResponseModel> CreateAsync(
        CreateBrandRequestModel request,
        CancellationToken cancellationToken = default);

    Task<BrandResponseModel> UpdateAsync(
        int brandId,
        UpdateBrandRequestModel request,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        int brandId,
        CancellationToken cancellationToken = default);

    
}