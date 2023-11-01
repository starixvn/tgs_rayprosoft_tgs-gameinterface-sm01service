using System.Net;
using Microsoft.Extensions.Logging.Abstractions;
using RichardSzalay.MockHttp;
using Tgs.GameInterface.SM01Service.Core.Exceptions;

namespace Tgs.GameInterface.SM01Service.Core.UnitTest;

[TestClass]
public class GetUnsettleTransactionsHistoryUnitTests
{
    [TestMethod]
    public async Task 查詢未結算賽局成功()
    {
        var apiUrl = new Uri("https://cn.ucfclive.com");
        var merchantId = "SQjCxm2cX5bahBta";
        var merchantKey = "XGKGkcmYNnfex8AecKzCEbD4GMExA2cb";

        var startDate = new DateTime(2023, 01, 11, 16, 20, 00);
        var endDate = new DateTime(2023, 01, 11, 16, 50, 00);

        var url = new Uri(apiUrl, "/api/merchant/getTransationsHistory");
        var response = """
            {
                "process": "success",
                "result": "",
                "data": {
                    "list": [
                        {
                            "bet_id": "8854209",
                            "game_number": "13741",
                            "bet_side": "wala",
                            "win_or_lost": 0,
                            "create_at": "2023-01-11 16:30:04",
                            "update_at": null,
                            "winner": false,
                            "bet_amount": 160,
                            "odds": "",
                            "customerName": "4ue4l6qtysn3"
                        },
                        {
                            "bet_id": "8854208",
                            "game_number": "13741",
                            "bet_side": "meron",
                            "win_or_lost": 0,
                            "create_at": "2023-01-11 16:29:46",
                            "update_at": null,
                            "winner": false,
                            "bet_amount": 100,
                            "odds": "",
                            "customerName": "4uij6bvwtr9i"
                        }
                    ]
                }
            }
            """;

        var handler = new MockHttpMessageHandler();

        _ = handler
            .Expect(HttpMethod.Post, url.ToString())
            .Respond(HttpStatusCode.OK, new StringContent(response));

        var fakeHttpClient = handler.ToHttpClient();

        var fakeLog = NullLogger<GameApiClient>.Instance;
        var sut = new GameApiClient(fakeHttpClient, fakeLog);

        var actual = await sut
            .GetUnsettleTransationsHistoryAsync(
                apiUrl: apiUrl,
                merchantId: merchantId,
                merchantKey: merchantKey,
                startDate: startDate,
                endDate: endDate)
            .ToArrayAsync()
            .ConfigureAwait(false);

        Assert.IsNotNull(actual);
    }

    [TestMethod]
    public void Sign錯誤4003()
    {
        var apiUrl = new Uri("https://cn.ucfclive.com");
        var merchantId = "SQjCxm2cX5bahBta";
        var merchantKey = "XGKGkcmYNnfex8AecKzCEbD4GMExA2cb";

        var startDate = new DateTime(2023, 01, 11, 16, 20, 00);
        var endDate = new DateTime(2023, 01, 11, 16, 50, 00);

        var url = new Uri(apiUrl, "/api/merchant/getTransationsHistory");
        var response = """
            {
                "process": "error",
                "result": "Sign Error",
                "code": "4003"
            }
            """;

        var handler = new MockHttpMessageHandler();

        _ = handler
            .Expect(HttpMethod.Post, url.ToString())
            .Respond(HttpStatusCode.OK, new StringContent(response));

        var fakeHttpClient = handler.ToHttpClient();

        var fakeLog = NullLogger<GameApiClient>.Instance;
        var sut = new GameApiClient(fakeHttpClient, fakeLog);

        _ = Assert.ThrowsExceptionAsync<GameProviderResponseErrorException>(async ()
            => await sut
                .GetUnsettleTransationsHistoryAsync(
                    apiUrl: apiUrl,
                    merchantId: merchantId,
                    merchantKey: merchantKey,
                    startDate: startDate,
                    endDate: endDate)
                .ToArrayAsync()
                .ConfigureAwait(false));
    }
}
