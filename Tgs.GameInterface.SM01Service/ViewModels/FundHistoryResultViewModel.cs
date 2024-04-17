namespace Tgs.GameInterface.SM01Service.ViewModels;

public record FundHistoryResultViewModel(
	string TransactionId,
	int Amount,
	DateTime CreatedTime,
	string PlayerCode);
