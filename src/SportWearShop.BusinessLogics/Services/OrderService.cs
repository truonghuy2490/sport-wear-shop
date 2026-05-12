using System.Linq.Expressions;
using LinqKit;
using Microsoft.Extensions.Logging;
using SportWearShop.BusinessLogics.Exceptions;
using SportWearShop.BusinessLogics.Interfaces;
using SportWearShop.BusinessLogics.ResponseModels;
using SportWearShop.BusinessLogics.ResponseModels.OrderModels;
using SportWearShop.Repositories.Entities;
using SportWearShop.Repositories.Enums;
using SportWearShop.Repositories.Implementations;
using SportWearShop.Repositories.UnitOfWorks;

namespace SportWearShop.BusinessLogics.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        IUnitOfWork unitOfWork,
        ILogger<OrderService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<PagingResponseModel<OrderSummaryResponseModel>> GetMyOrdersAsync(
        long userId,
        OrderQueryRequestModel request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        _logger.LogInformation(
            "Retrieving orders for user {UserId}",
            userId);
        

         var filter = PredicateBuilder.New<OrderHeader>(
                order => order.UserId == userId);

        var options = new QueryOptions<OrderHeader>
        {
            Filter = filter,
            SortBy = GetSortExpression(request.SortBy),
            Ascending = request.IsAscending,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            AsNoTracking = true
        };

        var result = await _unitOfWork.OrderHeaders.FindWithPagingAsync(
            options,
            selector: order => new OrderSummaryResponseModel
            {
                OrderId = order.OrderId,
                OrderNumber = order.OrderNumber,
                TotalAmount = order.TotalAmount,
                OrderStatus = order.OrderStatus,
                PaymentStatus = order.PaymentStatus,
                OrderedAtUtc = order.OrderedAtUtc,
                TotalItems = order.OrderItems.Count
            },
            cancellationToken);

        return new PagingResponseModel<OrderSummaryResponseModel>(
            result.Items,
            result.TotalCount,
            request.PageNumber,
            request.PageSize);
    }

    public async Task<OrderDetailResponseModel> GetMyOrderDetailAsync(
        long userId,
        long orderId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Retrieving order detail. UserId={UserId}, OrderId={OrderId}",
            userId,
            orderId);

        var order = await _unitOfWork.OrderHeaders.FirstOrDefaultAsync(
            predicate: order => order.OrderId == orderId
                                && order.UserId == userId,
            selector: MapOrderDetail(),
            asNoTracking: true,
            cancellationToken: cancellationToken,
            includes:
            [
                order => order.OrderItems,
                order => order.PaymentTransactions
            ]);

        if (order == null)
        {
            throw new NotFoundException("Order not found.");
        }

        return order;
    }

    public async Task<PagingResponseModel<OrderSummaryResponseModel>> GetAllForAdminAsync(
        OrderQueryRequestModel request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        Expression<Func<OrderHeader, bool>> filter = PredicateBuilder.New<OrderHeader>(true);

        var options = new QueryOptions<OrderHeader>
        {
            Filter = filter,
            SortBy = GetSortExpression(request.SortBy),
            Ascending = request.IsAscending,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            AsNoTracking = true
        };

        var result = await _unitOfWork.OrderHeaders.FindWithPagingAsync(
            options,
            selector: order => new OrderSummaryResponseModel
            {
                OrderId = order.OrderId,
                OrderNumber = order.OrderNumber,
                TotalAmount = order.TotalAmount,
                OrderStatus = order.OrderStatus,
                PaymentStatus = order.PaymentStatus,
                OrderedAtUtc = order.OrderedAtUtc,
                TotalItems = order.OrderItems.Count
            },
            cancellationToken);

        return new PagingResponseModel<OrderSummaryResponseModel>(
            result.Items,
            result.TotalCount,
            request.PageNumber,
            request.PageSize);
    }

    public async Task<OrderDetailResponseModel> GetOrderDetailForAdminAsync(
        long orderId,
        CancellationToken cancellationToken = default)
    {
        var order = await _unitOfWork.OrderHeaders.FirstOrDefaultAsync(
            predicate: order => order.OrderId == orderId,
            selector: MapOrderDetail(),
            asNoTracking: true,
            cancellationToken: cancellationToken,
            includes:
            [
                order => order.OrderItems,
                order => order.PaymentTransactions
            ]);

        if (order == null)
        {
            throw new NotFoundException("Order not found.");
        }

        return order;
    }

    public async Task CancelOrderAsync(
        long userId,
        long orderId,
        CancellationToken cancellationToken = default)
    {
        var order = await _unitOfWork.OrderHeaders.FirstOrDefaultAsync(
            predicate: order => order.OrderId == orderId
                                && order.UserId == userId,
            selector: order => order,
            asNoTracking: false,
            cancellationToken: cancellationToken);

        if (order == null)
        {
            throw new NotFoundException("Order not found.");
        }

        if (order.OrderStatus != "Pending")
        {
            throw new BadRequestException(
                "Only pending orders can be cancelled.");
        }

        order.OrderStatus = "Cancelled";
        order.PaymentStatus = "Cancelled";
        order.CancelledAtUtc = DateTime.UtcNow;
        order.UpdatedAtUtc = DateTime.UtcNow;

        _unitOfWork.OrderHeaders.Update(order);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static Expression<Func<OrderHeader, object>> GetSortExpression(
        OrderSortBy sortBy)
    {
        return sortBy switch
        {
            OrderSortBy.OrderNumber => order => order.OrderNumber,
            OrderSortBy.TotalAmount => order => order.TotalAmount,
            OrderSortBy.OrderStatus => order => order.OrderStatus,
            OrderSortBy.PaymentStatus => order => order.PaymentStatus,
            _ => order => order.OrderedAtUtc
        };
    }

    private static Expression<Func<OrderHeader, OrderDetailResponseModel>> MapOrderDetail()
    {
        return order => new OrderDetailResponseModel
        {
            OrderId = order.OrderId,
            OrderNumber = order.OrderNumber,
            ShippingAddressSnapshot = order.ShippingAddressSnapshot,
            TotalAmount = order.TotalAmount,
            OrderStatus = order.OrderStatus,
            PaymentStatus = order.PaymentStatus,
            OrderedAtUtc = order.OrderedAtUtc,

            Items = order.OrderItems.Select(item =>
                new OrderItemResponseModel
                {
                    SkuSnapshot = item.SkuSnapshot,
                    ProductNameSnapshot = item.ProductNameSnapshot,
                    ColorSnapshot = item.ColorSnapshot,
                    SizeSnapshot = item.SizeSnapshot,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    LineTotalAmount = item.LineTotalAmount
                }).ToList()
        };
    }
}