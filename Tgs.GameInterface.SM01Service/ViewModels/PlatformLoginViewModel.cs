namespace Tgs.GameInterface.SM01Service.ViewModels;

public record PlatformLoginViewModel(
	Uri ApiUrl,
	string MerchantId,
	string MerchantKey,
	string CustomerName);
