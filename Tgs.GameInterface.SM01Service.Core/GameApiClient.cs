using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using Tgs.GameInterface.SM01Service.Core.Exceptions;
using Tgs.GameInterface.SM01Service.Core.Models;

namespace Tgs.GameInterface.SM01Service.Core;

internal class GameApiClient : IGameApiClient
{
    private readonly HttpClient _client;
    private readonly ILogger<GameApiClient> _logger;

    public GameApiClient(
        HttpClient client,
        ILogger<GameApiClient> logger)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async ValueTask<CheckUserResult> CheckUserAsync(Uri apiUrl, string merchantId, string merchantKey, string customerName, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            ["merchantId"] = merchantId,
            ["customerName"] = customerName,
            ["timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
        };

        var sign = SM01Hasher.ComputeHash(merchantKey, parameters);
        parameters.Add("sign", sign);

        var url = new Uri(apiUrl, "/api/merchant/checkUser");

        var jsonNode = await PostAsync(
            url,
            parameters,
            "CheckUser",
            cancellationToken).ConfigureAwait(false);

        var process = jsonNode["process"]!.ToString() == "success";

        if (!process)
        {
            var errorCode = jsonNode["code"]!.GetValue<int>();

            var errorMessage = jsonNode["result"]!.ToString();

            throw new GameProviderResponseErrorException(errorCode.ToString(), errorMessage);
        }

        var state = jsonNode["data"]!["state"]!.GetValue<int>();//狀態：1正常 2凍結 0禁用

        var isPlayerExist = state == 1;

        return new CheckUserResult(isPlayerExist);
    }

    public async IAsyncEnumerable<GetSettledTransationsHistoryResult> GetSettledTransationsHistoryAsync(Uri apiUrl, string merchantId, string merchantKey, DateTime startDate, DateTime endDate, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            ["merchantId"] = merchantId,
            ["timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
            ["StartDate"] = startDate.DatetimeConvertToString(),
            ["EndDate"] = endDate.DatetimeConvertToString(),
            ["isResult"] = 1.ToString(),//1:settlement
        };

        var sign = SM01Hasher.ComputeHash(merchantKey, parameters);
        parameters.Add("sign", sign);

        var url = new Uri(apiUrl, "/api/merchant/getTransationsHistory");

        var jsonNode = await PostAsync(
            url,
            parameters,
            "GetSettledTransationsHistory",
            cancellationToken).ConfigureAwait(false);

        var process = jsonNode["process"]!.ToString() == "success";

        if (!process)
        {
            var errorCode = jsonNode["code"]!.GetValue<int>();

            var errorMessage = jsonNode["result"]!.ToString();

            throw new GameProviderResponseErrorException(errorCode.ToString(), errorMessage);
        }

        var data = jsonNode["data"];

        if (data is null)
            yield break;

        IEnumerable<GetSettledTransationsHistoryResult> results = new List<GetSettledTransationsHistoryResult>();

        if (data["list"] is not null && (data["list"]?.AsArray().Any() ?? false))
        {
            var array = data["list"]!.AsArray();

            results = array.Select(x => new GetSettledTransationsHistoryResult
            {
                PlayerCode = x!["customerName"]!.ToString(),
                ExternalRoundId = x!["bet_id"]!.ToString(),
                ExternalCreatedOn = new DateTimeOffset(DateTime.ParseExact(x!["create_at"]!.ToString(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture), TimeSpan.FromHours(8D)),
                ExternalClosedOn = new DateTimeOffset(DateTime.ParseExact(x!["update_at"]!.ToString(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture), TimeSpan.FromHours(8D)),
                WinOrLost = x!["win_or_lost"]!.GetValue<int>(),
                Bet = x!["bet_amount"]!.GetValue<decimal>(),
                AdditionalData = x.ToString(),
                IsCancelled = x["winner"]!.ToString() == "false",
            });
        }

        if (!results.Any())
        {
            yield break;
        }

        foreach (var result in results)
        {
            yield return result;
        }
    }

    public async IAsyncEnumerable<GetUnsettleTransationsHistoryResult> GetUnsettleTransationsHistoryAsync(Uri apiUrl, string merchantId, string merchantKey, DateTime startDate, DateTime endDate, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            ["merchantId"] = merchantId,
            ["timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
            ["StartDate"] = startDate.DatetimeConvertToString(),
            ["EndDate"] = endDate.DatetimeConvertToString(),
            ["isResult"] = 2.ToString(),//2：unsettlement
        };

        var sign = SM01Hasher.ComputeHash(merchantKey, parameters);
        parameters.Add("sign", sign);

        var url = new Uri(apiUrl, "/api/merchant/getTransationsHistory");

        var jsonNode = await PostAsync(
            url,
            parameters,
            "GetUnsettleTransationsHistory",
            cancellationToken).ConfigureAwait(false);

        var process = jsonNode["process"]!.ToString() == "success";

        if (!process)
        {
            var errorCode = jsonNode["code"]!.GetValue<int>();

            var errorMessage = jsonNode["result"]!.ToString();

            throw new GameProviderResponseErrorException(errorCode.ToString(), errorMessage);
        }

        var data = jsonNode["data"];

        if (data is null)
            yield break;

        IEnumerable<GetUnsettleTransationsHistoryResult> results = new List<GetUnsettleTransationsHistoryResult>();

        if (data["list"] is not null && (data["list"]?.AsArray().Any() ?? false))
        {
            var array = data["list"]!.AsArray();

            results = array.Select(x => new GetUnsettleTransationsHistoryResult
            {
                PlayerCode = x!["customerName"]!.ToString(),
                ExternalRoundId = x!["bet_id"]!.ToString(),
                ExternalCreatedOn = new DateTimeOffset(DateTime.ParseExact(x!["create_at"]!.ToString(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture), TimeSpan.FromHours(8D)),
                WinOrLost = x!["win_or_lost"]!.GetValue<int>(),
                Bet = x!["bet_amount"]!.GetValue<decimal>(),
                AdditionalData = x.ToString(),
            });
        }

        if (!results.Any())
        {
            yield break;
        }

        foreach (var result in results)
        {
            yield return result;
        }
    }

    public async IAsyncEnumerable<GetTransferHistoryResult> GetTransferHistoryAsync(Uri apiUrl, string merchantId, string merchantKey, string customerName, bool isFundOut, DateTime startDate, DateTime endDate, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            ["merchantId"] = merchantId,
            ["timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
            ["StartDate"] = startDate.ToString("yyyy-MM-dd HH:mm:ss"),
            ["EndDate"] = endDate.ToString("yyyy-MM-dd HH:mm:ss"),
            ["customerName"] = customerName,
        };

        if (isFundOut)
            parameters.Add("type", "1");//1轉入
        else
            parameters.Add("type", "2");//2轉出

        var sign = SM01Hasher.ComputeHash(merchantKey, parameters);
        parameters.Add("sign", sign);

        var url = new Uri(apiUrl, "/api/merchant/getTransferHistory");

        var jsonNode = await PostAsync(
            url,
            parameters,
            "GetTransferHistory",
            cancellationToken).ConfigureAwait(false);

        var process = jsonNode["process"]!.ToString() == "success";

        if (!process)
        {
            var errorCode = jsonNode["code"]!.ToString();

            var errorMessage = jsonNode["result"]!.ToString();

            throw new GameProviderResponseErrorException(errorCode, errorMessage);
        }

        var data = jsonNode["data"];

        if (data is null)
            yield break;

        IEnumerable<GetTransferHistoryResult> results = new List<GetTransferHistoryResult>();

        if (data["list"] is not null && (data["list"]?.AsArray().Any() ?? false))
        {
            var array = data["list"]!.AsArray();

            results = array.Select(x => new GetTransferHistoryResult
            {
                TransactionId = x!["merchant_transaction_id"]!.ToString(),
                Amount = x["amount"]!.GetValue<int>(),
                CustomerName = x["customerName"]!.ToString(),
                CreatedTime = DateTime.ParseExact(x["created_time"]!.ToString(), "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture),
            });
        }

        if (!results.Any())
        {
            yield break;
        }

        foreach (var result in results)
        {
            yield return result;
        }
    }

    public async ValueTask<GetUserBalanceResult> GetUserBalanceAsync(Uri apiUrl, string merchantId, string merchantKey, string customerName, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            ["merchantId"] = merchantId,
            ["customerName"] = customerName,
            ["timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
        };

        var sign = SM01Hasher.ComputeHash(merchantKey, parameters);
        parameters.Add("sign", sign);

        var url = new Uri(apiUrl, "/api/merchant/getUserBalance");

        var jsonNode = await PostAsync(
            url,
            parameters,
            "GetUserBalance",
            cancellationToken).ConfigureAwait(false);

        var process = jsonNode["process"]!.ToString() == "success";

        if (!process)
        {
            var errorCode = jsonNode["code"]!.ToString();

            var errorMessage = jsonNode["result"]!.ToString();

            _logger.LogError("getUserBalance failed, errorCode:{errorCode}, errorMessage:{errorMessage}", errorCode, errorMessage);
            return new GetUserBalanceResult(false, 0M);
        }

        var data = jsonNode["data"];

        if (data is null)
            return new GetUserBalanceResult(false, 0);

        var balance = data["balance"]!.GetValue<decimal>();

        return new GetUserBalanceResult(true, balance);
    }

    public ValueTask<PlatformLoginResult> PlatformLoginAsync(Uri apiUrl, string merchantId, string merchantKey, string customerName, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            ["merchantId"] = merchantId,
            ["customerName"] = customerName,
            ["timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
        };

        var sign = SM01Hasher.ComputeHash(merchantKey, parameters);
        parameters.Add("sign", sign);

        _logger.LogInformation("SM01 PlatformLoginAsync, parameters:{parameters}", JsonSerializer.Serialize(parameters));

        return ValueTask.FromResult(new PlatformLoginResult(apiUrl.ToString(), merchantId, customerName, long.Parse(parameters["timestamp"]), parameters["sign"]));
    }

    public async ValueTask<TransferInResult> TransferInAsync(Uri apiUrl, string merchantId, string merchantKey, string customerName, int amount, string merchantTransactionId, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            ["merchantId"] = merchantId,
            ["customerName"] = customerName,
            ["amount"] = amount.ToString(),
            ["timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
            ["merchantTransactionId"] = merchantTransactionId.Replace("-", ""),
        };

        var sign = SM01Hasher.ComputeHash(merchantKey, parameters);
        parameters.Add("sign", sign);

        var url = new Uri(apiUrl, "/api/merchant/transferIn");

        var jsonNode = await PostAsync(
            url,
            parameters,
            "TransferIn",
            cancellationToken).ConfigureAwait(false);

        var process = jsonNode["process"]!.ToString() == "success";

        if (!process)
        {
            var errorCode = jsonNode["code"]!.GetValue<int>();

            var errorMessage = jsonNode["result"]!.ToString();

            throw new GameProviderResponseErrorException(errorCode.ToString(), errorMessage);
        }

        var data = jsonNode["data"];

        if (data is null)
        {
            _logger.LogInformation("SM01 TransferIn data is null");
            return new TransferInResult(merchantTransactionId, merchantTransactionId.Replace("-", ""), 0);
        }

        var balance = data["balance"]!.GetValue<int>();

        return new TransferInResult(merchantTransactionId, merchantTransactionId.Replace("-", ""), balance);
    }

    public async ValueTask<TransferOutResult> TransferOutAsync(Uri apiUrl, string merchantId, string merchantKey, string customerName, int amount, string merchantTransactionId, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            ["merchantId"] = merchantId,
            ["customerName"] = customerName,
            ["amount"] = amount.ToString(),
            ["timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
            ["merchantTransactionId"] = merchantTransactionId.Replace("-", ""),//因廠商限制長度32
        };

        var sign = SM01Hasher.ComputeHash(merchantKey, parameters);
        parameters.Add("sign", sign);

        var url = new Uri(apiUrl, "/api/merchant/transferOut");

        var jsonNode = await PostAsync(
            url,
            parameters,
            "TransferOut",
            cancellationToken).ConfigureAwait(false);

        var process = jsonNode["process"]!.ToString() == "success";

        if (!process)
        {
            var errorCode = jsonNode["code"]!.GetValue<int>();

            var errorMessage = jsonNode["result"]!.ToString();

            throw new GameProviderResponseErrorException(errorCode.ToString(), errorMessage);
        }

        var data = jsonNode["data"];
        if (data is null)
        {
            _logger.LogInformation("SM01 TransferOut data is null");
            return new TransferOutResult(merchantTransactionId, merchantTransactionId.Replace("-", ""), 0);
        }

        var balance = data["balance"]!.GetValue<int>();

        return new TransferOutResult(merchantTransactionId, merchantTransactionId.Replace("-", ""), balance);
    }

    private async ValueTask<JsonNode> PostAsync(Uri apiUrl, IEnumerable<KeyValuePair<string, string>> parameters, string logPrefix, CancellationToken cancellationToken = default)
    {
        logPrefix = $"{nameof(GameApiClient)}.{logPrefix}".ToLower();

        var sw = Stopwatch.StartNew();

        HttpContent content = new FormUrlEncodedContent(parameters);

        var httpResponseMessage = await _client.PostAsync(apiUrl, content, cancellationToken);

        sw.Stop();

        var responseMessage = default(string);

        using (var sr = new StreamReader(httpResponseMessage.Content.ReadAsStreamAsync(cancellationToken).Result))
        {
            responseMessage = await sr.ReadToEndAsync().ConfigureAwait(false) ?? string.Empty;
        }

        var apiLogEvent = new GameApiLogEvent
        {
            Path = httpResponseMessage.RequestMessage?.RequestUri?.AbsoluteUri.NoLongerThan(512) ?? string.Empty,
            Method = httpResponseMessage.RequestMessage?.Method.ToString().NoLongerThan(8) ?? string.Empty,
            Prefix = logPrefix,
            ServerName = Environment.MachineName.NoLongerThan(128),
            RequestHeaders = httpResponseMessage.RequestMessage?.Headers + httpResponseMessage.RequestMessage?.Content?.Headers?.ToString(),
            RequestContent = (await content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false)).NoLongerThan(4096),
            ResponseHeaders = httpResponseMessage.Headers.ToString(),
            ResponseContent = responseMessage.NoLongerThan(4096),
            ResponseCode = (short)httpResponseMessage.StatusCode,
            ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
            RecordedOn = DateTimeOffset.UtcNow
        };

        _logger.LogInformation("{apiLogEvent}", JsonSerializer.Serialize(apiLogEvent));

        _ = httpResponseMessage.EnsureSuccessStatusCode();

        return JsonNode.Parse(responseMessage)!;
    }
}
