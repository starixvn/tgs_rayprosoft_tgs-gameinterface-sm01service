using Tgs.GameInterface.SM01Service.Protos;

namespace Tgs.GameInterface.SM01Service.Client;

public interface IGameInterfaceSM01Client
{
    ValueTask<FundReply> FundInAsync(
       Uri apiUrl,
       string merchantId,
       string merchantKey,
       string customerName,
       string refTransactionCode,
       int amount,
       CancellationToken cancellationToken = default);

    ValueTask<FundReply> FundOutAsync(
        Uri apiUrl,
        string merchantId,
        string merchantKey,
        string customerName,
        string refTransactionCode,
        int amount,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<FundHistory> FundHistoryAsync(
        Uri apiUrl,
        string merchantId,
        string merchantKey,
        string customerName,
        bool isFundOut,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    ValueTask PlayerCreateAsync(
        Uri apiUrl,
        string merchantId,
        string merchantKey,
        string customerName,
        CancellationToken cancellationToken = default);

    ValueTask<PlayerBalanceReply> PlayerBalanceAsync(
        Uri apiUrl,
        string merchantId,
        string merchantKey,
        string customerName,
        CancellationToken cancellationToken = default);

    ValueTask<PlatformLoginReply> PlatformLoginAsync(
        Uri apiUrl,
        string merchantId,
        string merchantKey,
        string customerName,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<GameSettledHistory> GameSettledHistoryAsync(
        Uri apiUrl,
        string merchantId,
        string merchantKey,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<GameUnsettleHistory> GameUnsettleHistoryAsync(
        Uri apiUrl,
        string merchantId,
        string merchantKey,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);
}
