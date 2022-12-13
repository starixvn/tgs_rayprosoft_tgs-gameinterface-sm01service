namespace Tgs.GameInterface.SM01Service.Core.Models;

public record PlatformLoginResult(
    string MerchantId,
    string CustomerName,
    long Timestamp,
    string Sign);
