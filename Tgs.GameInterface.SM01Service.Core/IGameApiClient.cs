using Tgs.GameInterface.SM01Service.Core.Models;

namespace Tgs.GameInterface.SM01Service.Core;

public interface IGameApiClient
{
    ValueTask<CheckUserResult> CheckUserAsync(Uri apiUrl, string merchantId, string merchantKey, string customerName, CancellationToken cancellationToken = default);

    ValueTask<TransferInResult> TransferInAsync(Uri apiUrl, string merchantId, string merchantKey, string customerName, int amount, string merchantTransactionId, CancellationToken cancellationToken = default);

    ValueTask<TransferOutResult> TransferOutAsync(Uri apiUrl, string merchantId, string merchantKey, string customerName, int amount, string merchantTransactionId, CancellationToken cancellationToken = default);

    ValueTask<PlatformLoginResult> PlatformLoginAsync(Uri apiUrl, string merchantId, string merchantKey, string customerName,CancellationToken cancellationToken = default);

    IAsyncEnumerable<GetTransferHistoryResult> GetTransferHistoryAsync(Uri apiUrl, string merchantId, string merchantKey, string customerName, bool isFundOut, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    IAsyncEnumerable<GetSettledTransationsHistoryResult> GetSettledTransationsHistoryAsync(Uri apiUrl, string merchantId, string merchantKey, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    IAsyncEnumerable<GetUnsettleTransationsHistoryResult> GetUnsettleTransationsHistoryAsync(Uri apiUrl, string merchantId, string merchantKey, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    ValueTask<GetUserBalanceResult> GetUserBalanceAsync(Uri apiUrl, string merchantId, string merchantKey, string customerName, CancellationToken cancellationToken = default);
}
