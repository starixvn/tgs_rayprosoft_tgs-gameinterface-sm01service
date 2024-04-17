namespace Tgs.GameInterface.SM01Service.Core.ApplicationServices;

public record PlayerBalanceQuery(
	Uri ApiUrl,
	string MerchantId,
	string MerchantKey,
	string CustomerName);
