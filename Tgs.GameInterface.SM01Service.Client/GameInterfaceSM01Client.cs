using System.Runtime.CompilerServices;
using Google.Protobuf.WellKnownTypes;
using Tgs.GameInterface.SM01Service.Protos;

namespace Tgs.GameInterface.SM01Service.Client;

internal class GameInterfaceSM01Client(
	GameInterfaceSM01Service.GameInterfaceSM01ServiceClient client) : IGameInterfaceSM01Client
{
	public async ValueTask<string> CheckingServiceAsync(string machineName, CancellationToken cancellationToken = default)
	{
		var reply = await client
			.CheckingServiceAsync(
				request: new CheckingServiceRequest
				{
					MachineName = machineName
				},
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return reply.MachineName;
	}

	public async ValueTask<FundReply> FundInAsync(Uri apiUrl, string merchantId, string merchantKey, string customerName, string refTransactionCode, int amount, CancellationToken cancellationToken = default)
	{
		var request = new FundRequest()
		{
			ApiUrl = apiUrl.ToString(),
			MerchantId = merchantId,
			MerchantKey = merchantKey,
			CustomerName = customerName,
			RefTransactionCode = refTransactionCode,
			Amount = amount
		};

		return await client
				.FundInAsync(
					request: request,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);
	}

	public async ValueTask<FundReply> FundOutAsync(Uri apiUrl, string merchantId, string merchantKey, string customerName, string refTransactionCode, int amount, CancellationToken cancellationToken = default)
	{
		var request = new FundRequest()
		{
			ApiUrl = apiUrl.ToString(),
			MerchantId = merchantId,
			MerchantKey = merchantKey,
			CustomerName = customerName,
			RefTransactionCode = refTransactionCode,
			Amount = amount
		};

		return await client
				.FundOutAsync(
					request: request,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);
	}

	public async IAsyncEnumerable<FundHistory> FundHistoryAsync(
		Uri apiUrl,
		string merchantId,
		string merchantKey,
		string customerName,
		bool isFundOut,
		DateTime startDate,
		DateTime endDate,
		[EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var request = new FundHistoryRequest()
		{
			ApiUrl = apiUrl.ToString(),
			MerchantId = merchantId,
			MerchantKey = merchantKey,
			CustomerName = customerName,
			IsFundOut = isFundOut,
			StartDate = startDate.ToTimestamp(),
			EndDate = endDate.ToTimestamp()
		};

		var reply = await client
				.FundHistoryAsync(
					request: request,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

		if (reply.Items.Count == 0)
		{
			yield break;
		}

		foreach (var item in reply.Items)
		{
			yield return item;
		}
	}

	public async IAsyncEnumerable<GameSettledHistory> GameSettledHistoryAsync(
		Uri apiUrl,
		string merchantId,
		string merchantKey,
		DateTime startDate,
		DateTime endDate,
		[EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var request = new GameSettledHistoryRequest()
		{
			ApiUrl = apiUrl.ToString(),
			MerchantId = merchantId,
			MerchantKey = merchantKey,
			StartDate = startDate.ToTimestamp(),
			EndDate = endDate.ToTimestamp()
		};

		var reply = await client
				.GameSettledHistoryAsync(
					request: request,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

		if (reply.Items.Count == 0)
		{
			yield break;
		}

		foreach (var item in reply.Items)
		{
			yield return item;
		}
	}

	public async IAsyncEnumerable<GameUnsettleHistory> GameUnsettleHistoryAsync(
		Uri apiUrl,
		string merchantId,
		string merchantKey,
		DateTime startDate,
		DateTime endDate,
		[EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var request = new GameUnsettleHistoryRequest()
		{
			ApiUrl = apiUrl.ToString(),
			MerchantId = merchantId,
			MerchantKey = merchantKey,
			StartDate = startDate.ToTimestamp(),
			EndDate = endDate.ToTimestamp()
		};

		var reply = await client
				.GameUnsettleHistoryAsync(
					request: request,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

		if (reply.Items.Count == 0)
		{
			yield break;
		}

		foreach (var item in reply.Items)
		{
			yield return item;
		}
	}

	public async ValueTask<PlayerBalanceReply> PlayerBalanceAsync(Uri apiUrl, string merchantId, string merchantKey, string customerName, CancellationToken cancellationToken = default)
	{
		var request = new PlayerBalanceRequest()
		{
			ApiUrl = apiUrl.ToString(),
			MerchantId = merchantId,
			MerchantKey = merchantKey,
			CustomerName = customerName,
		};

		return await client
				.PlayerBalanceAsync(
					request: request,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);
	}

	public async ValueTask PlayerCreateAsync(Uri apiUrl, string merchantId, string merchantKey, string customerName, CancellationToken cancellationToken = default)
	{
		var request = new PlayerCreateRequest()
		{
			ApiUrl = apiUrl.ToString(),
			MerchantId = merchantId,
			MerchantKey = merchantKey,
			CustomerName = customerName,
		};

		_ = await client
				.PlayerCreateAsync(
					request: request,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);
	}

	public async ValueTask<PlatformLoginReply> PlatformLoginAsync(Uri apiUrl, string merchantId, string merchantKey, string customerName, CancellationToken cancellationToken = default)
	{
		var request = new PlatformLoginRequest()
		{
			ApiUrl = apiUrl.ToString(),
			MerchantId = merchantId,
			MerchantKey = merchantKey,
			CustomerName = customerName,
		};

		return await client
			.PlatformLoginAsync(
				request: request,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
