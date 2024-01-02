using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SM01Service.Core.ApplicationServices;

internal class PlatformLoginGetService(
    IGameApiClient client) : IGetService<PlatformLoginQuery, PlatformLoginResult>
{
    public async ValueTask<PlatformLoginResult> GetAsync(PlatformLoginQuery query, CancellationToken cancellationToken = default)
    {
        var result = await client.PlatformLoginAsync(
                query.ApiUrl,
                query.MerchantId,
                query.MerchantKey,
                query.CustomerName,
                cancellationToken);
        return new PlatformLoginResult(result.ApiUrl, result.MerchantId, result.CustomerName, result.Timestamp, result.Sign);
    }
}
