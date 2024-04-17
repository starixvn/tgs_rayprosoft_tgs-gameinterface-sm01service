using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SM01Service.Core.ApplicationServices;

internal class PlayerCreateCommandService(
	IGameApiClient client) : ICommandService<PlayerCreateCommand>
{
	public async ValueTask ExecuteAsync(PlayerCreateCommand command, CancellationToken cancellationToken = default)
	{
		var _ = await client.CheckUserAsync(
			apiUrl: command.ApiUrl,
			merchantId: command.MerchantId,
			merchantKey: command.MerchantKey,
			customerName: command.CustomerName,
			cancellationToken).ConfigureAwait(false);
	}
}
