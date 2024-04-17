namespace Tgs.GameInterface.SM01Service.Core.ApplicationServices;

public record GameSettledHistoryResult(
	string PlayerCode,
	string ExternalRoundId,
	DateTimeOffset ExternalCreatedOn,
	DateTimeOffset ExternalClosedOn,
	decimal Bet,
	decimal ValidBet,
	decimal PlayerWinLoss,
	decimal Turnover,
	decimal Settle,
	string AdditionalData,
	bool IsCancelled);
