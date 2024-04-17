namespace Tgs.GameInterface.SM01Service.ViewModels;

public record PlayerCreateViewModel(
	Uri ApiUrl,
	string MerchantId,
	string MerchantKey,
	string CustomerName);
