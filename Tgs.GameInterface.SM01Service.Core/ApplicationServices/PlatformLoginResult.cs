namespace Tgs.GameInterface.SM01Service.Core.ApplicationServices;

public record PlatformLoginResult(
	string ApiUrl,
	string MerchantId,
	string CustomerName,
	long Timestamp,
	string Sign);
