namespace Tgs.GameInterface.SM01Service.ViewModels;

public record GameUnsettleHistoryViewModel(
    Uri ApiUrl,
    string MerchantId,
    string MerchantKey,
    DateTime StartDate,
    DateTime EndDate);
