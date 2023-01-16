namespace Tgs.GameInterface.SM01Service.Core.ApplicationServices;

public record PlayerBalanceResult(
    bool IsPlayerExist,
    decimal? Balance);
