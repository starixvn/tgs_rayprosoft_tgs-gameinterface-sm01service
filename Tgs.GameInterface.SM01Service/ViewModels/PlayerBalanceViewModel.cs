namespace Tgs.GameInterface.SM01Service.ViewModels;

public record PlayerBalanceViewModel(
    Uri ApiUrl,
    string MerchantId,
    string MerchantKey,
    string CustomerName);
