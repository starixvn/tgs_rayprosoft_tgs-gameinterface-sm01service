namespace Tgs.GameInterface.SM01Service.ViewModels;

public record GameHistorySettledResultViewModel(
    string PlayerCode,
    string ExternalRoundId,
    bool IsClosed,
    DateTimeOffset ExternalCreatedOn,
    DateTimeOffset ExternalClosedOn,
    decimal Bet,
    decimal ValidBet,
    decimal PlayerWinLoss,
    decimal Turnover,
    decimal Settle,
    string AdditionalData);
