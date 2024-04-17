namespace Tgs.GameInterface.SM01Service.ViewModels;

public record FundHistoryViewModel(
	Uri ApiUrl,
	string MerchantId,
	string MerchantKey,
	string CustomerName,
	bool IsFundOut,
	DateTime StartDate,
	DateTime EndDate);
