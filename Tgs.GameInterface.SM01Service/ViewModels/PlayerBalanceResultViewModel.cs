namespace Tgs.GameInterface.SM01Service.ViewModels;

public record PlayerBalanceResultViewModel(
    bool IsPlayerExist,
    int? Balance);
