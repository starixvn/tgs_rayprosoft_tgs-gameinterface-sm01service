namespace Tgs.GameInterface.SM01Service.Core.Models;

public record GetUserBalanceResult(
    bool IsPlayerExist,
    int Balance);
