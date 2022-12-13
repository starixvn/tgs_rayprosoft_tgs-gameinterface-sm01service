namespace Tgs.GameInterface.SM01Service.ViewModels;

public record PlatformLoginResultViewModel(
    string MerchantId,
    string CustomerName,
    long Timestamp,
    string Sign);
