using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SM01Service.Core.ApplicationServices
{
    internal class FundHistoryQueryService : IQueryService<FundHistoryQuery, FundHistoryResult>
    {
        private readonly IGameApiClient _client;

        public FundHistoryQueryService(IGameApiClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async IAsyncEnumerable<FundHistoryResult> QueryAsync(FundHistoryQuery query, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var fundHistorys = await _client
                .GetTransferHistoryAsync(
                    apiUrl: query.ApiUrl,
                    merchantId: query.MerchantId,
                    merchantKey: query.MerchantKey,
                    customerName: query.CustomerName,
                    isFundOut: query.IsFundOut,
                    startDate: query.StartDate,
                    endDate: query.EndDate,
                    cancellationToken: cancellationToken)
                .ToArrayAsync(cancellationToken)
                .ConfigureAwait(false);

            if (!fundHistorys.Any())
            {
                yield break;
            }

            foreach (var fundHistory in fundHistorys)
            {                
                yield return new FundHistoryResult(
                    TransactionId: fundHistory.TransactionId,
                    Amount: fundHistory.Amount,
                    CreatedTime: fundHistory.CreatedTime,
                    CustomerName: fundHistory.CustomerName);
            }
        }
    }
}
