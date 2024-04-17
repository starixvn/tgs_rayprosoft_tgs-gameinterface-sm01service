using System.Net;
using Microsoft.Extensions.Logging.Abstractions;
using RichardSzalay.MockHttp;
using Tgs.GameInterface.SM01Service.Core.Exceptions;

namespace Tgs.GameInterface.SM01Service.Core.UnitTest;

[TestClass]
public class GetSettledTransactionsHistoryUnitTests
{
	[TestMethod]
	public async Task 獲取交易歷史紀錄()
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
                            "win_or_lost": -160,
                            "create_at": "2023-01-11 16:30:04",
                            "update_at": "2023-01-11 16:35:59",
                            "winner": "meron",
                            "bet_amount": 160,
                            "odds": "195.00",
                            "customerName": "4ue4l6qtysn3"
                        },
                        {
                            "bet_id": "8854208",
                            "game_number": "13741",
                            "bet_side": "meron",
                            "win_or_lost": 95,
                            "create_at": "2023-01-11 16:29:46",
                            "update_at": "2023-01-11 16:35:59",
                            "winner": "meron",
                            "bet_amount": 100,
                            "odds": "195.00",
                            "customerName": "4uij6bvwtr9i"
                        },
                        {
                            "bet_id": "8854207",
                            "game_number": "13740",
                            "bet_side": "meron",
                            "win_or_lost": -200,
                            "create_at": "2023-01-11 16:20:08",
                            "update_at": "2023-01-11 16:25:55",
                            "winner": "wala",
                            "bet_amount": 200,
                            "odds": "200.00",
                            "customerName": "4ue4l6qtysn3"
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
			.GetSettledTransationsHistoryAsync(
				apiUrl: apiUrl,
				merchantId: merchantId,
				merchantKey: merchantKey,
				startDate: startDate,
				endDate: endDate)
			.ToArrayAsync()
			.ConfigureAwait(false);

		Assert.IsNotNull(actual);
		Assert.AreEqual(3, actual.Length);
		Assert.AreEqual("4ue4l6qtysn3", actual[0].PlayerCode);
		Assert.AreEqual("8854209", actual[0].ExternalRoundId);
		Assert.AreEqual(160M, actual[0].Bet);
	}

	[TestMethod]
	public async Task 獲取交易歷史紀錄_取消比賽()
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
                            "bet_id": "8854230",
                            "game_number": "4",
                            "bet_side": "draw",
                            "win_or_lost": 800,
                            "create_at": "2023-01-16 17:42:54",
                            "update_at": "2023-01-16 17:48:04",
                            "winner": "draw",
                            "bet_amount": 100,
                            "odds": "800.00",
                            "customerName": "4uglyyqu4nae"
                        },
                        {
                            "bet_id": "8854229",
                            "game_number": "4",
                            "bet_side": "draw",
                            "win_or_lost": 800,
                            "create_at": "2023-01-16 17:40:52",
                            "update_at": "2023-01-16 17:48:04",
                            "winner": "draw",
                            "bet_amount": 100,
                            "odds": "800.00",
                            "customerName": "4uglyyqu4nae"
                        },
                        {
                            "bet_id": "8854228",
                            "game_number": "3",
                            "bet_side": "draw",
                            "win_or_lost": 0,
                            "create_at": "2023-01-16 17:33:17",
                            "update_at": "2023-01-16 17:35:54",
                            "winner": false,
                            "bet_amount": 500,
                            "odds": "",
                            "customerName": "4uglyyqu4nae"
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
			.GetSettledTransationsHistoryAsync(
				apiUrl: apiUrl,
				merchantId: merchantId,
				merchantKey: merchantKey,
				startDate: startDate,
				endDate: endDate)
			.ToArrayAsync()
			.ConfigureAwait(false);

		Assert.IsNotNull(actual);
		Assert.AreEqual(3, actual.Length);
		Assert.AreEqual(true, actual[2].IsCancelled);
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
			sut.GetSettledTransationsHistoryAsync(
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
