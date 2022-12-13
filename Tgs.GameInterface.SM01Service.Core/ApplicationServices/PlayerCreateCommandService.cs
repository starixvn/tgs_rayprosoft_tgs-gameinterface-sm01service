using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SM01Service.Core.ApplicationServices;

internal class PlayerCreateCommandService : ICommandService<PlayerCreateCommand>
{
    private readonly IGameApiClient _client;

    public PlayerCreateCommandService(
        IGameApiClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask ExecuteAsync(PlayerCreateCommand command, CancellationToken cancellationToken = default)
    {
        var _ = await _client.CheckUserAsync(
            apiUrl: command.ApiUrl,
            merchantId: command.MerchantId,
            merchantKey: command.MerchantKey,
            customerName: command.CustomerName,
            cancellationToken).ConfigureAwait(false);
    }
}
