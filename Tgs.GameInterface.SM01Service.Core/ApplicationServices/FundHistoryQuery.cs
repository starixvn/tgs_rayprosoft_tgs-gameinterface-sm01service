namespace Tgs.GameInterface.SM01Service.Core.ApplicationServices;

public record FundHistoryQuery(
    Uri ApiUrl,
    string MerchantId,
    string MerchantKey,
    string CustomerName,
    bool IsFundOut,
    DateTime StartDate,
    DateTime EndDate);
