namespace Tgs.GameInterface.SM01Service.Core.ApplicationServices;

public record FundHistoryResult(
    string TransactionId,
    int Amount,
    DateTime CreatedTime,
    string CustomerName);
