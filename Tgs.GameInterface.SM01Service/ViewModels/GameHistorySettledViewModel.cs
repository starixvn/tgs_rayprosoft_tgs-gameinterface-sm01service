namespace Tgs.GameInterface.SM01Service.ViewModels;

public record GameHistorySettledViewModel(
    Uri ApiUrl,
    string MerchantId,
    string MerchantKey,
    DateTime StartDate,
    DateTime EndDate);
