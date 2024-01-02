using System.Runtime.CompilerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SM01Service.Core.ApplicationServices;

internal class GameUnsettleHistoryQueryService(
    IGameApiClient client) : IQueryService<GameUnsettleHistoryQuery, GameUnsettleHistoryResult>
{
    public async IAsyncEnumerable<GameUnsettleHistoryResult> QueryAsync(GameUnsettleHistoryQuery query, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var unsettledBets = await client
                .GetUnsettleTransationsHistoryAsync(
                    apiUrl: query.ApiUrl,
                    merchantId: query.MerchantId,
                    merchantKey: query.MerchantKey,
                    startDate: query.StartDate,
                    endDate: query.EndDate,
                    cancellationToken: cancellationToken)
                .ToArrayAsync(cancellationToken)
                .ConfigureAwait(false);

        if (unsettledBets.Length == 0)
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
