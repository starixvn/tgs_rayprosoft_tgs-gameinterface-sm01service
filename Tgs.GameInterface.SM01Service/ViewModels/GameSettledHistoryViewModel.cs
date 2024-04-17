namespace Tgs.GameInterface.SM01Service.ViewModels;

public record GameSettledHistoryViewModel(
	Uri ApiUrl,
	string MerchantId,
	string MerchantKey,
	DateTime StartDate,
	DateTime EndDate);
