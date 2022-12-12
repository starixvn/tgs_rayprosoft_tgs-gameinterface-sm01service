namespace Tgs.GameInterface.SM01Service.Core.ApplicationServices;

public record GameHistorySettledQuery(
    Uri ApiUrl,
    string MerchantId,
    string MerchantKey,
    DateTime StartDate,
    DateTime EndDate);
