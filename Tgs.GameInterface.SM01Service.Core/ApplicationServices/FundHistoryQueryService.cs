using System.Runtime.CompilerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SM01Service.Core.ApplicationServices;

internal class FundHistoryQueryService(
	IGameApiClient client) : IQueryService<FundHistoryQuery, FundHistoryResult>
{
	public async IAsyncEnumerable<FundHistoryResult> QueryAsync(FundHistoryQuery query, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var fundHistorys = await client
			.GetTransferHistoryAsync(
				apiUrl: query.ApiUrl,
				merchantId: query.MerchantId,
				merchantKey: query.MerchantKey,
				customerName: query.CustomerName,
				isFundOut: query.IsFundOut,
				startDate: query.StartDate,
				endDate: query.EndDate,
				cancellationToken: cancellationToken)
			.ToArrayAsync(cancellationToken)
			.ConfigureAwait(false);

		if (fundHistorys.Length == 0)
		{
			yield break;
		}

		foreach (var fundHistory in fundHistorys)
		{
			yield return new FundHistoryResult(
				TransactionId: fundHistory.TransactionId,
				Amount: fundHistory.Amount,
				CreatedTime: fundHistory.CreatedTime,
				CustomerName: fundHistory.CustomerName);
		}
	}
}
