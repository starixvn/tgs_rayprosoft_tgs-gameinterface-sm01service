using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SM01Service.Core.ApplicationServices;

internal class FundOutGetService(
	IGameApiClient client) : IGetService<FundOutQuery, FundResult>
{
	public async ValueTask<FundResult> GetAsync(FundOutQuery query, CancellationToken cancellationToken = default)
	{
		var result = await client.TransferInAsync(
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
