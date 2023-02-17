namespace Tgs.GameInterface.SM01Service.ViewModels;

public record PlatformLoginResultViewModel(
    string ApiUrl,
    string MerchantId,
    string CustomerName,
    long Timestamp,
    string Sign);
