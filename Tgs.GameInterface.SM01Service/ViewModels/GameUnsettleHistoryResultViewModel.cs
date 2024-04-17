namespace Tgs.GameInterface.SM01Service.ViewModels;

public record GameUnsettleHistoryResultViewModel(
	string PlayerCode,
	string ExternalRoundId,
	DateTimeOffset ExternalCreatedOn,
	decimal Bet,
	decimal ValidBet,
	string AdditionalData);
