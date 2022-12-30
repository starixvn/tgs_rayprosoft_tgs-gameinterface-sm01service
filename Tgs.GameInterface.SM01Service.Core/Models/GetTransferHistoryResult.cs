namespace Tgs.GameInterface.SM01Service.Core.Models;

public record GetTransferHistoryResult
{
    public string TransactionId { get; init; } = default!;

    public int Amount { get; init; }

    public string CustomerName { get; init; } = default!;

    public DateTime CreatedTime { get; init; }
}
