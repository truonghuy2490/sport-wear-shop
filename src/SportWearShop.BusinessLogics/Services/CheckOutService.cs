using Microsoft.Extensions.Logging;
using SportWearShop.BusinessLogics.Exceptions;
using SportWearShop.BusinessLogics.Interfaces;
using SportWearShop.BusinessLogics.ResponseModels.CheckOutModels;
using SportWearShop.Repositories.Entities;
using SportWearShop.Repositories.UnitOfWorks;

namespace SportWearShop.BusinessLogics.Services;

public class CheckoutService : ICheckoutService
{
    private const decimal DefaultShippingFee = 30000;
    private const string CurrencyCode = "VND";

    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CheckoutService> _logger;

    public CheckoutService(
        IUnitOfWork unitOfWork,
        ILogger<CheckoutService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<CheckoutPreviewResponseModel> PreviewAsync(
        long userId,
        CancellationToken cancellationToken = default)
    {
        var cart = await _unitOfWork.Carts.FirstOrDefaultAsync(
            predicate: cart => cart.UserId == userId,
            selector: cart => cart,
            asNoTracking: true,
            cancellationToken: cancellationToken,
            includes:
            [
                cart => cart.CartItems
            ]);

        if (cart == null || cart.CartItems.Count == 0)
        {
            throw new BadRequestException("Cart is empty.");
        }

        decimal subtotal = 0;
        var items = new List<CheckoutItemResponseModel>();

        foreach (var cartItem in cart.CartItems)
        {
            var variant = await _unitOfWork.ProductVariants.FirstOrDefaultAsync(
                predicate: variant => variant.ProductVariantId == cartItem.ProductVariantId,
                selector: variant => variant,
                asNoTracking: true,
                cancellationToken: cancellationToken,
                includes:
                [
                    variant => variant.Product,
                    variant => variant.InventoryStock
                ]);

            if (variant == null)
            {
                throw new NotFoundException("Product variant not found.");
            }

            if (variant.InventoryStock == null ||
                variant.InventoryStock.QuantityOnHand < cartItem.Quantity)
            {
                throw new BadRequestException(
                    $"Product {variant.Sku} does not have enough stock.");
            }

            var unitPrice = variant.SalePrice ?? variant.ListPrice;
            var lineTotalAmount = unitPrice * cartItem.Quantity;

            subtotal += lineTotalAmount;

            items.Add(new CheckoutItemResponseModel
            {
                ProductName = variant.Product.ProductName,
                Sku = variant.Sku,
                Quantity = cartItem.Quantity,
                UnitPrice = unitPrice,
                LineTotalAmount = lineTotalAmount
            });
        }

        var total = subtotal + DefaultShippingFee;

        return new CheckoutPreviewResponseModel
        {
            SubtotalAmount = subtotal,
            ShippingFee = DefaultShippingFee,
            TotalAmount = total,
            Items = items
        };
    }

    public async Task<CreateOrderResponseModel> CreateOrderFromCartAsync(
        long userId,
        CreateOrderFromCartRequestModel request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var now = DateTime.UtcNow;

        var shippingAddress = await _unitOfWork.UserAddresses.FirstOrDefaultAsync(
            predicate: address => address.UserAddressId == request.ShippingAddressId
                                && address.UserId == userId,
            selector: address => address,
            asNoTracking: true,
            cancellationToken: cancellationToken);

        if (shippingAddress == null)
        {
            throw new NotFoundException("Shipping address not found.");
        }

        var cart = await _unitOfWork.Carts.FirstOrDefaultAsync(
            predicate: cart => cart.UserId == userId,
            selector: cart => cart,
            asNoTracking: false,
            cancellationToken: cancellationToken,
            includes:
            [
                cart => cart.CartItems
            ]);

        if (cart == null || cart.CartItems.Count == 0)
        {
            throw new BadRequestException("Cart is empty.");
        }

        var orderItems = new List<OrderItem>();
        decimal subtotal = 0;

        foreach (var cartItem in cart.CartItems)
        {
            var variant = await _unitOfWork.ProductVariants.FirstOrDefaultAsync(
                predicate: variant => variant.ProductVariantId == cartItem.ProductVariantId,
                selector: variant => variant,
                asNoTracking: false,
                cancellationToken: cancellationToken,
                includes:
                [
                    variant => variant.Product,
                    variant => variant.InventoryStock,
                    variant => variant.ProductImages
                ]);

            if (variant == null)
            {
                throw new NotFoundException("Product variant not found.");
            }

            if (variant.InventoryStock == null ||
                variant.InventoryStock.QuantityOnHand < cartItem.Quantity)
            {
                throw new BadRequestException(
                    $"Product {variant.Sku} does not have enough stock.");
            }

            var unitPrice = variant.SalePrice ?? variant.ListPrice;
            var lineTotalAmount = unitPrice * cartItem.Quantity;

            subtotal += lineTotalAmount;

            variant.InventoryStock.QuantityOnHand -= cartItem.Quantity;
            variant.InventoryStock.UpdatedAtUtc = now;

            orderItems.Add(new OrderItem
            {
                ProductVariantId = variant.ProductVariantId,
                SkuSnapshot = variant.Sku,
                ProductNameSnapshot = variant.Product.ProductName,
                ColorSnapshot = variant.ColorName,
                SizeSnapshot = variant.SizeLabel,
                ImageUrlSnapshot = variant.ProductImages
                    .OrderBy(image => image.SortOrder)
                    .Select(image => image.ImageUrl)
                    .FirstOrDefault(),
                Quantity = cartItem.Quantity,
                UnitPrice = unitPrice,
                LineDiscountAmount = 0,
                LineTotalAmount = lineTotalAmount,
                CreatedAtUtc = now
            });

            _unitOfWork.InventoryStocks.Update(variant.InventoryStock);
        }

        var shippingFee = DefaultShippingFee;
        var discountAmount = 0;
        var totalAmount = subtotal + shippingFee - discountAmount;

        var order = new OrderHeader
        {
            OrderNumber = GenerateOrderNumber(),
            UserId = userId,

            ShippingAddressSnapshot = BuildAddressSnapshot(shippingAddress),
            BillingAddressSnapshot = null,

            OrderStatus = "Pending",
            PaymentStatus = request.PaymentMethod == "COD" ? "Unpaid" : "Pending",

            SubtotalAmount = subtotal,
            DiscountAmount = discountAmount,
            ShippingFee = shippingFee,
            TotalAmount = totalAmount,
            CurrencyCode = CurrencyCode,

            OrderedAtUtc = now,
            CreatedAtUtc = now,
            UpdatedAtUtc = now,

            OrderItems = orderItems
        };

        var paymentTransaction = new PaymentTransaction
        {
            Order = order,
            PaymentMethod = request.PaymentMethod,
            GatewayTransactionRef = null,
            Amount = totalAmount,
            TransactionStatus = "Pending",
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };

        order.PaymentTransactions.Add(paymentTransaction);

        await _unitOfWork.OrderHeaders.AddAsync(order, cancellationToken);

        foreach (var cartItem in cart.CartItems.ToList())
        {
            _unitOfWork.CartItems.Remove(cartItem);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Online Payment in next update
        string? paymentUrl = null;

        if (request.PaymentMethod != "COD")
        {
            // TODO: Generate payment URL from VNPay / PayOS
            paymentUrl = "https://payment-gateway-url";
        }

        return new CreateOrderResponseModel
        {       
            OrderId = order.OrderId,
            OrderNumber = order.OrderNumber,
            TotalAmount = order.TotalAmount,
            OrderStatus = order.OrderStatus,
            PaymentStatus = order.PaymentStatus,
            PaymentUrl = paymentUrl
        };
    }    
    
    public async Task<PaymentResultResponseModel> ConfirmPaymentAsync(
        long orderId,
        bool isSuccess,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var order = await _unitOfWork.OrderHeaders.FirstOrDefaultAsync(
            predicate: order => order.OrderId == orderId,
            selector: order => order,
            asNoTracking: false,
            cancellationToken: cancellationToken,
            includes:
            [
                order => order.PaymentTransactions
            ]);

        if (order == null)
        {
            throw new NotFoundException("Order not found.");
        }

        var paymentTransaction = order.PaymentTransactions
            .OrderByDescending(transaction => transaction.CreatedAtUtc)
            .FirstOrDefault();

        if (paymentTransaction == null)
        {
            throw new NotFoundException("Payment transaction not found.");
        }

        if (paymentTransaction.TransactionStatus == "Paid")
        {
            throw new BadRequestException("Payment already confirmed.");
        }

        if (isSuccess)
        {
            paymentTransaction.TransactionStatus = "Paid";
            paymentTransaction.PaidAtUtc = now;
            paymentTransaction.UpdatedAtUtc = now;

            order.PaymentStatus = "Paid";
            order.OrderStatus = "Confirmed";
            order.PaidAtUtc = now;
            order.UpdatedAtUtc = now;
        }
        else
        {
            paymentTransaction.TransactionStatus = "Failed";
            paymentTransaction.FailureReason = "Payment failed.";
            paymentTransaction.UpdatedAtUtc = now;

            order.PaymentStatus = "Failed";
            order.UpdatedAtUtc = now;
        }

        _unitOfWork.PaymentTransactions.Update(paymentTransaction);
        _unitOfWork.OrderHeaders.Update(order);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new PaymentResultResponseModel
        {
            OrderId = order.OrderId,
            IsSuccess = isSuccess,
            PaymentStatus = order.PaymentStatus,
            OrderStatus = order.OrderStatus
        };
    }
        private static string GenerateOrderNumber()
    {
        return $"ORD-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
    }
    private static string BuildAddressSnapshot(UserAddress address)
    {
        return $"{address.RecipientName} | {address.PhoneNumber} | {address.AddressLine1}, {address.Ward}, {address.District}, {address.City}";
    }
}