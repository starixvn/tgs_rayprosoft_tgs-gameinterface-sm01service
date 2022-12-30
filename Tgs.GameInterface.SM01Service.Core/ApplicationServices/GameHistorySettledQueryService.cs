using System.Runtime.CompilerServices;
using System.Text.Json;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SM01Service.Core.ApplicationServices;

internal class GameHistorySettledQueryService : IQueryService<GameHistorySettledQuery, GameHistorySettledResult>
{
    private readonly IGameApiClient _client;

    public GameHistorySettledQueryService(
        IGameApiClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async IAsyncEnumerable<GameHistorySettledResult> QueryAsync(GameHistorySettledQuery query, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var bettingHistoryBets = await _client
                .GetTransationsHistoryAsync(
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
            
            yield return new GameHistorySettledResult(
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
