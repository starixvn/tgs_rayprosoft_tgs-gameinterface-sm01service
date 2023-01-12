namespace Tgs.GameInterface.SM01Service.Core.Models;

public record GetUnsettleTransationsHistoryResult
{
    public string ExternalRoundId { get; init; } = default!;
    public int WinOrLost { get; init; }
    public DateTimeOffset ExternalCreatedOn { get; init; }
    public decimal Bet { get; init; }
    public string PlayerCode { get; init; } = default!;

    public string AdditionalData { get; init; } = default!;
}
