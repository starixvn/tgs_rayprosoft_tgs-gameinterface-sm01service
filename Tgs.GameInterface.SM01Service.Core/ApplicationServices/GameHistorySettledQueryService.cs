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
            DateTimeOffset externalClosedOn = DateTime.UnixEpoch;

            yield return new GameHistorySettledResult(
                PlayerCode: bettingHistoryBet.PlayerCode,
                ExternalRoundId: bettingHistoryBet.BetId,
                IsClosed: true,
                ExternalCreatedOn: bettingHistoryBet.CreateAt,
                ExternalClosedOn: externalClosedOn,
                Bet: bettingHistoryBet.BetAmount,
                ValidBet: bettingHistoryBet.BetAmount,
                PlayerWinLoss: decimal.Parse(bettingHistoryBet.WinOrLost.ToString()),
                Turnover: bettingHistoryBet.BetAmount,
                Settle: bettingHistoryBet.BetAmount + decimal.Parse(bettingHistoryBet.WinOrLost.ToString()),
                AdditionalData: JsonSerializer.Serialize(bettingHistoryBet));
        }
    }
}
