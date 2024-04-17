using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SM01Service.Core.ApplicationServices;

internal class FundInGetService(
	IGameApiClient client) : IGetService<FundInQuery, FundResult>
{
	public async ValueTask<FundResult> GetAsync(FundInQuery query, CancellationToken cancellationToken = default)
	{
		var result = await client.TransferOutAsync(
			apiUrl: query.ApiUrl,
			merchantId: query.MerchantId,
			merchantKey: query.MerchantKey,
			customerName: query.CustomerName,
			amount: query.Amount,
			merchantTransactionId: query.RefTransactionCode,
			cancellationToken: cancellationToken).ConfigureAwait(false);

		return new FundResult(
			RefTransactionCode: result.RefTransactionCode,
			TransactionId: result.TransactionId,
			Balance: result.Balance);
	}
}
