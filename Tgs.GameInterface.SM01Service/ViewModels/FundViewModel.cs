namespace Tgs.GameInterface.SM01Service.ViewModels;

public record FundViewModel(
    Uri ApiUrl,
    string MerchantId,
    string MerchantKey,
    string CustomerName,
    string RefTransactionCode,
    int Amount);
