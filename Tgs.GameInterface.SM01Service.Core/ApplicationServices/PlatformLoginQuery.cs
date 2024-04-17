namespace Tgs.GameInterface.SM01Service.Core.ApplicationServices;

public record PlatformLoginQuery(
	Uri ApiUrl,
	string MerchantId,
	string MerchantKey,
	string CustomerName);
