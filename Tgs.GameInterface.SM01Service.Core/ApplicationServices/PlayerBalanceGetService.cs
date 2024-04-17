using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SM01Service.Core.ApplicationServices;

internal class PlayerBalanceGetService(
	IGameApiClient client) : IGetService<PlayerBalanceQuery, PlayerBalanceResult>
{
	public async ValueTask<PlayerBalanceResult> GetAsync(PlayerBalanceQuery query, CancellationToken cancellationToken = default)
	{
		var result = await client.GetUserBalanceAsync(
			apiUrl: query.ApiUrl,
			merchantId: query.MerchantId,
			merchantKey: query.MerchantKey,
			customerName: query.CustomerName,
			cancellationToken: cancellationToken).ConfigureAwait(false);

		return new PlayerBalanceResult(
			 IsPlayerExist: result.IsPlayerExist,
			 Balance: result.Balance);
	}
}
