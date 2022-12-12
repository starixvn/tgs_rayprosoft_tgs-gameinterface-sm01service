namespace Tgs.GameInterface.SM01Service.Core.ApplicationServices;

public record PlayerCreateCommand(
    Uri ApiUrl,
    string MerchantId,
    string MerchantKey,
    string CustomerName);
