using System.Runtime.CompilerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SM01Service.Core.ApplicationServices;

internal class GameSettledHistoryQueryService(
    IGameApiClient client) : IQueryService<GameSettledHistoryQuery, GameSettledHistoryResult>
{
    public async IAsyncEnumerable<GameSettledHistoryResult> QueryAsync(GameSettledHistoryQuery query, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var bettingHistoryBets = await client
                .GetSettledTransationsHistoryAsync(
                    apiUrl: query.ApiUrl,
                    merchantId: query.MerchantId,
                    merchantKey: query.MerchantKey,
                    startDate: query.StartDate,
                    endDate: query.EndDate,
                    cancellationToken: cancellationToken)
                .ToArrayAsync(cancellationToken)
                .ConfigureAwait(false);

        if (bettingHistoryBets.Length == 0)
        {
            yield break;
        }

        foreach (var bettingHistoryBet in bettingHistoryBets)
        {
            var validBet = bettingHistoryBet.IsCancelled ? 0M : bettingHistoryBet.Bet;
            var turnover = bettingHistoryBet.IsCancelled ? 0M : bettingHistoryBet.Bet;

            yield return new GameSettledHistoryResult(
                PlayerCode: bettingHistoryBet.PlayerCode,
                ExternalRoundId: bettingHistoryBet.ExternalRoundId,
                ExternalCreatedOn: bettingHistoryBet.ExternalCreatedOn,
                ExternalClosedOn: bettingHistoryBet.ExternalClosedOn,
                Bet: bettingHistoryBet.Bet,
                ValidBet: validBet,
                PlayerWinLoss: bettingHistoryBet.WinOrLost,
                Turnover: turnover,
                Settle: bettingHistoryBet.Bet + bettingHistoryBet.WinOrLost,
                AdditionalData: bettingHistoryBet.AdditionalData,
                IsCancelled: bettingHistoryBet.IsCancelled);
        }
    }
}
