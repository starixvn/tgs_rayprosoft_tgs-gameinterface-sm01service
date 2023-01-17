namespace Tgs.GameInterface.SM01Service.ViewModels;

public record GameSettledHistoryResultViewModel(
    string PlayerCode,
    string ExternalRoundId,
    DateTimeOffset ExternalCreatedOn,
    DateTimeOffset? ExternalClosedOn,
    decimal Bet,
    decimal ValidBet,
    decimal PlayerWinLoss,
    decimal Turnover,
    decimal Settle,
    string AdditionalData,
    bool IsCancelled);
