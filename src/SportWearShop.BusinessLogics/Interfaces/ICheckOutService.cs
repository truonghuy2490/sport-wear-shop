using SportWearShop.BusinessLogics.ResponseModels.CheckOutModels;

namespace SportWearShop.BusinessLogics.Interfaces;
public interface ICheckoutService
{
    Task<CheckoutPreviewResponseModel> PreviewAsync(
        long userId,
        CancellationToken cancellationToken = default);

    Task<CreateOrderResponseModel> CreateOrderFromCartAsync(
        long userId,
        CreateOrderFromCartRequestModel request,
        CancellationToken cancellationToken = default);

    Task<PaymentResultResponseModel> ConfirmPaymentAsync(
        long orderId,
        bool isSuccess,
        CancellationToken cancellationToken = default);

    Task<PaymentResultResponseModel> ConfirmCodOrderAsync(
        long orderId,
        CancellationToken cancellationToken = default);
}

