using System.Runtime.CompilerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SM01Service.Core.ApplicationServices;

internal class GameSettledHistoryQueryService : IQueryService<GameSettledHistoryQuery, GameSettledHistoryResult>
{
    private readonly IGameApiClient _client;

    public GameSettledHistoryQueryService(
        IGameApiClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async IAsyncEnumerable<GameSettledHistoryResult> QueryAsync(GameSettledHistoryQuery query, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var bettingHistoryBets = await _client
                .GetSettledTransationsHistoryAsync(
                    apiUrl: query.ApiUrl,
                    merchantId: query.MerchantId,
                    merchantKey: query.MerchantKey,
                    startDate: query.StartDate,
                    endDate: query.EndDate,
                    cancellationToken: cancellationToken)
                .ToArrayAsync(cancellationToken)
                .ConfigureAwait(false);

        if (!bettingHistoryBets.Any())
        {
            yield break;
        }

        foreach (var bettingHistoryBet in bettingHistoryBets)
        {
            yield return new GameSettledHistoryResult(
                PlayerCode: bettingHistoryBet.PlayerCode,
                ExternalRoundId: bettingHistoryBet.ExternalRoundId,
                ExternalCreatedOn: bettingHistoryBet.ExternalCreatedOn,
                ExternalClosedOn: bettingHistoryBet.ExternalClosedOn,
                Bet: bettingHistoryBet.Bet,
                ValidBet: bettingHistoryBet.Bet,
                PlayerWinLoss: decimal.Parse(bettingHistoryBet.WinOrLost.ToString()),
                Turnover: bettingHistoryBet.Bet,
                Settle: bettingHistoryBet.Bet + decimal.Parse(bettingHistoryBet.WinOrLost.ToString()),
                AdditionalData: bettingHistoryBet.AdditionalData);
        }
    }
}
