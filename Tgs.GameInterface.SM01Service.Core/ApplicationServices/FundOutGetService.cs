using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SM01Service.Core.ApplicationServices;

internal class FundOutGetService : IGetService<FundOutQuery, FundResult>
{
    private readonly IGameApiClient _client;

    public FundOutGetService(IGameApiClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<FundResult> GetAsync(FundOutQuery query, CancellationToken cancellationToken = default)
    {
        var result = await _client.TransferInAsync(
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
