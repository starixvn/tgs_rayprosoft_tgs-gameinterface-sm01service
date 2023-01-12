using System.Runtime.CompilerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SM01Service.Core.ApplicationServices;

internal class GameUnsettleHistoryQueryService : IQueryService<GameUnsettleHistoryQuery, GameUnsettleHistoryResult>
{
    private readonly IGameApiClient _client;

    public GameUnsettleHistoryQueryService(IGameApiClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async IAsyncEnumerable<GameUnsettleHistoryResult> QueryAsync(GameUnsettleHistoryQuery query, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var unsettledBets = await _client
                .GetUnsettleTransationsHistoryAsync(
                    apiUrl: query.ApiUrl,
                    merchantId: query.MerchantId,
                    merchantKey: query.MerchantKey,
                    startDate: query.StartDate,
                    endDate: query.EndDate,
                    cancellationToken: cancellationToken)
                .ToArrayAsync(cancellationToken)
                .ConfigureAwait(false);

        if (!unsettledBets.Any())
        {
            yield break;
        }

        foreach (var bet in unsettledBets)
        {
            yield return new GameUnsettleHistoryResult(
                PlayerCode: bet.PlayerCode,
                ExternalRoundId: bet.ExternalRoundId,
                ExternalCreatedOn: bet.ExternalCreatedOn,
                Bet: bet.Bet,
                ValidBet: bet.Bet,
                AdditionalData: bet.AdditionalData);
        }
    }
}
