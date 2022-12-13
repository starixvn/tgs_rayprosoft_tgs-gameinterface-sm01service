using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SM01Service.Core.ApplicationServices;

internal class PlatformLoginGetService : IGetService<PlatformLoginQuery, PlatformLoginResult>
{
    private readonly IGameApiClient _client;

    public PlatformLoginGetService(IGameApiClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<PlatformLoginResult> GetAsync(PlatformLoginQuery query, CancellationToken cancellationToken = default)
    {
        var result = await _client.PlatformLoginAsync(
                query.ApiUrl,
                query.MerchantId,
                query.MerchantKey,
                query.CustomerName,
                cancellationToken);
        return new PlatformLoginResult(result.MerchantId, result.CustomerName, result.Timestamp, result.Sign);
    }
}
