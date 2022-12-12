using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SM01Service.Core.ApplicationServices;

internal class FundInGetService : IGetService<FundInQuery, FundResult>
{
    private readonly IGameApiClient _client;

    public FundInGetService(IGameApiClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<FundResult> GetAsync(FundInQuery query, CancellationToken cancellationToken = default)
    {
        var result = await _client.TransferOutAsync(
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
