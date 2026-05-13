using System;
using System.Collections.Generic;
using System.Text;
using SportWearShop.BusinessLogics.ResponseModels;
using SportWearShop.BusinessLogics.ResponseModels.InventoryModels;

namespace SportWearShop.BusinessLogics.Interfaces;

/*
Add to cart      => chưa đụng InventoryStock 
Create order     => reserve stock            | status = Reserve
Payment success  => release stock            | status = Sold
Cancel/Fail      => release reserved stock   | status = Release
*/
public interface IInventoryService
{
    Task<InventoryStockResponseModel> GetStockByVariantIdAsync(
        long productVariantId,
        CancellationToken cancellationToken = default);

    Task<PagingResponseModel<InventoryMovementResponseModel>> GetMovementsByVariantIdAsync(
        long productVariantId,
        InventoryMovementQueryRequestModel request,
        CancellationToken cancellationToken = default);


    Task<InventoryStockResponseModel> StockInAsync(
        StockInRequestModel request,
        CancellationToken cancellationToken = default);

    Task<InventoryStockResponseModel> StockOutAsync(
        StockOutRequestModel request,
        CancellationToken cancellationToken = default);

    // next update: bring this to repository layer

    // Internal System / Checkout Flow
    Task<InventoryStockResponseModel> ReserveAsync(
        ReserveStockRequestModel request,
        CancellationToken cancellationToken = default);

    // Internal System / Order Cancel Flow
    Task<InventoryStockResponseModel> ReleaseAsync(
        ReleaseStockRequestModel request,
        CancellationToken cancellationToken = default);

    // Internal System / Payment Success Flow    
    Task<InventoryStockResponseModel> SoldAsync(
        SoldStockRequestModel request,
        CancellationToken cancellationToken = default);
}