using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;
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
            ["timestamp"] = DateTime.UtcNow.MillisecondsTimestamp().ToString(),
        };

        var sign = SM01Hasher.ComputeHash(merchantKey, parameters);
        parameters.Add("sign", sign);

        var url = new Uri(apiUrl, "/api/merchant/checkUser");

        var document = await PostAsync(
            url,
            parameters,
            "CheckUser",
            cancellationToken).ConfigureAwait(false);

        if (document is null)
        {
            throw new Exception("Document is empty.");
        }

        if (!document.RootElement.TryGetProperty("process", out var jsonProcess))
        {
            throw new Exception($"JsonElement parsing failed.({JsonSerializer.Serialize(document)})");
        }

        var process = jsonProcess.ToString() == "success";

        if (!process)
        {
            _ = document.RootElement.TryGetProperty("code", out var jsonCode);
            var errorCode = jsonCode.GetInt32();

            _ = document.RootElement.TryGetProperty("result", out var jsonResult);
            var errorMessage = jsonResult.ToString();

            throw new GameProviderResponseErrorException(errorCode.ToString(), errorMessage);
        }

        if (!document.RootElement.TryGetProperty("data", out var jsonData))
        {
            throw new Exception($"JsonElement parsing failed.({JsonSerializer.Serialize(document)})");
        }

        var state = jsonData.GetProperty("state").GetInt32() == 1;//狀態：1正常 2凍結 0禁用

        return new CheckUserResult(state);
    }

    public async IAsyncEnumerable<GetTransationsHistoryResult> GetTransationsHistoryAsync(Uri apiUrl, string merchantId, string merchantKey, DateTime startDate, DateTime endDate, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            ["merchantId"] = merchantId,
            ["timestamp"] = DateTime.UtcNow.MillisecondsTimestamp().ToString(),
            ["StartDate"] = startDate.ToString("yyyy-MM-dd HH:mm:ss"),
            ["EndDate"] = endDate.ToString("yyyy-MM-dd HH:mm:ss"),
        };

        var sign = SM01Hasher.ComputeHash(merchantKey, parameters);
        parameters.Add("sign", sign);

        var url = new Uri(apiUrl, "/api/merchant/getTransationsHistory");

        var document = await PostAsync(
            url,
            parameters,
            "GetTransationsHistory",
            cancellationToken).ConfigureAwait(false);

        if (document is null)
        {
            throw new Exception("Document is empty.");
        }

        if (!document.RootElement.TryGetProperty("process", out var jsonProcess))
        {
            throw new Exception($"JsonElement parsing failed.({JsonSerializer.Serialize(document)})");
        }

        var process = jsonProcess.ToString() == "success";

        if (!process)
        {
            _ = document.RootElement.TryGetProperty("code", out var jsonCode);
            var errorCode = jsonCode.GetInt32();

            _ = document.RootElement.TryGetProperty("result", out var jsonResult);
            var errorMessage = jsonResult.ToString();

            throw new GameProviderResponseErrorException(errorCode.ToString(), errorMessage);
        }
        if (!document.RootElement.TryGetProperty("data", out var jsonData))
        {
            throw new Exception($"JsonElement parsing failed.({JsonSerializer.Serialize(document)})");
        }

        var list = jsonData.GetProperty("list").GetRawText();

        var results = JsonSerializer.Deserialize<IEnumerable<GetTransationsHistoryResult>>(list)
           ?? Enumerable.Empty<GetTransationsHistoryResult>();

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
            ["timestamp"] = DateTime.UtcNow.MillisecondsTimestamp().ToString(),
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

        var document = await PostAsync(
            url,
            parameters,
            "GetTransferHistory",
            cancellationToken).ConfigureAwait(false);

        if (document is null)
        {
            throw new Exception("Document is empty.");
        }

        if (!document.RootElement.TryGetProperty("process", out var jsonProcess))
        {
            throw new Exception($"JsonElement parsing failed.({JsonSerializer.Serialize(document)})");
        }

        var process = jsonProcess.ToString() == "success";

        if (!process)
        {
            _ = document.RootElement.TryGetProperty("code", out var jsonCode);
            var errorCode = jsonCode.ToString();

            _ = document.RootElement.TryGetProperty("result", out var jsonResult);
            var errorMessage = jsonResult.ToString();

            throw new GameProviderResponseErrorException(errorCode, errorMessage);
        }
        if (!document.RootElement.TryGetProperty("data", out var jsonData))
        {
            throw new Exception($"JsonElement parsing failed.({JsonSerializer.Serialize(document)})");
        }

        var list = jsonData.GetProperty("list").GetRawText();

        var results = JsonSerializer.Deserialize<IEnumerable<GetTransferHistoryResult>>(list)
           ?? Enumerable.Empty<GetTransferHistoryResult>();

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
            ["timestamp"] = DateTime.UtcNow.MillisecondsTimestamp().ToString(),
        };

        var sign = SM01Hasher.ComputeHash(merchantKey, parameters);
        parameters.Add("sign", sign);

        var url = new Uri(apiUrl, "/api/merchant/getUserBalance");

        var document = await PostAsync(
            url,
            parameters,
            "GetUserBalance",
            cancellationToken).ConfigureAwait(false);

        if (document is null)
        {
            throw new Exception("Document is empty.");
        }

        if (!document.RootElement.TryGetProperty("process", out var jsonProcess))
        {
            throw new Exception($"JsonElement parsing failed.({JsonSerializer.Serialize(document)})");
        }

        var process = jsonProcess.ToString() == "success";

        if (!process)
        {
            _ = document.RootElement.TryGetProperty("code", out var jsonCode);
            var errorCode = jsonCode.GetInt32();

            _ = document.RootElement.TryGetProperty("result", out var jsonResult);
            var errorMessage = jsonResult.ToString();

            throw new GameProviderResponseErrorException(errorCode.ToString(), errorMessage);
        }

        if (!document.RootElement.TryGetProperty("data", out var jsonData))
        {
            throw new Exception($"JsonElement parsing failed.({JsonSerializer.Serialize(document)})");
        }

        var balance = jsonData.GetProperty("balance").GetInt32();
        return new GetUserBalanceResult(process, balance);
    }

    public async ValueTask<string> PlatformLoginAsync(Uri apiUrl, string merchantId, string merchantKey, string customerName, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            ["merchantId"] = merchantId,
            ["customerName"] = customerName,
            ["timestamp"] = DateTime.UtcNow.MillisecondsTimestamp().ToString(),
        };

        var sign = SM01Hasher.ComputeHash(merchantKey, parameters);
        parameters.Add("sign", sign);

        var url = new Uri(apiUrl, "/auth/platformLogin");

        var sw = Stopwatch.StartNew();

        HttpContent content = new FormUrlEncodedContent(parameters);

        var httpResponseMessage = await _client.PostAsync(url, content, cancellationToken);

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
            Prefix = "PlatformLogin",
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

        return responseMessage;
    }

    public async ValueTask<TransferInResult> TransferInAsync(Uri apiUrl, string merchantId, string merchantKey, string customerName, int amount, string merchantTransactionId, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            ["merchantId"] = merchantId,
            ["customerName"] = customerName,
            ["amount"] = amount.ToString(),
            ["timestamp"] = DateTime.UtcNow.MillisecondsTimestamp().ToString(),
            ["merchantTransactionId"] = merchantTransactionId.Replace("-", ""),
        };

        var sign = SM01Hasher.ComputeHash(merchantKey, parameters);
        parameters.Add("sign", sign);

        var url = new Uri(apiUrl, "/api/merchant/transferIn");

        var document = await PostAsync(
            url,
            parameters,
            "TransferIn",
            cancellationToken).ConfigureAwait(false);

        if (document is null)
        {
            throw new Exception("Document is empty.");
        }

        if (!document.RootElement.TryGetProperty("process", out var jsonProcess))
        {
            throw new Exception($"JsonElement parsing failed.({JsonSerializer.Serialize(document)})");
        }

        var process = jsonProcess.ToString() == "success";

        if (!process)
        {
            _ = document.RootElement.TryGetProperty("code", out var jsonCode);
            var errorCode = jsonCode.GetInt32();

            _ = document.RootElement.TryGetProperty("result", out var jsonResult);
            var errorMessage = jsonResult.ToString();

            throw new GameProviderResponseErrorException(errorCode.ToString(), errorMessage);
        }

        if (!document.RootElement.TryGetProperty("data", out var jsonData))
        {
            throw new Exception($"JsonElement parsing failed.({JsonSerializer.Serialize(document)})");
        }

        var balance = jsonData.GetProperty("balance").GetInt32();

        return new TransferInResult(merchantTransactionId, merchantTransactionId.Replace("-", ""), balance);
    }

    public async ValueTask<TransferOutResult> TransferOutAsync(Uri apiUrl, string merchantId, string merchantKey, string customerName, int amount, string merchantTransactionId, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            ["merchantId"] = merchantId,
            ["customerName"] = customerName,
            ["amount"] = amount.ToString(),
            ["timestamp"] = DateTime.UtcNow.MillisecondsTimestamp().ToString(),
            ["merchantTransactionId"] = merchantTransactionId.Replace("-", ""),//因廠商限制長度32
        };

        var sign = SM01Hasher.ComputeHash(merchantKey, parameters);
        parameters.Add("sign", sign);

        var url = new Uri(apiUrl, "/api/merchant/transferOut");

        var document = await PostAsync(
            url,
            parameters,
            "TransferOut",
            cancellationToken).ConfigureAwait(false);

        if (document is null)
        {
            throw new Exception("Document is empty.");
        }

        if (!document.RootElement.TryGetProperty("process", out var jsonProcess))
        {
            throw new Exception($"JsonElement parsing failed.({JsonSerializer.Serialize(document)})");
        }

        var process = jsonProcess.ToString() == "success";

        if (!process)
        {
            _ = document.RootElement.TryGetProperty("code", out var jsonCode);
            var errorCode = jsonCode.GetInt32();

            _ = document.RootElement.TryGetProperty("result", out var jsonResult);
            var errorMessage = jsonResult.ToString();

            throw new GameProviderResponseErrorException(errorCode.ToString(), errorMessage);
        }

        if (!document.RootElement.TryGetProperty("data", out var jsonData))
        {
            throw new Exception($"JsonElement parsing failed.({JsonSerializer.Serialize(document)})");
        }

        var balance = jsonData.GetProperty("balance").GetInt32();
        return new TransferOutResult(merchantTransactionId, merchantTransactionId.Replace("-", ""), balance);
    }

    private async ValueTask<JsonDocument> PostAsync(Uri apiUrl, IEnumerable<KeyValuePair<string, string>> parameters, string logPrefix, CancellationToken cancellationToken = default)
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

        return JsonDocument.Parse(responseMessage);
    }
}
