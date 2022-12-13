using System.Net;
using Microsoft.Extensions.Logging.Abstractions;
using RichardSzalay.MockHttp;
using Tgs.GameInterface.SM01Service.Core.Exceptions;

namespace Tgs.GameInterface.SM01Service.Core.UnitTest;

[TestClass]
public class GetTransactionsHistoryUnitTests
{
    [TestMethod]
    public async Task 獲取交易歷史紀錄()
    {
        var apiUrl = new Uri("https://cn.ucfclive.com");
        var merchantId = "SQjCxm2cX5bahBta";
        var merchantKey = "XGKGkcmYNnfex8AecKzCEbD4GMExA2cb";

        var startDate = DateTime.UtcNow.AddMinutes(-5);
        var endDate = DateTime.UtcNow;

        var url = new Uri(apiUrl, "/api/merchant/getTransationsHistory");
        var response = "{\"process\":\"success\",\"result\":\"\",\"data\":{\"list\":[]}}";

        var handler = new MockHttpMessageHandler();

        _ = handler
            .Expect(HttpMethod.Post, url.ToString())
            .Respond(HttpStatusCode.OK, new StringContent(response));

        var fakeHttpClient = handler.ToHttpClient();

        var fakeLog = NullLogger<GameApiClient>.Instance;
        var sut = new GameApiClient(fakeHttpClient, fakeLog);

        var actual = await sut
            .GetTransationsHistoryAsync(
                apiUrl: apiUrl,
                merchantId: merchantId,
                merchantKey: merchantKey,
                startDate: startDate,
                endDate: endDate)
            .ToArrayAsync()
            .ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.IsTrue(actual.Length == 0);
    }

    [TestMethod]
    public async Task 開始時間與結束時間不能超過30分鐘()
    {
        var apiUrl = new Uri("https://cn.ucfclive.com");
        var merchantId = "SQjCxm2cX5bahBta";
        var merchantKey = "XGKGkcmYNnfex8AecKzCEbD4GMExA2cb";

        var startDate = DateTime.UtcNow.AddMinutes(-31);
        var endDate = DateTime.UtcNow;

        var url = new Uri(apiUrl, "/api/merchant/getTransationsHistory");
        var response = "{\"process\":\"error\",\"result\":\"The maximum time range for each search is 30 minutes\",\"code\":6001}";

        var handler = new MockHttpMessageHandler();

        _ = handler
            .Expect(HttpMethod.Post, url.ToString())
            .Respond(HttpStatusCode.OK, new StringContent(response));

        var fakeHttpClient = handler.ToHttpClient();

        var fakeLog = NullLogger<GameApiClient>.Instance;
        var sut = new GameApiClient(fakeHttpClient, fakeLog);

        _ = await Assert
        .ThrowsExceptionAsync<GameProviderResponseErrorException>(() =>
            sut.GetTransationsHistoryAsync(
                apiUrl: apiUrl,
                merchantId: merchantId,
                merchantKey: merchantKey,
                startDate: startDate,
                endDate: endDate)
            .ToArrayAsync()
            .AsTask())
        .ConfigureAwait(false);
    }
}
