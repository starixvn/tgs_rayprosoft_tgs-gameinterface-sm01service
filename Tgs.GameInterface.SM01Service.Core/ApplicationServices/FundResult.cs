namespace Tgs.GameInterface.SM01Service.Core.ApplicationServices;

public record FundResult(
    string RefTransactionCode,
    string TransactionId,
    decimal Balance);
