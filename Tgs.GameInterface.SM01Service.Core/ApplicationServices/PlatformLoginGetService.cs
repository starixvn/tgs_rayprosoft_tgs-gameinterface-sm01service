using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SM01Service.Core.ApplicationServices;

internal class PlatformLoginGetService : IGetService<PlatformLoginQuery, string>
{
    private readonly IGameApiClient _client;

    public PlatformLoginGetService(IGameApiClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public ValueTask<string> GetAsync(PlatformLoginQuery query, CancellationToken cancellationToken = default)
        => _client.PlatformLoginAsync(
            query.ApiUrl,
            query.MerchantId,
            query.MerchantKey,
            query.CustomerName,
            cancellationToken);
}
