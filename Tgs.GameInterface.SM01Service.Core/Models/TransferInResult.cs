namespace Tgs.GameInterface.SM01Service.Core.Models;

public record TransferInResult(
    string RefTransactionCode,
    string TransactionId,
    int Balance);
