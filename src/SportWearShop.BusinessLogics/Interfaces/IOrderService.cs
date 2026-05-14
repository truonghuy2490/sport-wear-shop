using System;
using System.Collections.Generic;
using System.Text;
using SportWearShop.BusinessLogics.ResponseModels;
using SportWearShop.BusinessLogics.ResponseModels.OrderModels;

namespace SportWearShop.BusinessLogics.Interfaces;

public interface IOrderService
{
    Task<PagingResponseModel<OrderSummaryResponseModel>> GetMyOrdersAsync(
        long userId,
        OrderQueryRequestModel request,
        CancellationToken cancellationToken = default);

    Task<OrderDetailResponseModel> GetMyOrderDetailAsync(
        long userId,
        long orderId,
        CancellationToken cancellationToken = default);

    Task<PagingResponseModel<OrderSummaryResponseModel>> GetAllForAdminAsync(
        OrderQueryRequestModel request,
        CancellationToken cancellationToken = default);

    Task<OrderDetailResponseModel> GetOrderDetailForAdminAsync(
        long orderId,
        CancellationToken cancellationToken = default);

    Task CancelOrderAsync(
        long userId,
        long orderId,
        CancellationToken cancellationToken = default);
}