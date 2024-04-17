using System.Net;
using Microsoft.Extensions.Logging.Abstractions;
using RichardSzalay.MockHttp;

namespace Tgs.GameInterface.SM01Service.Core.UnitTest;

[TestClass]
public class GetTransferHistoryUnitTests
{
	[TestMethod]
	public async Task 獲取轉入轉出歷史紀錄()
	{
		var apiUrl = new Uri("https://cn.ucfclive.com");
		var merchantId = "SQjCxm2cX5bahBta";
		var merchantKey = "XGKGkcmYNnfex8AecKzCEbD4GMExA2cb";
		var customerName = "4ucmqplxb029";
		var isFundOut = true;
		var startDate = DateTime.UtcNow.AddMinutes(-5);
		var endDate = DateTime.UtcNow;

		var url = new Uri(apiUrl, "/api/merchant/getTransferHistory");
		var response = "{\"process\":\"success\",\"result\":\"\",\"data\":{\"list\":[{\"merchant_transaction_id\":\"4e50abc338364715b2886d75d1a76a76\",\"amount\":1000,\"created_time\":\"2022-12-07 15:02:31.386\",\"customerName\":\"4ucmqplxb029\"},{\"merchant_transaction_id\":\"e62d613184010000000100008196fcf8\",\"amount\":1000,\"created_time\":\"2022-12-07 10:47:38.320\",\"customerName\":\"4ucmqplxb029\"}]}}";

		var handler = new MockHttpMessageHandler();

		_ = handler
			.Expect(HttpMethod.Post, url.ToString())
			.Respond(HttpStatusCode.OK, new StringContent(response));

		var fakeHttpClient = handler.ToHttpClient();

		var fakeLog = NullLogger<GameApiClient>.Instance;
		var sut = new GameApiClient(fakeHttpClient, fakeLog);

		var actual = await sut
			.GetTransferHistoryAsync(
				apiUrl: apiUrl,
				merchantId: merchantId,
				merchantKey: merchantKey,
				customerName: customerName,
				isFundOut: isFundOut,
				startDate: startDate,
				endDate: endDate)
			.ToArrayAsync()
			.ConfigureAwait(false);

		Assert.IsNotNull(actual);
		Assert.IsTrue(actual.Length > 0);
	}
}
