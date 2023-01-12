namespace Tgs.GameInterface.SM01Service.Core.ApplicationServices;

public record GameUnsettleHistoryResult(
     string PlayerCode,
     string ExternalRoundId,
     DateTimeOffset ExternalCreatedOn,
     decimal Bet,
     decimal ValidBet,
     string AdditionalData);
