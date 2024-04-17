using System.Net;
using Microsoft.Extensions.Logging.Abstractions;
using RichardSzalay.MockHttp;

namespace Tgs.GameInterface.SM01Service.Core.UnitTest;

[TestClass]
public class CheckUserUnitTests
{
	[TestMethod]
	public async Task Player_Exists_Will_Return_Player_Info()
	{
		var apiUrl = new Uri("https://cn.ucfclive.com");
		var merchnatId = "SQjCxm2cX5bahBta";
		var merchantKey = "XGKGkcmYNnfex8AecKzCEbD4GMExA2cb";
		var customerName = "4ucmqplxb029";

		var url = new Uri(apiUrl, "/api/merchant/checkUser");
		var response = "{\"process\":\"success\",\"result\":\"\",\"data\":{\"user_id\":341,\"balance\":0,\"state\":1,\"created_time\":\"2022-12-07 09:38:16\"}}";
		var handler = new MockHttpMessageHandler();
		_ = handler
			.Expect(HttpMethod.Post, url.ToString())
			.Respond(HttpStatusCode.OK, new StringContent(response));

		var fakeHttpClient = handler.ToHttpClient();
		var fakeLog = NullLogger<GameApiClient>.Instance;

		var sut = new GameApiClient(fakeHttpClient, fakeLog);

		var actual = await sut
			.CheckUserAsync(
				apiUrl: apiUrl,
				merchantId: merchnatId,
				merchantKey: merchantKey,
				customerName: customerName)
			.ConfigureAwait(false);

		Assert.IsNotNull(actual);
		Assert.IsTrue(actual.IsPlayerExist);
	}
}
