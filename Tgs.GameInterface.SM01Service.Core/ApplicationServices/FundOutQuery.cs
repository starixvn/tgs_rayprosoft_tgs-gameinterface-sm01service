namespace Tgs.GameInterface.SM01Service.Core.ApplicationServices;

public record FundOutQuery(
	Uri ApiUrl,
	string MerchantId,
	string MerchantKey,
	string CustomerName,
	string RefTransactionCode,
	int Amount);
