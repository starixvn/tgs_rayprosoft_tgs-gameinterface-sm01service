namespace Tgs.GameInterface.SM01Service.Core.ApplicationServices;

public record GameSettledHistoryQuery(
	Uri ApiUrl,
	string MerchantId,
	string MerchantKey,
	DateTime StartDate,
	DateTime EndDate);
