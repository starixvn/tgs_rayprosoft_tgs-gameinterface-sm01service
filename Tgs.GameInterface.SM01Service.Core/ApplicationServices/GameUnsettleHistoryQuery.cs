namespace Tgs.GameInterface.SM01Service.Core.ApplicationServices;

public record GameUnsettleHistoryQuery(
	Uri ApiUrl,
	string MerchantId,
	string MerchantKey,
	DateTime StartDate,
	DateTime EndDate);
