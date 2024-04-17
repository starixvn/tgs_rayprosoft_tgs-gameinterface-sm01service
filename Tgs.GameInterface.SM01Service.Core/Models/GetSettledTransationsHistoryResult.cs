namespace Tgs.GameInterface.SM01Service.Core.Models;

public record GetSettledTransationsHistoryResult
{
	public string ExternalRoundId { get; init; } = default!;
	public decimal WinOrLost { get; init; }
	public DateTimeOffset ExternalCreatedOn { get; init; }
	public DateTimeOffset ExternalClosedOn { get; init; }
	public decimal Bet { get; init; }
	public string PlayerCode { get; init; } = default!;
	public string AdditionalData { get; init; } = default!;
	public bool IsCancelled { get; init; }
}
