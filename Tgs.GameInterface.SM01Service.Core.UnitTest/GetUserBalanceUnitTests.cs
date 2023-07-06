using System.Net;
using Microsoft.Extensions.Logging.Abstractions;
using RichardSzalay.MockHttp;

namespace Tgs.GameInterface.SM01Service.Core.UnitTest;

[TestClass]
public class GetUserBalanceUnitTests
{
    [TestMethod]
    public async Task 當玩家不存在時查詢用戶餘額用得到錯誤碼4004()
    {
        var apiUrl = new Uri("https://cn.ucfclive.com");
        var merchantId = "SQjCxm2cX5bahBta";
        var merchantKey = "XGKGkcmYNnfex8AecKzCEbD4GMExA2cb";
        var customerName = "4ucmqplxb028";

        var url = new Uri(apiUrl, "/api/merchant/getUserBalance");
        var response = "{\"process\":\"error\",\"result\":\"Can not found this user\",\"code\":4004}";
        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, url.ToString())
                        .Respond(HttpStatusCode.OK, new StringContent(response));

        var fakeHttpClient = handler.ToHttpClient();
        var fakeLog = NullLogger<GameApiClient>.Instance;

        var sut = new GameApiClient(fakeHttpClient, fakeLog);

        var actual = await sut
            .GetUserBalanceAsync(
                apiUrl: apiUrl,
                merchantId: merchantId,
                merchantKey: merchantKey,
                customerName: customerName)
            .ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.IsFalse(actual.IsPlayerExist);
        Assert.AreEqual(0, actual.Balance);
    }

    [TestMethod]
    public async Task 取得餘額()
    {
        var apiUrl = new Uri("https://cn.ucfclive.com");
        var merchantId = "SQjCxm2cX5bahBta";
        var merchantKey = "XGKGkcmYNnfex8AecKzCEbD4GMExA2cb";
        var customerName = "4ucmqplxb029";

        var url = new Uri(apiUrl, "/api/merchant/getUserBalance");
        var response = "{\"process\":\"success\",\"result\":\"\",\"data\":{\"balance\":900}}";
        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, url.ToString())
                        .Respond(HttpStatusCode.OK, new StringContent(response));

        var fakeHttpClient = handler.ToHttpClient();
        var fakeLog = NullLogger<GameApiClient>.Instance;

        var sut = new GameApiClient(fakeHttpClient, fakeLog);

        var actual = await sut
            .GetUserBalanceAsync(
                apiUrl: apiUrl,
                merchantId: merchantId,
                merchantKey: merchantKey,
                customerName: customerName)
            .ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.IsTrue(actual.IsPlayerExist);
        Assert.AreEqual(900, actual.Balance);
    }
}
