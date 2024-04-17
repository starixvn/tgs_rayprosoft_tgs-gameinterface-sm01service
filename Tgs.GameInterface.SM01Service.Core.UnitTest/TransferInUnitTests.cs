using System.Net;
using Microsoft.Extensions.Logging.Abstractions;
using RichardSzalay.MockHttp;
using Tgs.GameInterface.SM01Service.Core.Exceptions;

namespace Tgs.GameInterface.SM01Service.Core.UnitTest;

[TestClass]
public class TransferInUnitTests
{
	[TestMethod]
	public async Task 主錢包轉入遊戲廠商成功()
	{
		var apiUrl = new Uri("https://cn.ucfclive.com");
		var merchantId = "SQjCxm2cX5bahBta";
		var merchantKey = "XGKGkcmYNnfex8AecKzCEbD4GMExA2cb";
		var customerName = "4ucmqplxb029";
		var refTransactionCode = "4e50abc3-3836-4715-b288-6d75d1a76a76";
		var transactionId = refTransactionCode.Replace("-", "");
		var amount = 1000;
		var balance = 900 + amount;

		var url = new Uri(apiUrl, "/api/merchant/transferIn");
		var response = "{\"process\":\"success\",\"result\":\"\",\"data\":{\"balance\":1900}}";
		var handler = new MockHttpMessageHandler();
		_ = handler.Expect(HttpMethod.Post, url.ToString())
						.Respond(HttpStatusCode.OK, new StringContent(response));

		var fakeHttpClient = handler.ToHttpClient();
		var fakeLog = NullLogger<GameApiClient>.Instance;

		var sut = new GameApiClient(fakeHttpClient, fakeLog);

		var actual = await sut
			.TransferInAsync(
				apiUrl: apiUrl,
				merchantId: merchantId,
				merchantKey: merchantKey,
				customerName: customerName,
				amount: amount,
				merchantTransactionId: refTransactionCode)
			.ConfigureAwait(false);

		Assert.IsNotNull(actual);
		Assert.AreEqual(refTransactionCode, actual.RefTransactionCode);
		Assert.AreEqual(transactionId.ToString(), actual.TransactionId);
		Assert.AreEqual(balance, actual.Balance);
	}

	[TestMethod]
	public async Task 金額給負整數會得到錯誤碼5004()
	{
		var apiUrl = new Uri("https://cn.ucfclive.com");
		var merchantId = "SQjCxm2cX5bahBta";
		var merchantKey = "XGKGkcmYNnfex8AecKzCEbD4GMExA2cb";
		var customerName = "4ucmqplxb029";
		var refTransactionCode = "e62d6131-8401-0000-0001-00008196fcf8";
		var amount = -100;

		var url = new Uri(apiUrl, "/api/merchant/transferIn");
		var response = "{\"process\":\"error\",\"result\":\"Amount can not be less than zero\",\"code\":5004}";
		var handler = new MockHttpMessageHandler();
		_ = handler.Expect(HttpMethod.Post, url.ToString())
						.Respond(HttpStatusCode.OK, new StringContent(response));

		var fakeHttpClient = handler.ToHttpClient();
		var fakeLog = NullLogger<GameApiClient>.Instance;

		var sut = new GameApiClient(fakeHttpClient, fakeLog);

		_ = await Assert
			.ThrowsExceptionAsync<GameProviderResponseErrorException>(() =>
				sut.TransferInAsync(
					apiUrl: apiUrl,
					merchantId: merchantId,
					merchantKey: merchantKey,
					customerName: customerName,
					amount: amount,
					merchantTransactionId: refTransactionCode)
				.AsTask())
			.ConfigureAwait(false);
	}

	[TestMethod]
	public async Task 用重複單號會得到錯誤碼5009()
	{
		var apiUrl = new Uri("https://cn.ucfclive.com");
		var merchantId = "SQjCxm2cX5bahBta";
		var merchantKey = "XGKGkcmYNnfex8AecKzCEbD4GMExA2cb";
		var customerName = "4ucmqplxb029";
		var refTransactionCode = "e62d6131-8401-0000-0001-00008196fcf8";
		var amount = 1000;

		var url = new Uri(apiUrl, "/api/merchant/transferIn");
		var response = "{\"process\":\"error\",\"result\":\"Can not use repetitive merchant transaction id\",\"code\":5009}";
		var handler = new MockHttpMessageHandler();
		_ = handler.Expect(HttpMethod.Post, url.ToString())
						.Respond(HttpStatusCode.OK, new StringContent(response));

		var fakeHttpClient = handler.ToHttpClient();
		var fakeLog = NullLogger<GameApiClient>.Instance;

		var sut = new GameApiClient(fakeHttpClient, fakeLog);

		_ = await Assert
			.ThrowsExceptionAsync<GameProviderResponseErrorException>(() =>
				sut.TransferInAsync(
					apiUrl: apiUrl,
					merchantId: merchantId,
					merchantKey: merchantKey,
					customerName: customerName,
					amount: amount,
					merchantTransactionId: refTransactionCode)
				.AsTask())
			.ConfigureAwait(false);
	}

	[TestMethod]
	public async Task 交易單號超過長度32會得到錯誤碼5008()
	{
		var apiUrl = new Uri("https://cn.ucfclive.com");
		var merchantId = "SQjCxm2cX5bahBta";
		var merchantKey = "XGKGkcmYNnfex8AecKzCEbD4GMExA2cb";
		var customerName = "4ucmqplxb029";
		var refTransactionCode = "e62d6131-8401-0000-0001-00008196fcf8";
		var amount = 1000;

		var url = new Uri(apiUrl, "/api/merchant/transferIn");
		var response = "{\"process\":\"error\",\"result\":\"Params merchantTransactionId length must less than 32\",\"code\":5008}";
		var handler = new MockHttpMessageHandler();
		_ = handler.Expect(HttpMethod.Post, url.ToString())
						.Respond(HttpStatusCode.OK, new StringContent(response));

		var fakeHttpClient = handler.ToHttpClient();
		var fakeLog = NullLogger<GameApiClient>.Instance;

		var sut = new GameApiClient(fakeHttpClient, fakeLog);

		_ = await Assert
			.ThrowsExceptionAsync<GameProviderResponseErrorException>(() =>
				sut.TransferInAsync(
					apiUrl: apiUrl,
					merchantId: merchantId,
					merchantKey: merchantKey,
					customerName: customerName,
					amount: amount,
					merchantTransactionId: refTransactionCode)
				.AsTask())
			.ConfigureAwait(false);
	}
}
