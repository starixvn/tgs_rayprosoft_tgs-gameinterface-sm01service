namespace Tgs.GameInterface.SM01Service.Core.Models;

public record TransferOutResult(
    string RefTransactionCode,
    string TransactionId,
    int Balance);
