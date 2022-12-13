namespace Tgs.GameInterface.SM01Service.Core.ApplicationServices;

public record PlatformLoginResult(
    string MerchantId,
    string CustomerName,
    long Timestamp,
    string Sign);
